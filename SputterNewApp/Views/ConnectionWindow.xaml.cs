using System;
using System.Windows;
using SputterNewApp.Core.Models;
using SputterNewApp.Models;
using SputterNewApp.Services;

namespace SputterNewApp.Views
{
    public partial class ConnectionWindow : Window
    {
        private readonly SputterServiceCoordinator? _coordinator;

        public ConnectionWindow()
        {
            InitializeComponent();
        }

        public ConnectionWindow(SputterServiceCoordinator coordinator) : this()
        {
            _coordinator = coordinator;

            // En son girilen/kaydedilen IP ve Port ayarlarını alanlara doldur
            PopulateFields();

            if (_coordinator?.Modbus.IsConnected == true)
            {
                txtStatus.Text = $"Bağlı: {_coordinator.Modbus.CurrentSettings?.IpAddress}:{_coordinator.Modbus.CurrentSettings?.Port}";
                btnConnect.Content = "Yeniden Bağlan";
            }

            UpdateQcmStatusDisplay();
        }

        private async void PopulateFields()
        {
            var settings = _coordinator?.LastSettings ?? await AppDataManager.LoadConnectionSettingsAsync();
            if (settings != null)
            {
                txtIpAddress.Text = settings.IpAddress;
                txtPort.Text = settings.Port.ToString();
                txtRegistrationNo.Text = settings.RegistrationNo;
            }
        }

        private async void OnConnectClicked(object sender, RoutedEventArgs e)
        {
            if (_coordinator == null)
            {
                Close();
                return;
            }

            var ip = txtIpAddress.Text.Trim();
            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("IP adresi boş olamaz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtPort.Text.Trim(), out var port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Geçersiz port numarası.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var regNo = txtRegistrationNo.Text.Trim();

            btnConnect.IsEnabled = false;
            txtStatus.Text = "Bağlanıyor...";

            try
            {
                await _coordinator.ConnectAsync(ip, port, regNo);
                txtStatus.Text = $"Bağlandı: {ip}:{port}";
                MessageBox.Show("PLC bağlantısı başarıyla kuruldu.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Hata: {ex.Message}";
                MessageBox.Show($"Bağlantı kurulamadı:\n{ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnConnect.IsEnabled = true;
            }
        }

        private async void OnScanQcmClicked(object sender, RoutedEventArgs e)
        {
            if (_coordinator == null) return;
            btnScanQcm.IsEnabled = false;
            txtQcmScanResult.Text = "Portlar taranıyor...";
            try
            {
                await _coordinator.AutoConnectStm2BySerialAsync();
                UpdateQcmStatusDisplay();
                txtQcmScanResult.Text = "Tarama tamamlandı.";
            }
            catch (Exception ex)
            {
                txtQcmScanResult.Text = $"Hata: {ex.Message}";
            }
            finally
            {
                btnScanQcm.IsEnabled = true;
            }
        }

        private void UpdateQcmStatusDisplay()
        {
            if (_coordinator == null) return;

            var dc = _coordinator.Preview.GetSensorBySource("DC");
            var rf1 = _coordinator.Preview.GetSensorBySource("RF1");
            var rf2 = _coordinator.Preview.GetSensorBySource("RF2");

            UpdateSingleQcmDisplay(dc, txtQcmDcPort, txtQcmDcStatus);
            UpdateSingleQcmDisplay(rf1, txtQcmRf1Port, txtQcmRf1Status);
            UpdateSingleQcmDisplay(rf2, txtQcmRf2Port, txtQcmRf2Status);
        }

        private void UpdateSingleQcmDisplay(SensorRow? row, System.Windows.Controls.TextBlock txtPort, System.Windows.Controls.TextBlock txtStatus)
        {
            if (row != null && row.IsConnected)
            {
                txtPort.Text = string.IsNullOrEmpty(row.Port) ? "Bağlı" : row.Port;
                txtStatus.Text = "Bağlı";
                txtStatus.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                txtPort.Text = !string.IsNullOrEmpty(row?.Port) ? row.Port : "—";
                txtStatus.Text = "Bağlı değil";
                txtStatus.Foreground = (System.Windows.Media.Brush?)TryFindResource("TopBarMutedBrush") ?? System.Windows.Media.Brushes.Gray;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();
    }
}
