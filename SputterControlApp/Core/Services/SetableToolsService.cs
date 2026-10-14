using SputterControlApp.Logging; // LogEvent, LogLevel, LogSource
using System.Globalization;
using SputterControlApp.Core.Helpers;   // ModbusCommandHelper, ModbusTypeExtensions
using SputterControlApp.Core.Services;  // IModbusClientService

namespace SputterControlApp.Services
{
    public sealed class SetableToolsService
    {
        private readonly IModbusClientService _modbus;
        private readonly Action<string>? _postStatus;
        private readonly Action<Exception>? _onError;
        private readonly Action<LogEvent>? _publishLog;

        public SetableToolsService(
            IModbusClientService modbus,
            Action<string>? postStatus = null,
            Action<Exception>? onError = null,
            Action<LogEvent>? publishLog = null)
        {
            _modbus = modbus ?? throw new ArgumentNullException(nameof(modbus));
            _postStatus = postStatus;
            _onError = onError;
            _publishLog = publishLog;
        }

        public enum CommitMode
        {
            AutoDebounce,   // Enter'sız: kullanıcı durunca gönder (debounce)
            EnterOnly       // Enter ile gönder
        }

        private void TryLog(LogEvent ev)
        {
            try { _publishLog?.Invoke(ev); }
            catch { /* log yüzünden UI kırılmasın */ }
        }


        // ============================================================
        //  Button Toggle Helpers (mevcut davranış korunur)
        // ============================================================

        public void BindOnOffButton(
            Button button,
            ushort writeAddress,
            ushort readAddress,
            ushort offValue = 0,
            ushort onValue = 1,
            string offText = "OFF",
            string onText = "ON",
            Color? offColor = null,
            Color? onColor = null,
            Func<Task>? onOnPressed_NoConfirm = null,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (button == null) throw new ArgumentNullException(nameof(button));

            offColor ??= Color.IndianRed;
            onColor ??= Color.ForestGreen;

            button.Click += async (_, __) =>
            {
                if (_modbus.IsConnected != true) return;

                var oldEnabled = button.Enabled;
                var oldText = button.Text;
                var oldBack = button.BackColor;
                var oldFore = button.ForeColor;
                var oldFlat = button.FlatStyle;

                try
                {
                    // 1) current state oku
                    var rb = await _modbus.ReadHoldingRegistersAsync(readAddress, 1);
                    ushort currentState = rb[0];

                    // 2) target üret (toggle)
                    ushort target = (ushort)(currentState == onValue ? offValue : onValue);

                    // ✅ B) ON’a basıldığı AN (confirm beklemeden) ek işi çalıştır
                    if (target == onValue && onOnPressed_NoConfirm != null)
                    {
                        try { await onOnPressed_NoConfirm(); }
                        catch (Exception ex) { _onError?.Invoke(ex); }
                    }

                    // 3) pending
                    ApplyPending(button);

                    // 4) write + confirm (tek register)
                    var (confirmed, lastRead) = await ModbusCommandHelper.WriteSingleRegisterAndConfirmAsync(
                        modbus: _modbus,
                        writeAddress: writeAddress,
                        targetValue: target,
                        readAddress: readAddress,
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    if (!confirmed)
                    {
                        // teyit yok → eski UI'ya dön
                        button.Text = oldText;
                        button.BackColor = oldBack;
                        button.ForeColor = oldFore;
                        button.FlatStyle = oldFlat;
                        button.Enabled = oldEnabled;

                        _postStatus?.Invoke("Command not confirmed.");
                        // başarısız log
                        TryLog(new LogEvent
                        {
                            Level = LogLevel.Warn,
                            Code = "STATE",
                            Source = LogSource.System,
                            Message = target == onValue
                                ? $"{button.AccessibleName} ON request failed (no confirmation)."
                                : $"{button.AccessibleName} OFF request failed (no confirmation)."
                        });

                        return;
                    }

                    // 5) UI update (lastRead’e göre)
                    if (lastRead == onValue)
                    {
                        button.Tag = true;
                        button.Text = onText;
                        button.BackColor = onColor.Value;
                    }
                    else if (lastRead == offValue)
                    {
                        button.Tag = false;
                        button.Text = offText;
                        button.BackColor = offColor.Value;
                    }
                    else
                    {
                        button.Tag = null;
                        button.Text = $"VAL={lastRead}";
                        button.BackColor = Color.DimGray;
                    }

                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    // başarılı log
                    TryLog(new LogEvent
                    {
                        Level = LogLevel.Info,
                        Code = "STATE",
                        Source = LogSource.System,
                        Message = target == onValue
                            ? $"{button.AccessibleName} successfully turned ON."
                            : $"{button.AccessibleName} successfully turned OFF."
                    });


                    _postStatus?.Invoke($"Confirmed: {lastRead}");
                }
                catch (Exception ex)
                {
                    // hata → eski UI
                    button.Text = oldText;
                    button.BackColor = oldBack;
                    button.ForeColor = oldFore;
                    button.FlatStyle = oldFlat;
                    button.Enabled = oldEnabled;
                    // hata log
                    TryLog(new LogEvent
                    {
                        Time = DateTime.Now,
                        Level = LogLevel.Error,
                        Code = "ERR",
                        Source = LogSource.System,
                        Message = $"{button.Name} error: {ex.GetType().Name} - {ex.Message}"
                    });
                    _onError?.Invoke(ex);
                }
                finally
                {
                    button.Enabled = oldEnabled;
                }
            };
        }

        // ============================================================
        //  Button Pending Style Helper
        // ============================================================
        public static void ApplyPending(Button btn)
        {
            btn.Enabled = false;
            btn.Text = "PENDING...";
            btn.BackColor = Color.DimGray;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
        }

        // ============================================================
        //  Core Write+Confirm helpers (Float32 / U16)
        // ============================================================

        public async Task<(bool confirmed, ushort[] lastRead)> WriteFloat32AndConfirmAsync(
            ushort writeAddress,
            float target,
            ushort readAddress,
            Func<ushort[], bool>? isConfirmed = null,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            var (lowWord, highWord) = ModbusTypeExtensions.FromFloat32_CDAB(target);
            ushort[] writeWords = { lowWord, highWord };

            isConfirmed ??= regs =>
            {
                float readVal = regs.ToFloat32_CDAB();
                return NearlyEqual(readVal, target, 0.001f);
            };

            return await ModbusCommandHelper.WriteRegistersAndConfirmAsync(
                modbus: _modbus,
                writeAddress: writeAddress,
                writeValues: writeWords,
                readAddress: readAddress,
                readCount: 2,
                isConfirmed: isConfirmed,
                retries: retries,
                timeoutMs: timeoutMs,
                pollDelayMs: pollDelayMs,
                retryDelayMs: retryDelayMs
            );
        }

        public async Task<(bool confirmed, ushort lastRead)> WriteU16AndConfirmAsync(
            ushort writeAddress,
            ushort target,
            ushort readAddress,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            return await ModbusCommandHelper.WriteSingleRegisterAndConfirmAsync(
                modbus: _modbus,
                writeAddress: writeAddress,
                targetValue: target,
                readAddress: readAddress,
                retries: retries,
                timeoutMs: timeoutMs,
                pollDelayMs: pollDelayMs,
                retryDelayMs: retryDelayMs
            );
        }

        // ============================================================
        //  FLOAT32 TextBox binder (2 register, CDAB)
        // ============================================================

        public void BindFloat32TextBox(
            TextBox tb,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            float min,
            float max,
            int decimals,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 500,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (tb == null) throw new ArgumentNullException(nameof(tb));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));

            bool internalSet = false;
            float? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(tb, msg ?? "!");
                    tb.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(tb, "");
                    tb.BackColor = SystemColors.Window;
                }
            }

            bool TryParse(out float v, out string? err)
            {
                err = null;
                v = 0;

                var s = tb.Text?.Trim().Replace(',', '.');

                if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                {
                    err = "Geçersiz sayı";
                    return false;
                }

                if (v < min || v > max)
                {
                    err = $"Aralık dışı ({min}..{max})";
                    return false;
                }

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    err = ex.Value.err ?? "Geçersiz değer";
                    return false;
                }

                return true;
            }

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (_modbus.IsConnected != true) return;

                if (!TryParse(out var target, out var err))
                {
                    MarkDirty(true, err);
                    return;
                }

                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = tb.Enabled;
                var oldBack = tb.BackColor;
                tb.Enabled = false;

                try
                {
                    var (confirmed, lastReadRegs) = await WriteFloat32AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: target,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    float readVal = lastReadRegs.ToFloat32_CDAB();
                    lastConfirmed = readVal;

                    if (!confirmed)
                    {
                        warn?.Invoke($"Doğrulanamadı.\nHedef={target}\nOkunan={readVal}");
                    }
                    else if (!NearlyEqual(readVal, target, 0.001f))
                    {
                        warn?.Invoke($"Uyarı: Hedef({target}) != Okunan({readVal}). Okunan değer yazıldı.");
                    }

                    internalSet = true;
                    tb.Text = readVal.ToString($"F{decimals}", CultureInfo.InvariantCulture);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex)
                {
                    _onError?.Invoke(ex);
                    warn?.Invoke($"Yazma hatası: {ex.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    //tb.BackColor = oldBack;
                    tb.Enabled = oldEnabled;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            tb.TextChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            tb.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        tb.Text = lastConfirmed.Value.ToString($"F{decimals}", CultureInfo.InvariantCulture);
                        internalSet = false;
                        MarkDirty(false);
                    }

                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            tb.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        // ============================================================
        //  FLOAT32 NumericUpDown binder (2 register, CDAB)
        // ============================================================

        public void BindFloat32NumericUpDown(
            NumericUpDown nud,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            float min,
            float max,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 1000,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (nud == null) throw new ArgumentNullException(nameof(nud));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));

            bool internalSet = false;
            float? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(nud, msg ?? "Enter'a basılmadı!");
                    nud.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(nud, "");
                    nud.BackColor = SystemColors.Window;
                }
            }

            float GetTarget()
            {
                // NumericUpDown decimal -> float
                return (float)nud.Value;
            }

            bool committing = false;
            bool disableByCommit = false;

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (committing) return;          // ✅ kritik: re-entry engeli
                if (_modbus.IsConnected != true) return;

                committing = true;

                // Commit sırasında timer tekrar tetiklemesin
                tmr.Stop();

                var prevEnabled = nud.Enabled;   // dışarıdan disable edilmiş olabilir
                disableByCommit = true;
                nud.Enabled = false;

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    MarkDirty(true, ex.Value.err);
                    return;
                }

                float target = GetTarget();

                if (target < min || target > max)
                {
                    MarkDirty(true, $"Aralık dışı ({min}..{max})");
                    return;
                }

                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = nud.Enabled;
                var oldBack = nud.BackColor;
                nud.Enabled = false;

                try
                {
                    var (confirmed, lastReadRegs) = await WriteFloat32AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: target,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    float readVal = lastReadRegs.ToFloat32_CDAB();
                    lastConfirmed = readVal;

                    internalSet = true;
                    nud.Value = ClampToRange(nud, (decimal)readVal);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex2)
                {
                    _onError?.Invoke(ex2);
                    warn?.Invoke($"Yazma hatası: {ex2.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    // ✅ sadece bu commit disable ettiyse geri aç
                    if (disableByCommit)
                        nud.Enabled = prevEnabled; // dışarıdan disable ise false kalmalı
                    disableByCommit = false;

                    committing = false;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            nud.ValueChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            nud.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        nud.Value = ClampToRange(nud, (decimal)lastConfirmed.Value);
                        internalSet = false;
                        MarkDirty(false);
                    }

                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            nud.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        // ============================================================
        //  U16 TextBox binder (1 register)
        // ============================================================

        public void BindUInt16TextBox(
            TextBox tb,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            ushort min,
            ushort max,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 500,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (tb == null) throw new ArgumentNullException(nameof(tb));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));


            bool internalSet = false;
            ushort? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(tb, msg ?? "!");
                    tb.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(tb, "");
                    tb.BackColor = SystemColors.Window;
                }
            }

            bool TryParse(out ushort v, out string? err)
            {
                err = null;
                v = 0;

                var s = tb.Text?.Trim();
                if (!ushort.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                {
                    err = "Geçersiz sayı";
                    return false;
                }

                if (v < min || v > max)
                {
                    err = $"Aralık dışı ({min}..{max})";
                    return false;
                }

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    err = ex.Value.err ?? "Geçersiz değer";
                    return false;
                }

                return true;
            }

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (_modbus.IsConnected != true) return;

                if (!TryParse(out var target, out var err))
                {
                    MarkDirty(true, err);
                    return;
                }

                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = tb.Enabled;
                var oldBack = tb.BackColor;
                tb.Enabled = false;

                try
                {
                    var (confirmed, lastRead) = await WriteU16AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: target,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    lastConfirmed = lastRead;

                    if (!confirmed)
                        warn?.Invoke($"Doğrulanamadı.\nHedef={target}\nOkunan={lastRead}");
                    else if (lastRead != target)
                        warn?.Invoke($"Uyarı: Hedef({target}) != Okunan({lastRead}). Okunan değer yazıldı.");

                    internalSet = true;
                    tb.Text = lastRead.ToString(CultureInfo.InvariantCulture);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex2)
                {
                    _onError?.Invoke(ex2);
                    warn?.Invoke($"Yazma hatası: {ex2.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    //tb.BackColor = oldBack;
                    tb.Enabled = oldEnabled;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            tb.TextChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            tb.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        tb.Text = lastConfirmed.Value.ToString(CultureInfo.InvariantCulture);
                        internalSet = false;
                        MarkDirty(false);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            tb.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        // ============================================================
        //  INT16 TextBox binder (1 register)
        // ============================================================

        public void BindInt16TextBox(
            TextBox tb,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            short min,
            short max,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 500,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            // Int16 = register’a unchecked(ushort) yazılır; readback ushort gelir -> unchecked(short)

            if (tb == null) throw new ArgumentNullException(nameof(tb));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));


            bool internalSet = false;
            short? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(tb, msg ?? "!");
                    tb.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(tb, "");
                    tb.BackColor = SystemColors.Window;
                }
            }

            bool TryParse(out short v, out string? err)
            {
                err = null;
                v = 0;

                var s = tb.Text?.Trim();
                if (!short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                {
                    err = "Geçersiz sayı";
                    return false;
                }

                if (v < min || v > max)
                {
                    err = $"Aralık dışı ({min}..{max})";
                    return false;
                }

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    err = ex.Value.err ?? "Geçersiz değer";
                    return false;
                }

                return true;
            }

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (_modbus.IsConnected != true) return;

                if (!TryParse(out var target, out var err))
                {
                    MarkDirty(true, err);
                    return;
                }

                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = tb.Enabled;
                var oldBack = tb.BackColor;
                tb.Enabled = false;

                try
                {
                    ushort targetU16 = unchecked((ushort)target);

                    var (confirmed, lastReadU16) = await WriteU16AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: targetU16,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    short lastRead = unchecked((short)lastReadU16);
                    lastConfirmed = lastRead;

                    if (!confirmed)
                        warn?.Invoke($"Doğrulanamadı.\nHedef={target}\nOkunan={lastRead}");
                    else if (lastRead != target)
                        warn?.Invoke($"Uyarı: Hedef({target}) != Okunan({lastRead}). Okunan değer yazıldı.");

                    internalSet = true;
                    tb.Text = lastRead.ToString(CultureInfo.InvariantCulture);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex2)
                {
                    _onError?.Invoke(ex2);
                    warn?.Invoke($"Yazma hatası: {ex2.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    //tb.BackColor = oldBack;
                    tb.Enabled = oldEnabled;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            tb.TextChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            tb.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        tb.Text = lastConfirmed.Value.ToString(CultureInfo.InvariantCulture);
                        internalSet = false;
                        MarkDirty(false);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            tb.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        // ============================================================
        //  NumericUpDown binders (U16 / Int16)
        // ============================================================

        public void BindUInt16NumericUpDown(
            NumericUpDown nud,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            ushort min,
            ushort max,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 500,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (nud == null) throw new ArgumentNullException(nameof(nud));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));

            bool internalSet = false;
            ushort? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(nud, msg ?? "!");
                    nud.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(nud, "");
                    nud.BackColor = SystemColors.Window;
                }
            }

            ushort GetTarget()
            {
                decimal v = nud.Value;
                if (v < min) v = min;
                if (v > max) v = max;
                return (ushort)v;
            }

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (_modbus.IsConnected != true) return;

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    MarkDirty(true, ex.Value.err);
                    return;
                }

                ushort target = GetTarget();
                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = nud.Enabled;
                var oldBack = nud.BackColor;
                nud.Enabled = false;

                try
                {
                    var (confirmed, lastRead) = await WriteU16AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: target,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    lastConfirmed = lastRead;

                    if (!confirmed)
                        warn?.Invoke($"Doğrulanamadı.\nHedef={target}\nOkunan={lastRead}");
                    else if (lastRead != target)
                        warn?.Invoke($"Uyarı: Hedef({target}) != Okunan({lastRead}). Okunan değer yazıldı.");

                    internalSet = true;
                    nud.Value = ClampToRange(nud, lastRead);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex2)
                {
                    _onError?.Invoke(ex2);
                    warn?.Invoke($"Yazma hatası: {ex2.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    //nud.BackColor = oldBack;
                    nud.Enabled = oldEnabled;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            nud.ValueChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            nud.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        nud.Value = ClampToRange(nud, lastConfirmed.Value);
                        internalSet = false;
                        MarkDirty(false);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            nud.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        public void BindInt16NumericUpDown(
            NumericUpDown nud,
            ErrorProvider ep,
            Func<ushort> getWriteAddr,
            Func<ushort> getReadAddr,
            short min,
            short max,
            CommitMode mode,
            Func<(bool ok, string? err)>? extraValidate = null,
            Action<string>? warn = null,
            int debounceMs = 1000,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200)
        {
            if (nud == null) throw new ArgumentNullException(nameof(nud));
            if (ep == null) throw new ArgumentNullException(nameof(ep));
            if (getWriteAddr == null) throw new ArgumentNullException(nameof(getWriteAddr));
            if (getReadAddr == null) throw new ArgumentNullException(nameof(getReadAddr));

            bool internalSet = false;
            short? lastConfirmed = null;

            var tmr = new System.Windows.Forms.Timer { Interval = debounceMs };

            void MarkDirty(bool dirty, string? msg = null)
            {
                if (dirty)
                {
                    ep.SetError(nud, msg ?? "!");
                    nud.BackColor = Color.LightYellow;
                }
                else
                {
                    ep.SetError(nud, "");
                    nud.BackColor = SystemColors.Window;
                }
            }

            short GetTarget()
            {
                decimal v = nud.Value;
                if (v < min) v = min;
                if (v > max) v = max;
                return (short)v;
            }

            async Task CommitAsync()
            {
                if (internalSet) return;
                if (_modbus.IsConnected != true) return;

                var ex = extraValidate?.Invoke();
                if (ex.HasValue && !ex.Value.ok)
                {
                    MarkDirty(true, ex.Value.err);
                    return;
                }

                short target = GetTarget();
                ushort targetU16 = unchecked((ushort)target);

                MarkDirty(true, "Gönderiliyor...");

                var oldEnabled = nud.Enabled;
                var oldBack = nud.BackColor;
                nud.Enabled = false;

                try
                {
                    var (confirmed, lastReadU16) = await WriteU16AndConfirmAsync(
                        writeAddress: getWriteAddr(),
                        target: targetU16,
                        readAddress: getReadAddr(),
                        retries: retries,
                        timeoutMs: timeoutMs,
                        pollDelayMs: pollDelayMs,
                        retryDelayMs: retryDelayMs);

                    short lastRead = unchecked((short)lastReadU16);
                    lastConfirmed = lastRead;

                    if (!confirmed)
                        warn?.Invoke($"Doğrulanamadı.\nHedef={target}\nOkunan={lastRead}");
                    else if (lastRead != target)
                        warn?.Invoke($"Uyarı: Hedef({target}) != Okunan({lastRead}). Okunan değer yazıldı.");

                    internalSet = true;
                    nud.Value = ClampToRange(nud, (decimal)lastRead);
                    internalSet = false;

                    MarkDirty(false);
                }
                catch (Exception ex2)
                {
                    _onError?.Invoke(ex2);
                    warn?.Invoke($"Yazma hatası: {ex2.Message}");
                    MarkDirty(true, "Hata");
                }
                finally
                {
                    //nud.BackColor = oldBack;
                    nud.Enabled = oldEnabled;
                }
            }

            tmr.Tick += async (_, __) =>
            {
                tmr.Stop();
                if (mode == CommitMode.AutoDebounce)
                    await CommitAsync();
            };

            nud.ValueChanged += (_, __) =>
            {
                if (internalSet) return;
                MarkDirty(true);

                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    tmr.Start();
                }
            };

            nud.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    if (lastConfirmed.HasValue)
                    {
                        internalSet = true;
                        nud.Value = ClampToRange(nud, (decimal)lastConfirmed.Value);
                        internalSet = false;
                        MarkDirty(false);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (mode == CommitMode.EnterOnly)
                        await CommitAsync();
                }
            };

            nud.Leave += async (_, __) =>
            {
                if (mode == CommitMode.AutoDebounce)
                {
                    tmr.Stop();
                    await CommitAsync();
                }
            };
        }

        // ============================================================
        //  Helpers
        // ============================================================

        private static bool NearlyEqual(float a, float b, float tol)
        {
            float diff = Math.Abs(a - b);
            if (diff <= tol) return true;

            float scale = Math.Max(1f, Math.Max(Math.Abs(a), Math.Abs(b)));
            return diff <= tol * scale;
        }

        private static decimal ClampToRange(NumericUpDown nud, decimal value)
        {
            if (value < nud.Minimum) return nud.Minimum;
            if (value > nud.Maximum) return nud.Maximum;
            return value;
        }

        private static decimal ClampToRange(NumericUpDown nud, ushort value)
        {
            decimal v = value;
            if (v < nud.Minimum) return nud.Minimum;
            if (v > nud.Maximum) return nud.Maximum;
            return v;
        }
    }
}
