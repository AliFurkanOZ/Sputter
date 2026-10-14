using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        // RF2 sequence state
        private CancellationTokenSource? _rf2SeqCts;
        private bool _rf2SeqRunning = false;

        private enum Rf2RunPhase { None, Step1, Step2 }
        private volatile Rf2RunPhase _rf2Phase = Rf2RunPhase.None;

        private readonly object _rf2Step2Sync = new();
        private DateTime _rf2Step2StartedAtUtc;
        private TimeSpan _rf2Step2Total;

        // RF2 preview wiring
        private void WireRf2TimePreviewHandlers()
        {
            txtRf2_Step1_Dwell.TextChanged -= Rf2Inputs_Changed;
            txtRf2_Step1_SetW.TextChanged -= Rf2Inputs_Changed;
            txtRf2_Step1_Ramp.TextChanged -= Rf2Inputs_Changed;

            numRf2_Step2_Dwell.TextChanged -= Rf2Inputs_Changed;
            numRf2_Step2_SetW.ValueChanged -= Rf2Inputs_Changed;
            numRf2_Step2_Ramp.TextChanged -= Rf2Inputs_Changed;

            lblRf2_FwP.TextChanged -= Rf2Inputs_Changed;

            txtRf2_Step1_Dwell.TextChanged += Rf2Inputs_Changed;
            txtRf2_Step1_SetW.TextChanged += Rf2Inputs_Changed;
            txtRf2_Step1_Ramp.TextChanged += Rf2Inputs_Changed;

            numRf2_Step2_Dwell.TextChanged += Rf2Inputs_Changed;
            numRf2_Step2_SetW.ValueChanged += Rf2Inputs_Changed;
            numRf2_Step2_Ramp.TextChanged += Rf2Inputs_Changed;

            lblRf2_FwP.TextChanged += Rf2Inputs_Changed;

            UpdateRf2TimePreviews();
        }

        private void Rf2Inputs_Changed(object? sender, EventArgs e)
        {
            UpdateRf2TimePreviews();
        }

        private void ApplyRf2ButtonUi(bool? state)
        {
            if (state == true)
            {
                btnControlRf2.Text = "RF2 ON";
                btnControlRf2.BackColor = System.Drawing.Color.ForestGreen;
                btnControlRf2.ForeColor = System.Drawing.Color.White;
            }
            else if (state == false)
            {
                btnControlRf2.Text = "RF2 OFF";
                btnControlRf2.BackColor = System.Drawing.Color.IndianRed;
                btnControlRf2.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                btnControlRf2.Text = "RF2 Status Pending...";
                btnControlRf2.BackColor = Color.SlateGray;
                btnControlRf2.ForeColor = Color.GhostWhite;
            }

            btnControlRf2.Tag = state;

            if (state == true) SetRf2Step1InputsEnabled(false);
            else if (state == false) SetRf2Step1InputsEnabled(true);

            UpdateTargetSelectionLocks();
        }

        private void UpdateRf2TimePreviews()
        {
            if (!_rf2SeqRunning)
            {
                txtRf2_Step1_rTime.Text = FormatHms(ComputeRf2Step1Duration());
                txtRf2_Step2_rTime.Text = FormatHms(ComputeRf2Step2Duration_Planned());
                return;
            }

            if (_rf2Phase == Rf2RunPhase.Step1)
            {
                txtRf2_Step2_rTime.Text = FormatHms(ComputeRf2Step2Duration_Planned());
                return;
            }

            if (_rf2Phase == Rf2RunPhase.Step2)
            {
                RecalcRf2Step2TotalFromInputs();
            }
        }

        private void RecalcRf2Step2TotalFromInputs()
        {
            if (!_rf2SeqRunning) return;
            if (_rf2Phase != Rf2RunPhase.Step2) return;

            var newTotal = ComputeRf2Step2Duration_FromCurrentPower();

            lock (_rf2Step2Sync)
                _rf2Step2Total = newTotal;

            var rem = GetRf2Step2Remaining();
            txtRf2_Step2_rTime.Text = FormatHms(rem);
        }

        private TimeSpan GetRf2Step2Remaining()
        {
            lock (_rf2Step2Sync)
            {
                var rem = _rf2Step2Total - (DateTime.UtcNow - _rf2Step2StartedAtUtc);
                return rem < TimeSpan.Zero ? TimeSpan.Zero : rem;
            }
        }

        private void StartRf2SequenceIfNotRunning()
        {
            if (_rf2SeqRunning) return;

            _rf2SeqRunning = true;
            _rf2SeqCts = new CancellationTokenSource();
            var ct = _rf2SeqCts.Token;

            SetRf2Step1InputsEnabled(false);

            _ = Task.Run(async () =>
            {
                try
                {
                    _rf2Phase = Rf2RunPhase.Step1;
                    await RunCountdownAsync_SafeUI(txtRf2_Step1_rTime, ComputeRf2Step1Duration(), ct);

                    _rf2Phase = Rf2RunPhase.Step2;
                    lock (_rf2Step2Sync)
                    {
                        _rf2Step2StartedAtUtc = DateTime.UtcNow;
                        _rf2Step2Total = ComputeRf2Step2Duration_Planned();
                    }

                    await RunRf2Step2CountdownDynamicAsync_SafeUI(ct);
                }
                catch (OperationCanceledException) { }
                catch
                {
                    // optional log
                }
                finally
                {
                    _rf2Phase = Rf2RunPhase.None;
                    _rf2SeqRunning = false;
                    UI(UpdateRf2TimePreviews);
                }
            }, ct);
        }

        private async Task RunRf2Step2CountdownDynamicAsync_SafeUI(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var rem = GetRf2Step2Remaining();

                UI(() =>
                {
                    if (!txtRf2_Step2_rTime.IsDisposed)
                        txtRf2_Step2_rTime.Text = FormatHms(rem);
                });

                if (rem <= TimeSpan.Zero) break;

                await Task.Delay(1000, ct);
            }
        }

        private void CancelRf2Sequence()
        {
            try { _rf2SeqCts?.Cancel(); } catch { }
            _rf2SeqCts = null;

            _rf2SeqRunning = false;
            _rf2Phase = Rf2RunPhase.None;

            SetRf2Step1InputsEnabled(true);
            UpdateRf2TimePreviews();
        }

        private void SetRf2Step1InputsEnabled(bool enabled)
        {
            txtRf2_Step1_SetW.Enabled = enabled;
            txtRf2_Step1_Ramp.Enabled = enabled;
            txtRf2_Step1_Dwell.Enabled = enabled;
        }

        private TimeSpan ComputeRf2Step1Duration()
        {
            int dwellMin = ReadInt(txtRf2_Step1_Dwell, 0);
            int setW = ReadInt(txtRf2_Step1_SetW, 0);
            int ramp = Math.Max(1, ReadInt(txtRf2_Step1_Ramp, 1));

            double totalSeconds = dwellMin * 60.0 + (double)setW / ramp;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        private TimeSpan ComputeRf2Step2Duration_Planned()
        {
            int dwellMin = ReadInt(numRf2_Step2_Dwell, 0);
            int setW1 = ReadInt(txtRf2_Step1_SetW, 0);
            int setW2 = ReadInt(numRf2_Step2_SetW, 0);
            int ramp2 = Math.Max(1, ReadInt(numRf2_Step2_Ramp, 1));

            double deltaW = setW2 - setW1;
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        private int? GetCurrentRf2FwP()
        {
            var s = lblRf2_FwP?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return int.TryParse(s, out var v) ? v : (int?)null;
        }

        private TimeSpan ComputeRf2Step2Duration_FromCurrentPower()
        {
            int dwellMin = ReadInt(numRf2_Step2_Dwell, 0);
            int setW2 = ReadInt(numRf2_Step2_SetW, 0);
            int ramp2 = Math.Max(1, ReadInt(numRf2_Step2_Ramp, 1));

            int fwP = GetCurrentRf2FwP() ?? setW2;
            double deltaW = setW2 - fwP;
            if (deltaW <= 0) deltaW = 0;

            double totalSeconds = dwellMin * 60.0 + deltaW / ramp2;
            if (totalSeconds < 0) totalSeconds = 0;

            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
