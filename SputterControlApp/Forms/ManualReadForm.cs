using SputterControlApp.Core.Services;
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class ManualReadForm : Form
    {
        private readonly IModbusClientService _modbus;

        public ManualReadForm(IModbusClientService modbus)
        {
            InitializeComponent();

            _modbus = modbus ?? throw new ArgumentNullException(nameof(modbus));

            // Defaults (UX)
            if (cmbManualType.Items.Count > 0 && cmbManualType.SelectedIndex < 0) cmbManualType.SelectedIndex = 0;          // Raw
            if (cmbManualEndian.Items.Count > 0 && cmbManualEndian.SelectedIndex < 0) cmbManualEndian.SelectedIndex = 0;    // Big Endian
            if (cmbManualWordOrder.Items.Count > 0 && cmbManualWordOrder.SelectedIndex < 0) cmbManualWordOrder.SelectedIndex = 0; // High-Low (ABCD)
            if (numManualCount.Value <= 0) numManualCount.Value = 1;
                        
        }


        private async void btnManualRead_Click(object? sender, EventArgs e)
        {
            lblManualError.Visible = false;
            lblManualError.Text = "";

            if (!_modbus.IsConnected)
            {
                ShowError("Not connected.");
                return;
            }

            ushort startAddr = (ushort)numManualAddr.Value;
            ushort count = (ushort)numManualCount.Value;

            string type = cmbManualType.SelectedItem?.ToString() ?? "Raw (16-bit regs)";
            string endian = cmbManualEndian.SelectedItem?.ToString() ?? "Big Endian";
            string wordOrder = cmbManualWordOrder.SelectedItem?.ToString() ?? "High-Low (ABCD)";

            // Type’a göre minimum register sayısını garanti et
            ushort minRegsNeeded = GetMinRegsNeeded(type);
            if (count < minRegsNeeded)
            {
                count = minRegsNeeded;
                numManualCount.Value = count; // kullanıcıya da yansısın
            }

            btnManualRead.Enabled = false;

            try
            {
                // Basit: 2-3 sn içinde cevap gelmezse UI kitlenmesin
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

                ushort[] regs = await _modbus.ReadHoldingRegistersAsync(startAddr, count, cts.Token);

                txtManualResult.Text = BuildResultText(startAddr, regs, type, endian, wordOrder);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                btnManualRead.Enabled = true;
            }
        }

        private static ushort GetMinRegsNeeded(string type)
        {
            return type switch
            {
                "INT32" => 2,
                "UINT32" => 2,
                "FLOAT32" => 2,
                _ => 1
            };
        }

        private void ShowError(string msg)
        {
            lblManualError.Text = msg;
            lblManualError.Visible = true;
        }

        private static string BuildResultText(ushort startAddr, ushort[] regs, string type, string endian, string wordOrder)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine($"Address: {startAddr}   Quantity: {regs.Length}");
            sb.AppendLine($"Type: {type}   Endian: {endian}   Word order: {wordOrder}");
            sb.AppendLine(new string('-', 60));

            // Raw dump
            sb.AppendLine("Raw registers:");
            for (int i = 0; i < regs.Length; i++)
            {
                ushort addr = (ushort)(startAddr + i);
                ushort r = regs[i];
                sb.AppendLine($"  [{addr}] = 0x{r:X4}   (dec {r})");
            }

            sb.AppendLine(new string('-', 60));

            // Decode
            try
            {
                sb.AppendLine("Decoded:");
                sb.AppendLine(DecodeValue(regs, type, endian, wordOrder));
            }
            catch (Exception ex)
            {
                sb.AppendLine($"(Decode error) {ex.Message}");
            }

            return sb.ToString();
        }

        private static string DecodeValue(ushort[] regs, string type, string endian, string wordOrder)
        {
            switch (type)
            {
                case "Raw (16-bit regs)":
                    return "Raw mode selected (no decode).";

                case "INT16":
                    return DecodeInt16(regs, endian);

                case "UINT16":
                    return DecodeUInt16(regs, endian);

                case "INT32":
                    return DecodeInt32(regs, endian, wordOrder);

                case "UINT32":
                    return DecodeUInt32(regs, endian, wordOrder);

                case "FLOAT32":
                    return DecodeFloat32(regs, endian, wordOrder);

                default:
                    return $"Unknown type: {type}";
            }
        }

        // 16-bit: regs[0] içindeki byte order düzeltmesi
        private static string DecodeInt16(ushort[] regs, string endian)
        {
            ushort r = regs[0];
            ushort corrected = CorrectWordEndian(r, endian);
            short v = unchecked((short)corrected);
            return $"INT16 = {v}";
        }

        private static string DecodeUInt16(ushort[] regs, string endian)
        {
            ushort r = regs[0];
            ushort corrected = CorrectWordEndian(r, endian);
            return $"UINT16 = {corrected}";
        }

        // 32-bit: önce 2 register’ı canonical ABCD’ye çevir, sonra sayıya dönüştür
        private static string DecodeInt32(ushort[] regs, string endian, string wordOrder)
        {
            var (A, B, C, D) = ToCanonicalABCD(regs, endian, wordOrder);
            uint u = ((uint)A << 24) | ((uint)B << 16) | ((uint)C << 8) | D;
            int v = unchecked((int)u);
            return $"INT32 = {v}";
        }

        private static string DecodeUInt32(ushort[] regs, string endian, string wordOrder)
        {
            var (A, B, C, D) = ToCanonicalABCD(regs, endian, wordOrder);
            uint u = ((uint)A << 24) | ((uint)B << 16) | ((uint)C << 8) | D;
            return $"UINT32 = {u}";
        }

        private static string DecodeFloat32(ushort[] regs, string endian, string wordOrder)
        {
            var (A, B, C, D) = ToCanonicalABCD(regs, endian, wordOrder);

            // BitConverter little-endian bekler → ABCD (big-endian) ise ters çevirip veriyoruz
            byte[] le = new byte[] { D, C, B, A };
            float f = BitConverter.ToSingle(le, 0);

            return $"FLOAT32 = {f.ToString("G9", CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Device’ten okunan 2x16-bit register’ı, seçilen Endian/WordOrder'a göre
        /// canonical 32-bit byte dizilimine (A,B,C,D = MSB..LSB) çevirir.
        /// 
        /// - Endian: register içindeki byte sırası (Big: Hi-Lo, Little: Lo-Hi)
        /// - Word order: 2 register sırası (High-Low: [reg0][reg1], Low-High: [reg1][reg0])
        /// </summary>
        private static (byte A, byte B, byte C, byte D) ToCanonicalABCD(ushort[] regs, string endian, string wordOrder)
        {
            if (regs.Length < 2)
                throw new InvalidOperationException("Need at least 2 registers for 32-bit types.");

            ushort w0 = regs[0];
            ushort w1 = regs[1];

            // Word order düzelt
            bool lowHigh = wordOrder.StartsWith("Low-High", StringComparison.OrdinalIgnoreCase);
            if (lowHigh)
            {
                (w0, w1) = (w1, w0);
            }

            // Word endian düzeltmesi (her word içindeki byte swap)
            w0 = CorrectWordEndian(w0, endian);
            w1 = CorrectWordEndian(w1, endian);

            // Canonical ABCD (MSB..LSB)
            byte A = (byte)(w0 >> 8);
            byte B = (byte)(w0 & 0xFF);
            byte C = (byte)(w1 >> 8);
            byte D = (byte)(w1 & 0xFF);

            return (A, B, C, D);
        }

        private static ushort CorrectWordEndian(ushort word, string endian)
        {
            bool little = endian.StartsWith("Little", StringComparison.OrdinalIgnoreCase);
            if (!little) return word;

            // byte swap: 0xAABB -> 0xBBAA
            return (ushort)(((word & 0x00FF) << 8) | ((word & 0xFF00) >> 8));
        }
    }
}
