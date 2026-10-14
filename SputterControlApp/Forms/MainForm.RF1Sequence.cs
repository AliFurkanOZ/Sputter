using SputterControlApp.Core.Registers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
         // =====================================================
        // RF1 CONTROL - Clean architecture + Preview + Dynamic Step2 (FIXED UI THREAD)
        // =====================================================

        // ------------------------
        // RF1 state (confirmed)
        // ------------------------
        private bool? _rf1ConfirmedState = null;
        private CancellationTokenSource? _rf1ConfirmCts;

        // ------------------------
        // RF1 sequence
        // ------------------------
        private CancellationTokenSource? _rf1SeqCts;
        private bool _rf1SeqRunning = false;

        // ------------------------
        // RF1 phases
        // ------------------------
        private enum Rf1RunPhase { None, Step1, Step2 }
        private volatile Rf1RunPhase _rf1Phase = Rf1RunPhase.None;

        // ------------------------
        // Step2 dynamic timing
        // ------------------------
        private readonly object _rf1Step2Sync = new();
        private DateTime _rf1Step2StartedAtUtc;
        private TimeSpan _rf1Step2Total;

        private DeviceToggleSupervisor? _rf1Sup;
        private void SetupRf1Supervisor()
        {
            _rf1Sup?.Dispose();

            _rf1Sup = new DeviceToggleSupervisor(
                uiOwner: this,
                button: btnControlRf1,
                readStatusAsync: async (ct) =>
                {
                    var regs = await _modbus!.ReadHoldingRegistersAsync((ushort)RegisterId.RF1_Status, 1);
                    return unchecked((short)regs[0]);
                },
                offValue: 0,
                onValue: 1,
                applyUi: ApplyRf1ButtonUi,
                onConfirmedOn: _ => StartRf1SequenceIfNotRunning(),
                onConfirmedOff: _ => CancelRf1Sequence(),
                triggerRefresh: () => btnRefresh.PerformClick()
            );

            // süre preview güncellemeleri için event wiring
            // supervisor UI güncellemeleri sonrası tetiklenecek
            WireRf1TimePreviewHandlers();
        }

        // =====================================================
        // 0) Wire handlers (CALL ONCE AFTER UI INIT)
        // =====================================================
        private void WireRf1TimePreviewHandlers()
        {
            // önce temizle (double subscribe olmasın)
            txtRf1_Step1_Dwell.TextChanged -= Rf1Inputs_Changed;
            txtRf1_Step1_SetW.TextChanged -= Rf1Inputs_Changed;
            txtRf1_Step1_Ramp.TextChanged -= Rf1Inputs_Changed;

            numRf1_Step2_Dwell.ValueChanged -= Rf1Inputs_Changed;
            numRf1_Step2_SetW.ValueChanged -= Rf1Inputs_Changed;
            numRf1_Step2_Ramp.ValueChanged -= Rf1Inputs_Changed;

            txtRf1_Step1_Dwell.TextChanged += Rf1Inputs_Changed;
            txtRf1_Step1_SetW.TextChanged += Rf1Inputs_Changed;
            txtRf1_Step1_Ramp.TextChanged += Rf1Inputs_Changed;

            numRf1_Step2_Dwell.ValueChanged += Rf1Inputs_Changed;
            numRf1_Step2_SetW.ValueChanged += Rf1Inputs_Changed;
            numRf1_Step2_Ramp.ValueChanged += Rf1Inputs_Changed;

            UpdateRf1TimePreviews();
        }

        private void Rf1Inputs_Changed(object? sender, EventArgs e)
        {
            UpdateRf1TimePreviews();
        }

        // =====================================================
        // 1) UI ONLY
        // =====================================================
        private void ApplyRf1ButtonUi(bool? state)
        {
            if (state == true)
            {
                btnControlRf1.Text = "RF1 ON";
                btnControlRf1.BackColor = Color.ForestGreen;
                btnControlRf1.ForeColor = Color.White;
            }
            else if (state == false)
            {
                btnControlRf1.Text = "RF1 OFF";
                btnControlRf1.BackColor = Color.IndianRed;
                btnControlRf1.ForeColor = Color.White;
            }
            else
            {
                btnControlRf1.Text = "RF1 Status Pending...";
                btnControlRf1.BackColor = Color.SlateGray;
                btnControlRf1.ForeColor = Color.GhostWhite;
            }

            btnControlRf1.Tag = state;

            if (state == true) SetRf1Step1InputsEnabled(false);
            else if (state == false) SetRf1Step1InputsEnabled(true);

            UpdateTargetSelectionLocks();
        }

        // =====================================================
        // 2) CONFIRMED STATE HANDLER
        // =====================================================
        private void OnRf1StateConfirmed(bool state)
        {
            if (_rf1ConfirmedState == state) return;

            _rf1ConfirmedState = state;
            ApplyRf1ButtonUi(state);

            if (state) StartRf1SequenceIfNotRunning();
            else CancelRf1Sequence();
        }
        // =====================================================
        // 4) CONFIRM WATCHER
        // =====================================================
        private void UpdateRf1TimePreviews()
        {
            if (!_rf1SeqRunning)
            {
                txtRf1_Step1_rTime.Text = FormatHms(ComputeStep1Duration());
                txtRf1_Step2_rTime.Text = FormatHms(ComputeStep2Duration_Planned());
                return;
            }

            // Step1 sayarken Step2 preview güncellensin
            if (_rf1Phase == Rf1RunPhase.Step1)
            {
                txtRf1_Step2_rTime.Text = FormatHms(ComputeStep2Duration_Planned());
                return;
            }

            // Step2 sayarken SetW/Ramp/FwP kuralı geçerli
            if (_rf1Phase == Rf1RunPhase.Step2)
            {
                RecalcStep2TotalFromInputs();
            }
        }

        // =====================================================
        // 6) STEP2 DYNAMIC UPDATE
        // - Step2 running iken:
        //   dwell değişince de OK
        //   setW/ramp değişince: (setW - current FwP) / ramp
        // =====================================================
        private void RecalcStep2TotalFromInputs()
        {
            if (!_rf1SeqRunning) return;
            if (_rf1Phase != Rf1RunPhase.Step2) return;

            var newTotal = ComputeStep2Duration_FromCurrentPower();

            lock (_rf1Step2Sync)
                _rf1Step2Total = newTotal;

            var rem = GetStep2Remaining();
            txtRf1_Step2_rTime.Text = FormatHms(rem);
        }

        private TimeSpan GetStep2Remaining()
        {
            lock (_rf1Step2Sync)
            {
                var rem = _rf1Step2Total - (DateTime.UtcNow - _rf1Step2StartedAtUtc);
                return rem < TimeSpan.Zero ? TimeSpan.Zero : rem;
            }
        }

        // =====================================================
        // 7) SEQUENCE
        // =====================================================
        private void StartRf1SequenceIfNotRunning()
        {
            if (_rf1SeqRunning) return;

            _rf1SeqRunning = true;
            _rf1SeqCts = new CancellationTokenSource();
            var ct = _rf1SeqCts.Token;

            SetRf1Step1InputsEnabled(false);

            _ = Task.Run(async () =>
            {
                try
                {
                    // ---------- STEP 1 ----------
                    _rf1Phase = Rf1RunPhase.Step1;
                    await RunCountdownAsync_SafeUI(txtRf1_Step1_rTime, ComputeStep1Duration(), ct);

                    // ---------- STEP 2 ----------
                    _rf1Phase = Rf1RunPhase.Step2;
                    lock (_rf1Step2Sync)
                    {
                        _rf1Step2StartedAtUtc = DateTime.UtcNow;
                        _rf1Step2Total = ComputeStep2Duration_Planned(); // Step2 başlarken plan baz alınır
                    }

                    await RunStep2CountdownDynamicAsync_SafeUI(ct);

                    // Step2 complete; status reconciliation is handled by 1Hz poller.
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    // Debug istersen:
                    // UI(() => PostStatus($"RF1 seq error: {ex.Message}"));
                }
                finally
                {
                    _rf1Phase = Rf1RunPhase.None;
                    _rf1SeqRunning = false;
                    UI(UpdateRf1TimePreviews);
                }
            }, ct);
        }

        private async Task RunCountdownAsync_SafeUI(TextBox tb, TimeSpan dur, CancellationToken ct)
        {
            var rem = dur;

            // İlk bas
            UI(() =>
            {
                if (!tb.IsDisposed) tb.Text = FormatHms(rem);
            });

            while (rem > TimeSpan.Zero)
            {
                await Task.Delay(1000, ct);
                rem -= TimeSpan.FromSeconds(1);
                if (rem < TimeSpan.Zero) rem = TimeSpan.Zero;

                UI(() =>
                {
                    if (!tb.IsDisposed) tb.Text = FormatHms(rem);
                });
            }
        }

        private async Task RunStep2CountdownDynamicAsync_SafeUI(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var rem = GetStep2Remaining();

                UI(() =>
                {
                    if (!txtRf1_Step2_rTime.IsDisposed)
                        txtRf1_Step2_rTime.Text = FormatHms(rem);
                });

                if (rem <= TimeSpan.Zero) break;

                await Task.Delay(1000, ct);
                // remaining = total - elapsed; user total değiştirirse otomatik yansır
            }
        }

        // =====================================================
        // 8) OFF CONFIRM
        // =====================================================
        private void CancelRf1Sequence()
        {
            try { _rf1SeqCts?.Cancel(); } catch { }
            _rf1SeqCts = null;

            _rf1SeqRunning = false;
            _rf1Phase = Rf1RunPhase.None;

            SetRf1Step1InputsEnabled(true);
            UpdateRf1TimePreviews();
        }

        // =====================================================
        // HELPERS
        // =====================================================
        private void SetRf1Step1InputsEnabled(bool en)
        {
            txtRf1_Step1_SetW.Enabled = en;
            txtRf1_Step1_Ramp.Enabled = en;
            txtRf1_Step1_Dwell.Enabled = en;
        }

        private static int ReadInt(TextBox tb, int fb = 0)
            => int.TryParse(tb?.Text?.Trim(), out var v) ? v : fb;

        private static int ReadInt(NumericUpDown nud, int fb = 0)
            => nud == null ? fb : (int)Math.Round(nud.Value, MidpointRounding.AwayFromZero);

        private static string FormatHms(TimeSpan t)
        {
            if (t < TimeSpan.Zero) t = TimeSpan.Zero;
            int h = (int)Math.Floor(t.TotalHours);
            return $"{h:00}:{t.Minutes:00}:{t.Seconds:00}";
        }

        // =====================================================
        // DURATION CALCS
        // =====================================================
        private TimeSpan ComputeStep1Duration()
        {
            int dwellMin = ReadInt(txtRf1_Step1_Dwell, 0);
            int setW = ReadInt(txtRf1_Step1_SetW, 0);
            int ramp = Math.Max(1, ReadInt(txtRf1_Step1_Ramp, 1));

            double totalSeconds = dwellMin * 60.0 + (double)setW / ramp;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        private TimeSpan ComputeStep2Duration_Planned()
        {
            int dwellMin = ReadInt(numRf1_Step2_Dwell, 0);
            int setW1 = ReadInt(txtRf1_Step1_SetW, 0);
            int setW2 = ReadInt(numRf1_Step2_SetW, 0);
            int ramp2 = Math.Max(1, ReadInt(numRf1_Step2_Ramp, 1));

            double deltaW = setW2 - setW1;
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        private int? GetCurrentRf1FwP()
        {
            var s = lblRf1_FwP?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return int.TryParse(s, out var v) ? v : (int?)null;
        }

        // Step2 running: setW/ramp değişince (setW - currentFwP) / ramp
        private TimeSpan ComputeStep2Duration_FromCurrentPower()
        {
            int dwellMin = ReadInt(numRf1_Step2_Dwell, 0);
            int setW2 = ReadInt(numRf1_Step2_SetW, 0);
            int ramp2 = Math.Max(1, ReadInt(numRf1_Step2_Ramp, 1));

            int fwP = GetCurrentRf1FwP() ?? setW2; // okunamazsa ek süre yok gibi davran
            double deltaW = setW2 - fwP;
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
