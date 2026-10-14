using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SputterNewApp.Core.Helpers;
using SputterNewApp.Core.Services;
using SputterNewApp.Services;

namespace SputterNewApp.Views
{
    public partial class ManualReadWindow : Window
    {
        private readonly SputterServiceCoordinator? _coordinator;

        public ManualReadWindow()
        {
            InitializeComponent();
        }

        public ManualReadWindow(SputterServiceCoordinator coordinator) : this()
        {
            _coordinator = coordinator;
            if (_coordinator?.Modbus.IsConnected == true)
            {
                txtConnStatus.Text = "Modbus bağlantısı aktif.";
            }
            else
            {
                txtConnStatus.Text = "Modbus bağlı değil.";
            }
        }

        private async void OnReadClicked(object sender, RoutedEventArgs e)
        {
            if (_coordinator?.Modbus.IsConnected != true)
            {
                MessageBox.Show("Modbus bağlantısı yok. Lütfen önce bağlanın.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ushort.TryParse(txtStartAddress.Text.Trim(), out var startAddr))
            {
                MessageBox.Show("Geçersiz başlangıç adresi.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ushort.TryParse(txtCount.Text.Trim(), out var count) || count == 0 || count > 125)
            {
                MessageBox.Show("Adet 1 ile 125 arasında olmalıdır.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnRead.IsEnabled = false;
            txtResult.Text = "Okunuyor...";

            try
            {
                var regs = await _coordinator.Modbus.ReadHoldingRegistersAsync(startAddr, count);
                var sb = new StringBuilder();
                sb.AppendLine($"Adres {startAddr} - {startAddr + count - 1} ({count} register okundu):\n");

                var dataType = (cmbDataType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "UInt16";

                for (int i = 0; i < regs.Length; i++)
                {
                    ushort addr = (ushort)(startAddr + i);
                    ushort val = regs[i];

                    if (dataType.Contains("Float32") && i + 1 < regs.Length)
                    {
                        float fVal = ModbusTypeExtensions.ToFloat32_CDAB(regs[i], regs[i + 1]);
                        sb.AppendLine($"D{addr}-D{addr + 1} (Float32): {fVal:0.######}  [Word0: 0x{regs[i]:X4}, Word1: 0x{regs[i + 1]:X4}]");
                        i++; // 2 register atla
                    }
                    else if (dataType == "Int16")
                    {
                        short sVal = unchecked((short)val);
                        sb.AppendLine($"D{addr}: {sVal}  (0x{val:X4})");
                    }
                    else
                    {
                        sb.AppendLine($"D{addr}: {val}  (0x{val:X4})");
                    }
                }

                txtResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Okuma hatası:\n{ex.Message}";
            }
            finally
            {
                btnRead.IsEnabled = true;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();
    }
}
