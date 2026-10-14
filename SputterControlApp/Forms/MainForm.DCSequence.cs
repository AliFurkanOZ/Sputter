using SputterControlApp.Core.Registers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm
    {
        // =====================================================
        // DC CONTROL - Clean architecture + Preview + Dynamic Step2 (FIXED UI THREAD)
        // - ApplyDcButtonUi: only draws UI + sets Tag (sende var)
        // - PLC confirm: DeviceToggleSupervisor ile gecikmeler yakalanır
        // - Preview: user changes inputs -> rTime shows immediately (no need to start)
        // - Dynamic Step2: while Step2 counting, changing SetW/Ramp/Dwell updates remaining time
        //   Formula while Step2 running: (SetW2 - txtDC_ReadPower) / Ramp2
        // =====================================================

        // ------------------------
        // DC state (confirmed)
        // ------------------------
        private bool? _dcConfirmedState = null;

        // ------------------------
        // DC sequence
        // ------------------------
        private CancellationTokenSource? _dcSeqCts;
        private bool _dcSeqRunning = false;

        // ------------------------
        // DC phases + dynamic Step2 timing
        // ------------------------
        private enum DcRunPhase { None, Step1, Step2 }
        private volatile DcRunPhase _dcPhase = DcRunPhase.None;

        private readonly object _dcStep2Sync = new();
        private DateTime _dcStep2StartedAtUtc;
        private TimeSpan _dcStep2Total;

        // ------------------------
        // DC supervisor
        // ------------------------
        private DeviceToggleSupervisor? _dcSup;

        // =====================================================
        // 0) Setup supervisor + preview handlers (call once)
        // =====================================================
        private void SetupDcSupervisor()
        {
            _dcSup?.Dispose();

            _dcSup = new DeviceToggleSupervisor(
                uiOwner: this,
                button: btnControlDC,
                readStatusAsync: async (ct) =>
                {
                    var regs = await _modbus!.ReadHoldingRegistersAsync((ushort)RegisterId.DC_Set_Out, 1);
                    return unchecked((short)regs[0]);
                },
                offValue: 0,
                onValue: 1,
                applyUi: ApplyDcButtonUi, // sende var (Tag + renk + SetDCStep1InputsEnabled çağırıyor)
                onConfirmedOn: _ => StartDcSequenceIfNotRunning(),
                onConfirmedOff: _ => CancelDcSequence(),
                triggerRefresh: () => btnRefresh.PerformClick()
            );

            WireDcTimePreviewHandlers();
        }
        // =====================================================
        // 1) Apply DC button UI state
        // =====================================================
        private void ApplyDcButtonUi(bool? state)
        {
            if (state == true)
            {
                btnControlDC.Text = "DC ON";
                btnControlDC.BackColor = Color.ForestGreen;
                btnControlDC.ForeColor = Color.White;
            }
            else if (state == false)
            {
                btnControlDC.Text = "DC OFF";
                btnControlDC.BackColor = Color.IndianRed;
                btnControlDC.ForeColor = Color.White;
            }
            else
            {
                btnControlDC.Text = "DC Status Pending...";
                btnControlDC.BackColor = Color.SlateGray;
                btnControlDC.ForeColor = Color.GhostWhite;
            }

            btnControlDC.Tag = state;

            if (state == true) SetDCStep1InputsEnabled(false);
            else if (state == false) SetDCStep1InputsEnabled(true);

            UpdateTargetSelectionLocks();
        }

        // =====================================================
        // Preview event wiring
        // =====================================================
        private void WireDcTimePreviewHandlers()
        {
            // Step1 textboxes
            txtDc_Step1_Dwell.TextChanged -= DcInputs_Changed;
            txtDc_Step1_SetW.TextChanged -= DcInputs_Changed;
            txtDc_Step1_Ramp.TextChanged -= DcInputs_Changed;

            txtDc_Step1_Dwell.TextChanged += DcInputs_Changed;
            txtDc_Step1_SetW.TextChanged += DcInputs_Changed;
            txtDc_Step1_Ramp.TextChanged += DcInputs_Changed;

            // Step2 numeric
            numDc_Step2_Dwell.ValueChanged -= DcInputs_Changed;
            numDc_Step2_SetW.ValueChanged -= DcInputs_Changed;
            numDc_Step2_Ramp.ValueChanged -= DcInputs_Changed;

            numDc_Step2_Dwell.ValueChanged += DcInputs_Changed;
            numDc_Step2_SetW.ValueChanged += DcInputs_Changed;
            numDc_Step2_Ramp.ValueChanged += DcInputs_Changed;

            // ReadPower değişirse (poll güncelliyor olabilir) Step2 dynamic kalan süreyi güncellemek istersen:
            txtDC_ReadPower.TextChanged -= DcInputs_Changed;
            txtDC_ReadPower.TextChanged += DcInputs_Changed;

            UpdateDcTimePreviews();
        }

        private void DcInputs_Changed(object? sender, EventArgs e)
        {
            UpdateDcTimePreviews();
        }

        // =====================================================
        // Confirmed state hook (optional)
        // =====================================================
        private void OnDcStateConfirmed(bool state)
        {
            if (_dcConfirmedState == state) return;
            _dcConfirmedState = state;

            ApplyDcButtonUi(state);

            if (state) StartDcSequenceIfNotRunning();
            else CancelDcSequence();
        }

        // =====================================================
        // Time Preview (no need to press START)
        // - If not running: show Step1 & Step2 planned totals
        // - If running Step1: Step2 planned preview updates
        // - If running Step2: dynamic remaining updates (SetW/Ramp/Dwell + ReadPower)
        // =====================================================
        private void UpdateDcTimePreviews()
        {
            if (!_dcSeqRunning)
            {
                txtDC_Step1_rTime.Text = DcFormatHms(DcComputeStep1Duration());
                txtDC_Step2_rTime.Text = DcFormatHms(DcComputeStep2Duration_Planned());
                return;
            }

            if (_dcPhase == DcRunPhase.Step1)
            {
                txtDC_Step2_rTime.Text = DcFormatHms(DcComputeStep2Duration_Planned());
                return;
            }

            if (_dcPhase == DcRunPhase.Step2)
            {
                DcRecalcStep2TotalFromInputs();
            }
        }

        // =====================================================
        // Dynamic Step2 total update:
        // while running Step2, formula uses CURRENT POWER:
        //   dwellMin*60 + max(0, SetW2 - ReadPower) / Ramp2
        // =====================================================
        private void DcRecalcStep2TotalFromInputs()
        {
            if (!_dcSeqRunning) return;
            if (_dcPhase != DcRunPhase.Step2) return;

            var newTotal = DcComputeStep2Duration_FromCurrentPower();

            lock (_dcStep2Sync)
                _dcStep2Total = newTotal;

            var rem = DcGetStep2Remaining();
            txtDC_Step2_rTime.Text = DcFormatHms(rem);
        }

        private TimeSpan DcGetStep2Remaining()
        {
            lock (_dcStep2Sync)
            {
                var rem = _dcStep2Total - (DateTime.UtcNow - _dcStep2StartedAtUtc);
                return rem < TimeSpan.Zero ? TimeSpan.Zero : rem;
            }
        }

        // =====================================================
        // Sequence: Step1 -> Step2(dynamic) -> Refresh + Confirm DC OFF
        // =====================================================
        private void StartDcSequenceIfNotRunning()
        {
            if (_dcSeqRunning) return;

            _dcSeqRunning = true;
            _dcSeqCts = new CancellationTokenSource();
            var ct = _dcSeqCts.Token;

            // DC ON olduktan sonra Step1 inputları kilitli
            SetDCStep1InputsEnabled(false);

            _ = Task.Run(async () =>
            {
                try
                {
                    // ---------- STEP 1 ----------
                    _dcPhase = DcRunPhase.Step1;
                    await DcRunCountdownAsync_SafeUI(txtDC_Step1_rTime, DcComputeStep1Duration(), ct);

                    // ---------- STEP 2 (dynamic) ----------
                    _dcPhase = DcRunPhase.Step2;
                    lock (_dcStep2Sync)
                    {
                        _dcStep2StartedAtUtc = DateTime.UtcNow;

                        // Step2 başlarken planned baz alınır (RF1 gibi)
                        _dcStep2Total = DcComputeStep2Duration_Planned();
                    }

                    await DcRunStep2CountdownDynamicAsync_SafeUI(ct);

                    // Step2 complete; status reconciliation is handled by 1Hz poller.
                }
                catch (OperationCanceledException) { }
                catch
                {
                    // istersen log
                }
                finally
                {
                    _dcPhase = DcRunPhase.None;
                    _dcSeqRunning = false;

                    UI(UpdateDcTimePreviews);
                }
            }, ct);
        }

        private async Task DcRunCountdownAsync_SafeUI(TextBox tb, TimeSpan dur, CancellationToken ct)
        {
            var rem = dur;

            UI(() =>
            {
                if (!tb.IsDisposed) tb.Text = DcFormatHms(rem);
            });

            while (rem > TimeSpan.Zero)
            {
                await Task.Delay(1000, ct);

                rem -= TimeSpan.FromSeconds(1);
                if (rem < TimeSpan.Zero) rem = TimeSpan.Zero;

                UI(() =>
                {
                    if (!tb.IsDisposed) tb.Text = DcFormatHms(rem);
                });
            }
        }

        private async Task DcRunStep2CountdownDynamicAsync_SafeUI(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var rem = DcGetStep2Remaining();

                UI(() =>
                {
                    if (!txtDC_Step2_rTime.IsDisposed)
                        txtDC_Step2_rTime.Text = DcFormatHms(rem);
                });

                if (rem <= TimeSpan.Zero) break;

                await Task.Delay(1000, ct);
                // remaining = total - elapsed; user total değiştirince otomatik yansır
            }
        }

        private void CancelDcSequence()
        {
            try { _dcSeqCts?.Cancel(); } catch { }
            _dcSeqCts = null;

            _dcSeqRunning = false;
            _dcPhase = DcRunPhase.None;

            // UI restore
            SetDCStep1InputsEnabled(true);

            // Cancel sonrası preview'a dön
            UpdateDcTimePreviews();
        }

        // =====================================================
        // DC Step1 input enable helper (SENDE YOKTU -> EKLENDİ)
        // ApplyDcButtonUi bunu çağırıyor.
        // =====================================================
        private void SetDCStep1InputsEnabled(bool enabled)
        {
            txtDc_Step1_SetW.Enabled = enabled;
            txtDc_Step1_Ramp.Enabled = enabled;
            txtDc_Step1_Dwell.Enabled = enabled;
        }

        // =====================================================
        // IO helper (DC) - separate names to avoid clashes
        // =====================================================
        private async Task<bool> DcWaitRegisterEqualsAsync(
            ushort addr, short expected, int timeoutMs, int pollMs, CancellationToken ct)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var regs = await _modbus!.ReadHoldingRegistersAsync(addr, 1);
                    short v = unchecked((short)regs[0]);
                    if (v == expected) return true;
                }
                catch { }

                await Task.Delay(pollMs, ct);
            }

            return false;
        }

        // =====================================================
        // Parse helpers (DC)
        // =====================================================
        private static int DcReadInt(TextBox tb, int fallback = 0)
        {
            if (tb == null) return fallback;
            return int.TryParse(tb.Text?.Trim(), out var v) ? v : fallback;
        }

        private static int DcReadInt(NumericUpDown nud, int fallback = 0)
        {
            if (nud == null) return fallback;
            try
            {
                return (int)Math.Round(nud.Value, MidpointRounding.AwayFromZero);
            }
            catch
            {
                return fallback;
            }
        }

        private static string DcFormatHms(TimeSpan t)
        {
            if (t < TimeSpan.Zero) t = TimeSpan.Zero;
            int h = (int)Math.Floor(t.TotalHours);
            return $"{h:00}:{t.Minutes:00}:{t.Seconds:00}";
        }

        // =====================================================
        // DURATION CALCS (DC)
        // =====================================================
        private TimeSpan DcComputeStep1Duration()
        {
            int dwellMin = DcReadInt(txtDc_Step1_Dwell, 0);
            int setW = DcReadInt(txtDc_Step1_SetW, 0);
            int ramp = Math.Max(1, DcReadInt(txtDc_Step1_Ramp, 1));

            double totalSeconds = dwellMin * 60.0 + (double)setW / ramp;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        // Planned (preview / Step2 start baseline): (SetW2 - SetW1) / Ramp2
        private TimeSpan DcComputeStep2Duration_Planned()
        {
            int dwellMin = DcReadInt(numDc_Step2_Dwell, 0);
            int setW1 = DcReadInt(txtDc_Step1_SetW, 0);
            int setW2 = DcReadInt(numDc_Step2_SetW, 0);
            int ramp2 = Math.Max(1, DcReadInt(numDc_Step2_Ramp, 1));

            double deltaW = setW2 - setW1;
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        // While Step2 running: (SetW2 - ReadPower) / Ramp2
        private TimeSpan DcComputeStep2Duration_FromCurrentPower()
        {
            int dwellMin = DcReadInt(numDc_Step2_Dwell, 0);
            int setW2 = DcReadInt(numDc_Step2_SetW, 0);
            int ramp2 = Math.Max(1, DcReadInt(numDc_Step2_Ramp, 1));

            int readP = GetCurrentDcReadPower() ?? setW2; // okunamazsa delta=0 davranışı
            double deltaW = setW2 - readP;

            // txtDC_ReadPower ile SetW eşitse veya readP > setW ise ekleme yok
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        private int? GetCurrentDcReadPower()
        {
            var s = txtDC_ReadPower?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;

            // "--" / invalid -> null
            return int.TryParse(s, out var v) ? v : (int?)null;
        }
    }
}
