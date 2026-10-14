using SputterControlApp.Core.Registers;
using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using static SputterControlApp.Services.SetableToolsService;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        // -----------------------------
        // Status + Exception
        // -----------------------------
        private void PostStatus(string message)
        {
            // ManualRead açıkken MainForm SUSAR
            if (_manualReadForm != null && !_manualReadForm.IsDisposed)
                return;

            tsStatus.Text = message;
        }

        private void PostQcmStatus(string message)
        {
            if (_manualReadForm != null && !_manualReadForm.IsDisposed)
                return;

            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => tsStatusQcm.Text = message));
                return;
            }

            tsStatusQcm.Text = message;
        }

        private void HandleUiException(Exception ex)
        {
            // şimdilik sessiz (istersen log + MessageBox yaparız)
            // MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Warn(string msg)
        {
            // ✅ Snapshot okuması sırasında uyarı göstermeyelim
            if (_snapshotApplying) return;

            MessageBox.Show(msg, "Validation / Confirm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private static decimal ClampToNud(NumericUpDown nud, int value)
        {
            decimal v = value;
            if (v < nud.Minimum) return nud.Minimum;
            if (v > nud.Maximum) return nud.Maximum;
            return v;
        }

        private static decimal ClampToNud(NumericUpDown nud, float value)
        {
            decimal v = (decimal)value;
            if (v < nud.Minimum) return nud.Minimum;
            if (v > nud.Maximum) return nud.Maximum;
            return v;
        }

        private static void ApplyValveUi(Button btn, short valveValue)
        {
            switch (valveValue)
            {
                case 1:
                    btn.Text = "OPEN";
                    btn.BackColor = Color.ForestGreen;
                    btn.Tag = true;
                    break;

                case 2:
                    btn.Text = "CLOSE";
                    btn.BackColor = Color.IndianRed;
                    btn.Tag = false;
                    break;

                default:
                    btn.Text = $"ERR ({valveValue})";
                    btn.BackColor = Color.OrangeRed;
                    btn.Tag = null;
                    break;
            }
        }

        public enum PressureUnit
        {
            Torr,
            mBar,
            Pascal
        }

        private bool _torUiStyled;

        private void EnsureTorValueVisualStyle()
        {
            if (_torUiStyled) return;
            _torUiStyled = true;

            lblTorDeger.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTorDeger.BorderStyle = BorderStyle.FixedSingle;
            lblTorDeger.Padding = new Padding(6, 0, 6, 0);
        }

        private enum TorStatus { Unknown, Good, Ok, Warn, Bad }

        private static TorStatus GetTorStatusFromRawTorr(float rawTorr)
        {
            if (float.IsNaN(rawTorr) || float.IsInfinity(rawTorr) || rawTorr < 0)
                return TorStatus.Unknown;

            // ✅ Örnek eşikler (Torr bazında) — istersen sonra birlikte netleştiririz
            if (rawTorr <= 1e-5f) return TorStatus.Good;
            if (rawTorr <= 1e-3f) return TorStatus.Ok;
            if (rawTorr <= 1e-1f) return TorStatus.Warn;
            return TorStatus.Bad;
        }

        private void ApplyTorStatusHighlight(float rawTorr)
        {
            EnsureTorValueVisualStyle();

            var st = GetTorStatusFromRawTorr(rawTorr);
            switch (st)
            {
                case TorStatus.Good:
                    lblTorDeger.BackColor = Color.ForestGreen;
                    lblTorDeger.ForeColor = Color.White;
                    break;

                case TorStatus.Ok:
                    lblTorDeger.BackColor = Color.SeaGreen;
                    lblTorDeger.ForeColor = Color.White;
                    break;

                case TorStatus.Warn:
                    lblTorDeger.BackColor = Color.Goldenrod;
                    lblTorDeger.ForeColor = Color.Black;
                    break;

                case TorStatus.Bad:
                    lblTorDeger.BackColor = Color.IndianRed;
                    lblTorDeger.ForeColor = Color.White;
                    break;

                default:
                    lblTorDeger.BackColor = Color.DimGray;
                    lblTorDeger.ForeColor = Color.White;
                    break;
            }
        }

        private void UpdateTorUIFromRawTorr(float rawTorr)
        {
            var unit = GetSelectedTorUnit();
            var converted = ConvertFromTorr(rawTorr, unit);
            lblTorDeger.Text = FormatSci(converted);

            // ✅ yeni: renk/durum vurgusu ham Torr üzerinden (birimden bağımsız)
            ApplyTorStatusHighlight(rawTorr);
        }


        private static float ConvertFromTorr(float torr, PressureUnit unit)
        {
            return unit switch
            {
                PressureUnit.Torr => torr,
                PressureUnit.mBar => torr * 1.33322f,
                PressureUnit.Pascal => torr * 133.322f,
                _ => torr
            };
        }

        private PressureUnit GetSelectedTorUnit()
        {
            if (cmbTorUnit?.SelectedItem is PressureUnit u)
                return u;

            return PressureUnit.Torr;
        }

        private static string FormatSci(
            float value,
            int significantDigits = 4)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return "--";

            if (value == 0f)
                return "0";

            float abs = Math.Abs(value);

            int exponent = (int)Math.Floor(Math.Log10(abs));
            float mantissa = value / (float)Math.Pow(10, exponent);

            // Mantissa'yı 1–9.999 aralığına zorla
            if (Math.Abs(mantissa) < 1)
            {
                mantissa *= 10;
                exponent--;
            }

            string mantissaStr = mantissa.ToString(
                $"F{significantDigits - 1}",
                CultureInfo.InvariantCulture);

            // ✅ gerçek üs gösterimi
            string expSup = ToSuperscript(exponent);

            return $"{mantissaStr} × 10{expSup}";
        }


        private static string ToSuperscript(int number)
        {
            var map = new Dictionary<char, char>
            {
                ['0'] = '⁰',
                ['1'] = '¹',
                ['2'] = '²',
                ['3'] = '³',
                ['4'] = '⁴',
                ['5'] = '⁵',
                ['6'] = '⁶',
                ['7'] = '⁷',
                ['8'] = '⁸',
                ['9'] = '⁹',
                ['-'] = '⁻'
            };

            var s = number.ToString(CultureInfo.InvariantCulture);
            var sb = new StringBuilder(s.Length);

            foreach (char c in s)
            {
                if (map.TryGetValue(c, out char sup))
                    sb.Append(sup);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }



        private static void ApplyStartButtonUi(Button btn, bool isActive)
        {
            btn.Text = isActive ? "START" : "STOP";
            btn.BackColor = isActive ? Color.ForestGreen : Color.IndianRed;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
        }


        // =====================================================
        // UI invoke helper
        // =====================================================
        private void UI(Action a)
        {
            try
            {
                if (IsDisposed || !IsHandleCreated) return;

                if (InvokeRequired) BeginInvoke(a);
                else a();
            }
            catch (ObjectDisposedException)
            {
                // form kapanırken düşerse sessiz geç
            }
        }



    }
}
