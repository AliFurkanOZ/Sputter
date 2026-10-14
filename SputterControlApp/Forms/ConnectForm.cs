using SputterControlApp.Core.Services;
using SputterControlApp.Core.Models;   
using System.Net;
using System.Net.Sockets;

namespace SputterControlApp
{
    public partial class ConnectForm : Form
    {
        private readonly IModbusClientService _modbus;

        public ConnectForm()
        {
            InitializeComponent();

            // Şimdilik basit: servis instance'ını burada oluştur.
            _modbus = new NModbusTcpClientService();

            // Form ilk görünürken çalışacak
            this.Shown += ConnectForm_Shown;

            // Kapanırken ayarları kaydet
            this.FormClosing += ConnectForm_FormClosing;

            UpdateStatus(false, "Disconnected");

            // UX: Port textbox'a sadece sayı girilsin (basit)
            txtPort.KeyPress += (_, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
        }

        private void ConnectForm_Shown(object? sender, EventArgs e)
        {
            // 1) Son kayıtlı IP/Port’u textboxlara koy
            txtIp.Text = Properties.Settings.Default.LastIp ?? "";
            txtPort.Text = Properties.Settings.Default.LastPort > 0
                ? Properties.Settings.Default.LastPort.ToString()
                : "502";

            // 2) İmleci Registration Number alanına al
            txtRegistrationNo.Focus();

            // Otomatik bağlantı kapatıldı: kullanıcı Connect tuşu ile başlatır.
        }
        private void ConnectForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastIp = txtIp.Text?.Trim() ?? "";
            if (int.TryParse(txtPort.Text, out var port) && port > 0 && port <= 65535)
                Properties.Settings.Default.LastPort = port;

            Properties.Settings.Default.Save();
        }



        private async void btnConnect_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = SystemColors.Highlight;
            UpdateStatus(false, "Connecting...");

            Cursor = Cursors.WaitCursor;
            btnConnect.Enabled = false;
            btnExit.Enabled = false;
            MainForm? main = null;
            bool mainShown = false;
            bool connectionEstablished = false;

            try
            {
                var settings = BuildSettingsFromUi();
                var registrationNo = BuildRegistrationNoFromUi();

                await _modbus.ConnectAsync(settings);
                connectionEstablished = true;

                UpdateStatus(true, $"Connected: {settings.IpAddress}:{settings.Port} (UnitId={settings.UnitId})");

                main = new MainForm(_modbus, registrationNo)
                {
                    Owner = this // Böylece MainForm kapandığında ConnectForm otomatik olarak Show olur.
                };

                using var splash = new SplashForm();

                Hide();
                splash.Show();
                splash.BringToFront();
                splash.Refresh();

                // Splash önce çizilsin, ardından MainForm arka planda ısınsın.
                await Task.Yield();

                var warmupTask = main.WarmupAsync();
                await Task.WhenAll(Task.Delay(3000), warmupTask);

                splash.Close();
                main.Show();
                mainShown = true;
            }

            catch (TimeoutException)
            {
                UpdateStatus(false, "Disconnected: Timeout (cihaz yanıt vermedi).");
            }
            catch (SocketException)
            {
                UpdateStatus(false, "Disconnected: Ağ hatası (IP/Port kontrol et).");
            }
            catch (Exception ex)
            {
                if (connectionEstablished && !mainShown)
                {
                    try { _modbus.Disconnect(); } catch { /* ignore */ }
                }

                if (!mainShown && main is not null && !main.IsDisposed)
                {
                    try { main.Dispose(); } catch { /* ignore */ }
                }

                UpdateStatus(false, $"Disconnected: {ex.Message}");
            }

            finally
            {
                if (!mainShown && !IsDisposed && !Visible)
                {
                    Show();
                    BringToFront();
                    Activate();
                }

                // Eğer başarılı bağlandıysak zaten Hide ettik, ama butonlar şimdilik sorun değil.
                Cursor = Cursors.Default;
                btnConnect.Enabled = true;
                btnExit.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try { _modbus.Dispose(); } catch { /* ignore */ }
            Close();
        }


        private string BuildRegistrationNoFromUi()
        {
            var registrationNo = txtRegistrationNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(registrationNo))
                throw new ArgumentException("Registration Number boş bırakılamaz.");

            return registrationNo;
        }

        private ConnectionSettings BuildSettingsFromUi()
        {
            var ipText = txtIp.Text.Trim();
            if (!IPAddress.TryParse(ipText, out _))
                throw new ArgumentException("IP Address geçersiz.");

            if (!int.TryParse(txtPort.Text.Trim(), out var port) || port < 1 || port > 65535)
                throw new ArgumentException("Port geçersiz (1-65535).");

            // Şimdilik UnitId UI’da yok → default 1
            return new ConnectionSettings
            {
                IpAddress = ipText,
                Port = port,
                UnitId = 1
            };
        }

        private void UpdateStatus(bool connected, string text)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = connected ? Color.Green : SystemColors.Highlight;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Eğer connect ekranda kapatılırsa bağlantıyı da kapat
            try { _modbus.Dispose(); } catch { /* ignore */ }
            base.OnFormClosed(e);
        }

        internal void ReturnFromDisconnect()
        {
            UpdateStatus(false, "Disconnected");
            btnConnect.Enabled = true;
            btnExit.Enabled = true;

            this.Show();
            this.BringToFront();
            this.Activate();
        }
    }
}
