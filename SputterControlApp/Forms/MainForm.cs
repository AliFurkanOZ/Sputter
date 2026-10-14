using SputterControlApp.Core.Models;
using SputterControlApp.Core.Services;
using SputterControlApp.Logging;
using SputterControlApp.Services;
using System.ComponentModel;
using System.IO.Ports;
using static SputterControlApp.Core.Models.AppDataManager;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private readonly IModbusClientService? _modbus;

        private bool _logoutRequested = false;
        private bool _appExitRequested = false;

        private ManualReadForm? _manualReadForm;
        private bool _manualReadOpen = false;

        // Tek polling mekanizması: scheduler
        private UiTaskScheduler? _uiScheduler;

        private SetableToolsService? _toolsSvc;

        private readonly ErrorProvider _ep = new ErrorProvider();

        private readonly LogService _logService = new LogService(capacity: 2000);
        private string _registrationNo = "";

        // Designer’ın kullandığı parametresiz constructor KALACAK
        public MainForm()
        {
            InitializeComponent();
            LoadSavedTargetSelections();
            UpdateTargetSelectionLocks();
        }

        // Bizim kullandığımız servis alan constructor
        public MainForm(IModbusClientService modbus, string? registrationNo = null) : this()
        {
            _modbus = modbus ?? throw new ArgumentNullException(nameof(modbus));
            _registrationNo = (registrationNo ?? "").Trim();

            _toolsSvc = new SetableToolsService(
                modbus,
                postStatus: PostStatus,
                onError: HandleUiException,
                publishLog: PublishLog
            );

            // UI başlangıç
            lblConnState.Text = "Connected";
            lblPollState.Text = "Polling: Stopped";
            PostStatus("Ready");
            PostQcmStatus("QCM: waiting for STM-2");

            // ✅ Önce wiring'ler (event'ler hazır olsun)
            WireManualStartButtons();
            WireToggleButtons();
            WireSetPointEnterHandlers();
            WireCommonStepHandlers();
            WireRf1StepHandlers();
            WireRf2StepHandlers();
            WireDcStepHandlers();
            WireTargetSelectionButtons();
            WireDeviceControlButtons();
            InitTorUnitCombo();
            InitSnapshotReadModule();
            InitEventsUi(_logService);
        }


        private void PublishLog(LogEvent ev)
        {
            var registrationNo = (ev.RegistrationNo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(registrationNo))
                registrationNo = _registrationNo;

            _logService.Publish(new LogEvent
            {
                Time = ev.Time,
                Level = ev.Level,
                Code = ev.Code,
                Message = ev.Message,
                Source = ev.Source,
                RegistrationNo = registrationNo
            });
        }

        // -----------------------------
        // Disconnect
        // -----------------------------
        private void miDisconnect_Click(object sender, EventArgs e)
        {
            if (_modbus == null)
                return;

            miDisconnect.Enabled = false;

            try
            {
                _logoutRequested = true;

                _uiScheduler?.Stop();

                lblPollState.Text = "Polling: Stopped";
                lblConnState.Text = "Disconnected";
                PostStatus("Disconnecting...");
                PostQcmStatus("QCM: disconnected");

                _modbus.Disconnect();

                //Eğer MainForm’un Ownerı ConnectForm ise (ConnectForm bunu set etmişti), o ConnectForm geri çağrılır.
                if (this.Owner is ConnectForm cf && !cf.IsDisposed)
                {
                    cf.ReturnFromDisconnect();
                }
                else // Değilse yeni bir ConnectForm açılır
                {
                    var cfNew = new ConnectForm();
                    cfNew.Show();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                miDisconnect.Enabled = true;
                _logoutRequested = false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            miDisconnect_Click(sender, e);
        }

        // -----------------------------
        // Manual Read
        // -----------------------------
        private void miManualRead_Click(object sender, EventArgs e)
        {
            if (_modbus == null)
            {
                PostStatus("Modbus servisi hazır değil (MainForm DI ile açılmadı).");
                MessageBox.Show(
                    "Modbus servisi başlatılmamış.\nMainForm DI constructor ile açılmalı.",
                    "Internal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Zaten açıksa öne getir
            if (_manualReadForm != null && !_manualReadForm.IsDisposed)
            {
                _manualReadForm.BringToFront();
                _manualReadForm.Activate();
                return;
            }

            // ManualRead açılırken: polling durdur
            _manualReadOpen = true;
            _uiScheduler?.Stop();

            lblPollState.Text = "Polling: Paused";
            PostStatus("Manual Read açıldı → polling durduruldu.");
            PostQcmStatus("QCM: polling paused");

            _manualReadForm = new ManualReadForm(_modbus)
            {
                Owner = this,
                StartPosition = FormStartPosition.CenterParent
            };

            _manualReadForm.FormClosed += (_, __) =>
            {
                _manualReadForm = null;
                _manualReadOpen = false;

                // ManualRead kapanınca: polling resume
                if (_modbus?.IsConnected == true)
                {
                    lblPollState.Text = "Polling: Active";
                    PostStatus("Manual Read kapandı → polling devam ediyor.");
                    PostQcmStatus("QCM: waiting for poll");
                    _uiScheduler?.Start();
                }
                else
                {
                    lblPollState.Text = "Polling: Stopped";
                    PostStatus("Manual Read kapandı → disconnected.");
                    PostQcmStatus("QCM: disconnected");
                }
            };

            _manualReadForm.Show(this);
        }

        // -----------------------------
        // Tor unit combo box
        // -----------------------------
        private void InitTorUnitCombo()
        {
            cmbTorUnit.DataSource = Enum.GetValues(typeof(PressureUnit));
            cmbTorUnit.SelectedItem = PressureUnit.Torr;
            cmbTorUnit.SelectedIndexChanged += CmbTorUnit_SelectedIndexChanged;
        }
        // Tor birimi değiştiğinde çağrılır
        private void CmbTorUnit_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_lastTorTorr.HasValue)
            {
                UpdateTorUIFromRawTorr(_lastTorTorr.Value);
            }
            else
                lblTorDeger.Text = "--";
        }

        private void UiLog(string msg)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiLog(msg))); return; }

            bool autoScroll = IsLogScrolledToBottom();
            int topIndex = lstLog.TopIndex;

            lstLog.Items.Add($"{DateTime.Now:HH:mm:ss} {msg}");

            if (autoScroll)
            {
                lstLog.TopIndex = lstLog.Items.Count - 1;
            }
            else if (topIndex >= 0 && topIndex < lstLog.Items.Count)
            {
                lstLog.TopIndex = topIndex;
            }
        }

        private bool IsLogScrolledToBottom()
        {
            if (lstLog.Items.Count == 0)
                return true;

            int itemHeight = Math.Max(1, lstLog.ItemHeight);
            int visibleItemCount = Math.Max(1, lstLog.ClientSize.Height / itemHeight);
            int lastVisibleIndex = lstLog.TopIndex + visibleItemCount - 1;

            return lastVisibleIndex >= lstLog.Items.Count - 1;
        }
    }
}
