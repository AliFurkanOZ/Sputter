using SputterControlApp.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private bool _shownInitStarted = false;
        private bool _initialViewPrepared = false;
        private Task? _warmupTask;

        // ✅ Cleanup bir kez çalışsın diye guard
        private bool _cleanupDone = false;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //RefreshComPorts();
            //numTimeoutMs.Value = 1500;
        }

        //MainForm OnShown override
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_shownInitStarted) return;
            _shownInitStarted = true;

            EnsureInitialViewPrepared();
            _ = WarmupAsync(); // görünmeden önce de çağrılabilir
        }

        public Task WarmupAsync()
            => _warmupTask ??= WarmupCoreAsync();

        private void EnsureInitialViewPrepared()
        {
            if (_initialViewPrepared) return;
            _initialViewPrepared = true;

            dgvMaterials.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            dgvMaterials.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            dgvProcesses.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            dgvProcesses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            dgvLayers.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            dgvLayers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            BindGridShortcuts();

            dgvLayers.SelectionChanged -= dgvLayers_SelectionChanged;
            dgvLayers.SelectionChanged += dgvLayers_SelectionChanged;
        }

        private void InitializePlots()
        {
            InitTorPlotOnPanel();
            InitTorMiniSparkline();
            InitRatePlotOnPanel();
            InitThicknessPlotOnPanel();
        }

        private void EnsurePollingScheduler()
        {
            if (_uiScheduler != null)
                return;

            _uiScheduler = new UiTaskScheduler(
                job250ms: async () =>
                {
                    await PollMfcAsync();
                },
                job500ms: async () =>
                {
                    await PollTorAsync();
                    await PollStm2LifeForAllSensorsAsync();
                    await PollStm2RatesForAllSensorsWhenIdleAsync();
                    await PollStm2SensorsForActiveLayerAsync();
                },
                job1000ms: async () =>
                {
                    await PollRF1Async();
                    await PollRF2Async();
                    await PollDCAsync();
                    await PollTurboAsync();
                    await PollRezistanceAsync();
                    await PollDeviceStatusesAsync();
                    await PollEmergencyStopAsync();
                    //Events_TestDump();
                });
        }

        private void StartPolling()
        {
            if (_modbus?.IsConnected != true)
                return;

            EnsurePollingScheduler();
            _uiScheduler?.Start();

            lblPollState.Text = "Polling: Active";
            PostStatus("Polling started.");
        }

        private async Task WarmupCoreAsync()
        {
            EnsureInitialViewPrepared();

            try
            {
                // Seed -> dgvMaterials
                await LoadMaterialsToGridAsync();
                // Seed -> dgvProcesses
                await LoadProcessesToGridAsync();
                // Seed -> dgvLayers
                await LoadLayersToGridAsync();
                UpdateLayerUiState();

                _ = InitializeStm2AfterStartupAsync();

                // Splash görünürken veya form ilk kez çizilirken UI nefes alsın.
                await Task.Yield();

                if (_modbus?.IsConnected == true)
                {
                    lblConnState.Text = "Connected";
                    lblPollState.Text = "Polling: Warming up...";

                    // Plotlar polling başlamadan önce hazır olsun.
                    InitializePlots();

                    await LoadTargetLifeValuesFromTargetsAsync();
                    SyncDcUsedWhFromUi();
                    SyncRf1UsedWhFromUi();
                    SyncRf2UsedWhFromUi();

                    // İlk görünüm için gerekli başlangıç verileri.
                    await ReadSnapshotAsync(rethrowOnFailure: true);

                    // Polling warmup tamamlanınca başlasın.
                    StartPolling();
                }
                else
                {
                    lblConnState.Text = "Disconnected";
                    lblPollState.Text = "Polling: Stopped";
                    PostStatus("Disconnected.");
                }
            }
            catch (Exception ex)
            {
                PostStatus($"Warmup error: {ex.Message}");
                lblPollState.Text = "Polling: Stopped";
                throw;
            }
        }

        private async Task InitializeStm2AfterStartupAsync()
        {
            try
            {
                PostStatus("STM-2 auto connect started...");
                PostQcmStatus("QCM: STM-2 auto connect started");
                await AutoConnectStm2BySerialAsync(showWarningDialog: false);
                UpdateLayerUiState();
                await PollStm2LifeForAllSensorsAsync();
            }
            catch (Exception ex)
            {
                PostStatus($"STM-2 init error: {ex.Message}");
                PostQcmStatus($"QCM init error: {ex.Message}");
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // ✅ Önce cleanup (bir kez)
            CleanupOnExitOnce();

            base.OnFormClosing(e);

            if (!_logoutRequested && !_appExitRequested && e.CloseReason == CloseReason.UserClosing)
            {
                _appExitRequested = true;
                Application.Exit();
            }
        }

        // =====================================================
        // ✅ Cleanup method (Designer.cs'e dokunmadan)
        // =====================================================
        private void CleanupOnExitOnce()
        {
            if (_cleanupDone) return;
            _cleanupDone = true;

            // 1) Scheduler
            try { _uiScheduler?.Stop(); } catch { }
            try { _uiScheduler?.Dispose(); } catch { }
            _uiScheduler = null;

            // 2) Supervisors (event unsubscribe + watcher cancel)
            try { _rf1Sup?.Dispose(); } catch { }
            //try { _dcSup?.Dispose(); } catch { }
            //try { _rf2Sup?.Dispose(); } catch { }

            _rf1Sup = null;
            //_dcSup = null;
            //_rf2Sup = null;

            // 3) Internal CTS (sequence / confirm)
            try { _rf1SeqCts?.Cancel(); } catch { }
            try { _rf1ConfirmCts?.Cancel(); } catch { }
            try { _rf2SeqCts?.Cancel(); } catch { }
            try { _dcSeqCts?.Cancel(); } catch { }

            DisposeStm2Clients();

            _rf1SeqCts = null;
            _rf1ConfirmCts = null;
            _rf2SeqCts = null;
            _dcSeqCts = null;

            // 4) Event log UI cleanup
            TeardownEventsUi();

            // 5) Modbus disconnect
            try { _modbus?.Disconnect(); } catch { }
        }

    }
}
