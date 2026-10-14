using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SputterNewApp.Core.Helpers;
using SputterNewApp.Core.Models;
using SputterNewApp.Core.Registers;
using SputterNewApp.Core.Services;
using SputterNewApp.Infrastructure.Logging;
using SputterNewApp.Infrastructure.Modbus;
using SputterNewApp.Infrastructure.Serial;
using SputterNewApp.Models;

namespace SputterNewApp.Services
{
    public sealed class SputterServiceCoordinator : IDisposable
    {
        public WorkspaceState State { get; }
        public PreviewData Preview { get; }
        public IModbusClientService Modbus { get; }
        public LogService Logger { get; }
        public SafetyInterlockService Interlocks { get; }
        public TargetUsageService TargetTracker { get; }

        public DepositionSequenceController DcSequence { get; }
        public DepositionSequenceController Rf1Sequence { get; }
        public DepositionSequenceController Rf2Sequence { get; }

        public LayerRow? ActiveRunningLayer { get; private set; }
        public bool IsLayerRunning => ActiveRunningLayer != null;
        public event Action<bool, LayerRow?>? LayerRunningStateChanged;
        public event Action<LayerRow>? LayerAutoCompleted;

        private PollingScheduler? _scheduler;
        private Stm2Client? _stmRf1;
        private Stm2Client? _stmRf2;
        private Stm2Client? _stmDc;
        private CancellationTokenSource? _qcmPollingCts;
        private int _isQcmPollingBusy;
        private int _qcmPollCycleCounter;
        private int _isQcmAutoDiscoveryBusy;
        private int _failCountDc;
        private int _failCountRf1;
        private int _failCountRf2;

        private const string STM2_SN_RF1 = "700950";
        private const string STM2_SN_RF2 = "700918";
        private const string STM2_SN_DC = "700969";

        private string _registrationNo = "";
        public ConnectionSettings? LastSettings { get; private set; }
        public string RegistrationNo => _registrationNo;
        private bool _isSnapshotBusy;

        private short? _emergencyStopLastStatus;
        private short? _rf1LastStatus;
        private short? _rf2LastStatus;
        private short? _dcLastStatus;

        private CancellationTokenSource? _rf1WatchCts;
        private CancellationTokenSource? _rf2WatchCts;
        private CancellationTokenSource? _dcWatchCts;

        private enum SourceTransition
        {
            None,
            Starting,
            Stopping
        }

        private SourceTransition _rf1Transition = SourceTransition.None;
        private SourceTransition _rf2Transition = SourceTransition.None;
        private SourceTransition _dcTransition = SourceTransition.None;

        private SourceTransition GetTransition(string sourceName) => sourceName switch
        {
            "RF1" => _rf1Transition,
            "RF2" => _rf2Transition,
            "DC" => _dcTransition,
            _ => SourceTransition.None
        };

        private void SetTransition(string sourceName, SourceTransition transition)
        {
            switch (sourceName)
            {
                case "RF1": _rf1Transition = transition; break;
                case "RF2": _rf2Transition = transition; break;
                case "DC": _dcTransition = transition; break;
            }
        }

        public SputterServiceCoordinator(WorkspaceState state, PreviewData preview)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            Preview = preview ?? throw new ArgumentNullException(nameof(preview));

            Modbus = new NModbusTcpClientService();
            Logger = new LogService();
            Interlocks = new SafetyInterlockService();
            TargetTracker = new TargetUsageService();
            LastSettings = new ConnectionSettings();

            DcSequence = new DepositionSequenceController("DC");
            Rf1Sequence = new DepositionSequenceController("RF1");
            Rf2Sequence = new DepositionSequenceController("RF2");

            Logger.OnNewEvent += OnLogEventReceived;

            WireSequenceEvents();
        }

        private void WireSequenceEvents()
        {
            DcSequence.Step1ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Dc.Step1.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            DcSequence.Step2ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Dc.Step2.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            DcSequence.SequenceCompletedAsync += async () =>
            {
                await StopSourceAsync("DC");
            };

            Rf1Sequence.Step1ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Rf1.Step1.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            Rf1Sequence.Step2ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Rf1.Step2.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            Rf1Sequence.SequenceCompletedAsync += async () =>
            {
                await StopSourceAsync("RF1");
            };

            Rf2Sequence.Step1ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Rf2.Step1.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            Rf2Sequence.Step2ProgressChanged += (rem, total) =>
                RunOnUi(() => State.Rf2.Step2.Remaining = DepositionSequenceController.FormatStepProgress(total, rem));
            Rf2Sequence.SequenceCompletedAsync += async () =>
            {
                await StopSourceAsync("RF2");
            };
        }

        private void OnLogEventReceived(LogEvent ev)
        {
            RunOnUi(() =>
            {
                Preview.Events.Insert(0, new LogRow(
                    ev.TimeText,
                    ev.LevelText,
                    ev.Code,
                    ev.Message,
                    ev.Source.ToString(),
                    ev.RegistrationNo
                ));

                while (Preview.Events.Count > 1000)
                    Preview.Events.RemoveAt(Preview.Events.Count - 1);
            });
        }

        public async Task ConnectAsync(string ip, int port, string regNo, CancellationToken ct = default)
        {
            _registrationNo = regNo?.Trim() ?? "";

            var settings = new ConnectionSettings
            {
                IpAddress = ip,
                Port = port,
                UnitId = 1,
                RegistrationNo = _registrationNo
            };

            await Modbus.ConnectAsync(settings, ct);

            // İlk doğrulama okuması: Soketin açık ve PLC'nin cevap verebilir olduğunu doğrula
            try
            {
                await Modbus.ReadHoldingRegistersAsync(100, 1, ct);
            }
            catch (Exception ex)
            {
                Modbus.Disconnect();
                throw new IOException($"PLC bağlantısı açıldı ancak veri okunamadı (Soket karşı taraf veya ana makine tarafından kapatıldı): {ex.Message}", ex);
            }

            LastSettings = settings;
            _ = AppDataManager.SaveConnectionSettingsAsync(settings);

            RunOnUi(() =>
            {
                State.IsConnected = true;
                State.ConnectionStatusText = $"Bağlı: {ip}:{port}";
                State.PlcStatusText = "PLC: Bağlı";
            });

            LogInfo("PLC", $"Modbus TCP bağlantısı kuruldu ({ip}:{port}).");

            if (Modbus.IsConnected)
            {
                RunOnUi(() => State.PollingStatusText = "Polling: Isınıyor...");
                await ReadSnapshotAsync();
                StartPolling();
            }
        }

        public async Task AutoConnectLastSettingsAsync(CancellationToken ct = default)
        {
            try
            {
                var settings = await AppDataManager.LoadConnectionSettingsAsync();
                LastSettings = settings;

                if (!string.IsNullOrWhiteSpace(settings.IpAddress) && settings.Port > 0)
                {
                    RunOnUi(() =>
                    {
                        State.ConnectionStatusText = $"Otomatik bağlanıyor ({settings.IpAddress}:{settings.Port})...";
                    });

                    await ConnectAsync(settings.IpAddress, settings.Port, settings.RegistrationNo, ct);
                }
            }
            catch (Exception ex)
            {
                LogWarn("PLC", $"Otomatik bağlantı kurulamadı ({LastSettings?.IpAddress}:{LastSettings?.Port}): {ex.Message}");
                RunOnUi(() =>
                {
                    State.IsConnected = false;
                    State.ConnectionStatusText = LastSettings != null ? $"Bağlı değil (Son: {LastSettings.IpAddress})" : "Bağlı değil";
                    State.PlcStatusText = "PLC: Bağlı değil";
                });
            }
        }

        public void Disconnect()
        {
            _scheduler?.Stop();
            _scheduler?.Dispose();
            _scheduler = null;

            CancelAllSourceWatchers();
            _rf1LastStatus = null;
            _rf2LastStatus = null;
            _dcLastStatus = null;

            DcSequence.Cancel();
            Rf1Sequence.Cancel();
            Rf2Sequence.Cancel();

            Modbus.Disconnect();

            RunOnUi(() =>
            {
                State.IsConnected = false;
                State.ConnectionStatusText = LastSettings != null ? $"Bağlı değil (Son: {LastSettings.IpAddress})" : "Bağlı değil";
                State.PollingStatusText = "Polling: Durduruldu";
                State.PlcStatusText = "PLC: —";
                State.Dc.IsActive = false; State.Dc.Status = "OFF";
                State.Rf1.IsActive = false; State.Rf1.Status = "OFF";
                State.Rf2.IsActive = false; State.Rf2.Status = "OFF";
                State.ResetGateValve();
            });

            LogInfo("PLC", "Bağlantı kesildi.");
        }

        public async Task WarmupAndInitializeAsync()
        {
            try
            {
                await LoadAllStorageDataAsync();

                _ = Task.Run(() => InitializeStm2Async());

                // En son girilen IP adresine otomatik olarak bağlan
                await AutoConnectLastSettingsAsync();
            }
            catch (Exception ex)
            {
                LogError("INIT", $"Warmup hatası: {ex.Message}");
            }
        }

        private async Task LoadAllStorageDataAsync()
        {
            var mats = await AppDataManager.LoadMaterialsAsync().ConfigureAwait(false);
            var procs = await AppDataManager.LoadProcessesAsync().ConfigureAwait(false);
            var layers = await AppDataManager.LoadLayersAsync().ConfigureAwait(false);
            var targets = await AppDataManager.LoadTargetsAsync().ConfigureAwait(false);

            RunOnUi(() =>
            {
                string? prevMatId = Preview.SelectedMaterial?.Id;
                string? prevProcId = Preview.SelectedProcess?.Id;
                string? prevLayerId = Preview.SelectedLayer?.Id;

                Preview.Materials.Clear();
                for (int i = 0; i < mats.Count; i++)
                    Preview.Materials.Add(new MaterialRow((i + 1).ToString(), mats[i].Name, mats[i].Density.ToString("0.00"), mats[i].ZRatio.ToString("0.000"), mats[i].Id));

                Preview.Processes.Clear();
                for (int i = 0; i < procs.Count; i++)
                {
                    var matName = mats.FirstOrDefault(m => m.Id == procs[i].MaterialId)?.Name 
                               ?? (!string.IsNullOrWhiteSpace(procs[i].MaterialName) ? procs[i].MaterialName : null)
                               ?? mats.FirstOrDefault()?.Name
                               ?? "—";
                    Preview.Processes.Add(new ProcessRow((i + 1).ToString(), procs[i].Name, matName, $"{procs[i].Tooling} %", procs[i].Id));
                }

                Preview.Layers.Clear();
                for (int i = 0; i < layers.Count; i++)
                {
                    var proc = procs.FirstOrDefault(p => p.Id == layers[i].ProcessId);
                    var matName = proc != null 
                        ? mats.FirstOrDefault(m => m.Id == proc.MaterialId)?.Name 
                          ?? (!string.IsNullOrWhiteSpace(proc.MaterialName) ? proc.MaterialName : null)
                          ?? mats.FirstOrDefault()?.Name
                          ?? "—"
                        : "—";

                    string sensorName = layers[i].RF1SensorEnabled ? "RF1" 
                                      : layers[i].RF2SensorEnabled ? "RF2" 
                                      : layers[i].DCSensorEnabled ? "DC" 
                                      : layers[i].Source.ToString();

                    Preview.Layers.Add(new LayerRow((i + 1).ToString(), proc?.Name ?? "—", matName, layers[i].FinalThickness.ToString("0.00"), layers[i].Source.ToString(), sensorName, "Auto", "—", layers[i].Id));
                }

                Preview.Targets.Clear();
                foreach (var t in targets)
                    Preview.Targets.Add(new TargetRow(t.Name, string.IsNullOrWhiteSpace(t.Source) ? "—" : t.Source, t.UsedWh.ToString("0.00"), t.Id));

                Preview.SelectedMaterial = Preview.Materials.FirstOrDefault(m => m.Id == prevMatId) ?? Preview.Materials.FirstOrDefault();
                Preview.SelectedProcess = Preview.Processes.FirstOrDefault(p => p.Id == prevProcId) ?? Preview.Processes.FirstOrDefault();
                Preview.SelectedLayer = Preview.Layers.FirstOrDefault(l => l.Id == prevLayerId) ?? Preview.Layers.FirstOrDefault();
            });
        }

        private async Task InitializeStm2Async()
        {
            try
            {
                RunOnUi(() => State.QcmStatusText = "QCM: Otomatik aranıyor...");
                await AutoConnectStm2BySerialAsync();
            }
            catch (Exception ex)
            {
                LogWarn("QCM", $"STM-2 otomatik bağlantı sırasında hata: {ex.Message}");
                RunOnUi(() => State.QcmStatusText = "QCM: Bağlantı hatası");
            }
        }

        private void StartPolling()
        {
            if (_scheduler != null) return;

            _scheduler = new PollingScheduler(
                job250ms: PollMfcAsync,
                job500ms: PollTorAndQcmAsync,
                job1000ms: PollDevicesAndSourcesAsync
            );

            _scheduler.Start();
            RunOnUi(() => State.PollingStatusText = "Polling: Aktif");
        }

        public async Task ReadSnapshotAsync()
        {
            if (_isSnapshotBusy || !Modbus.IsConnected) return;
            _isSnapshotBusy = true;

            try
            {
                // 1) D100..D120 Manuel start durumları
                var regs100 = await Modbus.ReadHoldingRegistersAsync(100, 21);
                for (int i = 0; i < regs100.Length; i++)
                {
                    ushort addr = (ushort)(100 + i);
                    bool active = regs100[i] != 0;
                    UpdateDeviceStateUi(addr, active);
                }

                // 2) MFC Blok D10..D89 (80 registers)
                var regsMfc = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.D1ActualFlow, 80);
                ApplyMfcBlock(regsMfc);

                // 3) TorDeger D123..D128
                var regsTor = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.TorDegerAux, 6);
                float torAux = ModbusTypeExtensions.ToFloat32_CDAB(regsTor[0], regsTor[1]);
                float torMain = ModbusTypeExtensions.ToFloat32_CDAB(regsTor[4], regsTor[5]);
                RunOnUi(() =>
                {
                    State.Pressure = FormatSci(torMain);
                    State.Capacitance = FormatSci(torAux);
                    State.RawPressureTorr = torMain;
                });

                // 4) GateValve, Motor, Turbo D150..D154 (5 registers)
                var regs150 = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.GateValve_Position, 5);
                RunOnUi(() =>
                {
                    short rawPos = unchecked((short)regs150[0]);
                    short motorRpm = unchecked((short)regs150[1]);
                    short turboFreq = unchecked((short)regs150[2]);
                    short gvState = unchecked((short)regs150[4]);

                    short actualPos = rawPos == 132 ? (short)100 : Math.Clamp(rawPos, (short)0, (short)100);
                    State.GateSet = actualPos;
                    State.RotationRpm = motorRpm;
                    if (State.RotationSet == 0 && motorRpm > 0) State.RotationSet = motorRpm;
                    State.TurboFrequency = (turboFreq / 10).ToString();
                    State.UpdateGateValve(rawPos, gvState);
                });

                // 5) Ordel Rezistans D292..D295
                var regsOrdel = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.Ordel_SetValue, 4);
                RunOnUi(() =>
                {
                    State.HeaterSet = (decimal)(regsOrdel[0] / 10.0f);
                    State.HeaterRead = (regsOrdel[2] / 10.0f).ToString("0.0");
                });

                // 6) RF1 D131..D141 (regs[9] is D140 RF1_Status)
                var regsRf1 = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.RF1_Step1_SetW, 11);
                RunOnUi(() =>
                {
                    State.Rf1.Step1.Setpoint = regsRf1[0];
                    State.Rf1.Step1.Ramp = regsRf1[1];
                    State.Rf1.Step1.Dwell = regsRf1[2];
                    State.Rf1.Step2.Setpoint = regsRf1[3];
                    State.Rf1.Step2.Ramp = regsRf1[4];
                    State.Rf1.Step2.Dwell = regsRf1[5];
                    State.Rf1.OffRamp = regsRf1[6];
                    State.Rf1.Read1 = regsRf1[7].ToString(); // FwP
                    State.Rf1.Read2 = regsRf1[8].ToString(); // RefP
                    State.Rf1.Read3 = regsRf1[10].ToString(); // ReadW
                });
                short rf1Stat = unchecked((short)regsRf1[9]);
                _rf1LastStatus = rf1Stat;
                ApplySourceConfirmedState("RF1", rf1Stat == 1);

                // 7) RF2 D281..D291 (regs[9] is D290 RF2_Status)
                var regsRf2 = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.RF2_Step1_SetW, 11);
                RunOnUi(() =>
                {
                    State.Rf2.Step1.Setpoint = regsRf2[0];
                    State.Rf2.Step1.Ramp = regsRf2[1];
                    State.Rf2.Step1.Dwell = regsRf2[2];
                    State.Rf2.Step2.Setpoint = regsRf2[3];
                    State.Rf2.Step2.Ramp = regsRf2[4];
                    State.Rf2.Step2.Dwell = regsRf2[5];
                    State.Rf2.OffRamp = regsRf2[6];
                    State.Rf2.Read1 = regsRf2[7].ToString();
                    State.Rf2.Read2 = regsRf2[8].ToString();
                    State.Rf2.Read3 = regsRf2[10].ToString();
                });
                short rf2Stat = unchecked((short)regsRf2[9]);
                _rf2LastStatus = rf2Stat;
                ApplySourceConfirmedState("RF2", rf2Stat == 1);

                // 8) DC D0..D9
                var regsDc = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.DC_Step1_SetW, 9);
                RunOnUi(() =>
                {
                    State.Dc.Step1.Setpoint = regsDc[0];
                    State.Dc.Step1.Ramp = regsDc[1];
                    State.Dc.Step1.Dwell = regsDc[2];
                    State.Dc.Step2.Setpoint = regsDc[3];
                    State.Dc.Step2.Ramp = regsDc[4];
                    State.Dc.Step2.Dwell = regsDc[5];
                    State.Dc.OffRamp = regsDc[6];
                    State.Dc.VoltageSet = regsDc[7];
                    State.Dc.CurrentSet = regsDc[8];
                });

                // 9) DC Status D222
                var dcStateReg = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.DC_Status, 1);
                short dcStat = unchecked((short)dcStateReg[0]);
                _dcLastStatus = dcStat;
                ApplySourceConfirmedState("DC", dcStat == 1);

                LogInfo("PLC", "Snapshot yenileme tamamlandı.");
            }
            catch (Exception ex)
            {
                LogError("PLC", $"Snapshot okuma hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
            finally
            {
                _isSnapshotBusy = false;
            }
        }

        private void ApplyMfcBlock(ushort[] regs)
        {
            // baseAddr = 10
            int Idx(ushort addr) => addr - 10;
            float ReadF32(ushort addr) => ModbusTypeExtensions.ToFloat32_CDAB(regs[Idx(addr)], regs[Idx(addr) + 1]);
            short ReadI16(ushort addr) => unchecked((short)regs[Idx(addr)]);

            RunOnUi(() =>
            {
                // ID1: RF2 Ar
                State.Rf2.Argon.Actual = ReadF32(10).ToString("0.##");
                State.Rf2.Argon.Setpoint = (decimal)ReadF32(14);
                State.Rf2.Argon.Valve.IsOpen = ReadI16(22) == 1;

                // ID2: RF2 O2
                State.Rf2.Oxygen.Actual = ReadF32(26).ToString("0.##");
                State.Rf2.Oxygen.Setpoint = (decimal)ReadF32(30);
                State.Rf2.Oxygen.Valve.IsOpen = ReadI16(38) == 1;

                // ID3: RF1 Ar
                State.Rf1.Argon.Actual = ReadF32(42).ToString("0.##");
                State.Rf1.Argon.Setpoint = (decimal)ReadF32(46);
                State.Rf1.Argon.Valve.IsOpen = ReadI16(54) == 1;

                // ID4: DC Ar
                State.Dc.Argon.Actual = ReadF32(58).ToString("0.##");
                State.Dc.Argon.Setpoint = (decimal)ReadF32(62);
                State.Dc.Argon.Valve.IsOpen = ReadI16(70) == 1;

                // ID5: DC O2
                State.Dc.Oxygen.Actual = ReadF32(74).ToString("0.##");
                State.Dc.Oxygen.Setpoint = (decimal)ReadF32(78);
                State.Dc.Oxygen.Valve.IsOpen = ReadI16(86) == 1;
            });
        }

        private async Task PollMfcAsync()
        {
            if (!Modbus.IsConnected) return;

            try
            {
                var regs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.D1ActualFlow, 80);
                int Idx(ushort addr) => addr - 10;
                float ReadF32(ushort addr) => ModbusTypeExtensions.ToFloat32_CDAB(regs[Idx(addr)], regs[Idx(addr) + 1]);

                RunOnUi(() =>
                {
                    State.Rf2.Argon.Actual = ReadF32(10).ToString("0.##");
                    State.Rf2.Oxygen.Actual = ReadF32(26).ToString("0.##");
                    State.Rf1.Argon.Actual = ReadF32(42).ToString("0.##");
                    State.Dc.Argon.Actual = ReadF32(58).ToString("0.##");
                    State.Dc.Oxygen.Actual = ReadF32(74).ToString("0.##");
                });
            }
            catch (Exception ex)
            {
                CheckAndHandleDisconnection(ex);
            }
        }

        private async Task PollTorAndQcmAsync()
        {
            if (!Modbus.IsConnected) return;

            // 1) Tor Vakum (500ms)
            try
            {
                var regs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.TorDegerAux, 6);
                float aux = ModbusTypeExtensions.ToFloat32_CDAB(regs[0], regs[1]);
                float mainTor = ModbusTypeExtensions.ToFloat32_CDAB(regs[4], regs[5]);

                RunOnUi(() =>
                {
                    State.Pressure = FormatSci(mainTor);
                    State.Capacitance = FormatSci(aux);
                    State.RawPressureTorr = mainTor;
                });
            }
            catch (Exception ex)
            {
                CheckAndHandleDisconnection(ex);
            }

            // 2) Gate Valve & Turbo Frekans D150..D154 (500ms)
            await PollGateValveAsync();
        }

        private async Task PollGateValveAsync()
        {
            if (!Modbus.IsConnected) return;

            try
            {
                var regs150 = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.GateValve_Position, 5);
                short rawPos = unchecked((short)regs150[0]);
                short motorRpm = unchecked((short)regs150[1]);
                short turboFreq = unchecked((short)regs150[2]);
                short gvState = unchecked((short)regs150[4]);

                RunOnUi(() =>
                {
                    State.TurboFrequency = (turboFreq / 10).ToString();
                    State.RotationRpm = motorRpm;
                    State.UpdateGateValve(rawPos, gvState);
                });
            }
            catch (Exception ex)
            {
                CheckAndHandleDisconnection(ex);
            }
        }

        private async Task PollStm2SensorsAsync()
        {
            if (Interlocked.Exchange(ref _isQcmPollingBusy, 1) == 1) return;

            try
            {
                int cycle = Interlocked.Increment(ref _qcmPollCycleCounter);
                bool shouldPollLife = (cycle % 10 == 0); // Her 5 saniyede bir kristal ömrü sorgula
                bool isLayerRunning = ActiveRunningLayer != null;
                bool shouldPollThk = isLayerRunning || (cycle % 2 == 0); // Aktif katmanda her cycle, boştayken her 1 sn

                string[] systemPorts = SerialPort.GetPortNames();

                // DC, RF1 ve RF2 sensörlerini paralel (eş zamanlı) olarak oku ve durumlarını kontrol et
                var taskDc = PollSingleSensorAsync(_stmDc, "DC", shouldPollThk, shouldPollLife, systemPorts);
                var taskRf1 = PollSingleSensorAsync(_stmRf1, "RF1", shouldPollThk, shouldPollLife, systemPorts);
                var taskRf2 = PollSingleSensorAsync(_stmRf2, "RF2", shouldPollThk, shouldPollLife, systemPorts);

                await Task.WhenAll(taskDc, taskRf1, taskRf2).ConfigureAwait(false);

                // Üst/alt durum çubuğundaki genel QCM özetini güncelle
                UpdateQcmStatusSummary();

                // 4) Active Layer Auto-Stop Kontrolü (SputterControlApp algoritması)
                if (ActiveRunningLayer != null)
                {
                    var layer = ActiveRunningLayer;
                    var s = Preview.GetSensorBySource(layer.Source);
                    if (s != null
                        && float.TryParse(s.Thickness.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var currentKa)
                        && float.TryParse(layer.Thickness.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var targetKa)
                        && targetKa > 0
                        && currentKa >= targetKa)
                    {
                        LogInfo("QCM", $"[AUTO-STOP] Hedef kalınlığa ulaşıldı ({layer.Source}): Mevcut={currentKa:0.00} kÅ >= Hedef={targetKa:0.00} kÅ. Katman tamamlanıyor...");
                        _ = Task.Run(async () =>
                        {
                            await StopLayerExecutionAsync(manual: false);
                            RunOnUi(() => LayerAutoCompleted?.Invoke(layer));
                        });
                    }
                }

                // 5) Otomatik Tak-Çalıştır Algılama (Plug-and-Play)
                // Herhangi bir sensör bağlı değilse, her 3 saniyede bir yeni takılan COM port var mı kontrol et
                bool hasMissingSensor = (_stmDc == null || _stmRf1 == null || _stmRf2 == null);
                if (hasMissingSensor && (cycle % 6 == 0))
                {
                    _ = Task.Run(() => TryAutoDiscoverMissingSensorsAsync(systemPorts));
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isQcmPollingBusy, 0);
            }
        }

        private async Task PollSingleSensorAsync(Stm2Client? client, string source, bool pollThickness, bool pollLife, string[] systemPorts)
        {
            var s = Preview.GetSensorBySource(source);
            if (s == null) return;

            if (client == null)
            {
                if (s.IsConnected)
                {
                    RunOnUi(() => s.Reset());
                }
                return;
            }

            // 1) Donanımsal Kontrol: Port Windows'tan fiziki olarak çıkarıldı mı?
            bool portExists = systemPorts.Any(p => string.Equals(p, client.PortName, StringComparison.OrdinalIgnoreCase));
            if (!portExists)
            {
                DisconnectSensor(source, $"{source} ({client.PortName}) sensörünün USB kablosu çıkarıldı.");
                return;
            }

            try
            {
                // Rate: her 500ms döngüde hızlıca oku
                var rStr = await client.TryGetRateAsync(500).ConfigureAwait(false);

                // Yanıt alınamadıysa sensör iletişim hatası (kablo koptu veya cihaz kapalı)
                if (string.IsNullOrWhiteSpace(rStr))
                {
                    int fails = IncrementFailureCount(source);
                    if (fails >= 2) // 2 ardışık başarısızlık (~1 saniye yanıt yok)
                    {
                        DisconnectSensor(source, $"{source} ({client.PortName}) sensöründen yanıt alınamadı, bağlantı kesildi.");
                    }
                    return;
                }

                // Yanıt başarılı -> Hata sayacını sıfırla
                ResetFailureCount(source);

                string? thkStr = null;
                if (pollThickness)
                {
                    thkStr = await client.TryGetThicknessAsync(500).ConfigureAwait(false);
                }

                string? lifeStr = null;
                if (pollLife)
                {
                    lifeStr = await client.TryGetLifeAsync(700).ConfigureAwait(false);
                }

                RunOnUi(() =>
                {
                    s.Rate = ParseOrDash(rStr);
                    if (thkStr != null) s.Thickness = FormatAngstromToKa(thkStr);
                    if (lifeStr != null) s.Life = ParseOrDash(lifeStr);
                    s.IsConnected = true;
                    s.Port = client.PortName;
                    if (!string.IsNullOrEmpty(client.SerialNumber))
                        s.SerialNo = client.SerialNumber;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QCM] {source} okuma istisnası ({client.PortName}): {ex.Message}");
                int fails = IncrementFailureCount(source);
                if (fails >= 2)
                {
                    DisconnectSensor(source, $"{source} ({client.PortName}) iletişim hatası: {ex.Message}, bağlantı kesildi.");
                }
            }
        }

        private void DisconnectSensor(string source, string reason)
        {
            Stm2Client? client = null;
            switch (source.ToUpperInvariant())
            {
                case "DC":
                    client = _stmDc;
                    _stmDc = null;
                    _failCountDc = 0;
                    break;
                case "RF1":
                    client = _stmRf1;
                    _stmRf1 = null;
                    _failCountRf1 = 0;
                    break;
                case "RF2":
                    client = _stmRf2;
                    _stmRf2 = null;
                    _failCountRf2 = 0;
                    break;
            }

            try { client?.Dispose(); } catch { }

            var s = Preview.GetSensorBySource(source);
            if (s != null)
            {
                RunOnUi(() => s.Reset());
            }

            UpdateQcmStatusSummary();
            LogWarn("QCM", reason);
        }

        private int IncrementFailureCount(string source)
        {
            return source.ToUpperInvariant() switch
            {
                "DC" => Interlocked.Increment(ref _failCountDc),
                "RF1" => Interlocked.Increment(ref _failCountRf1),
                "RF2" => Interlocked.Increment(ref _failCountRf2),
                _ => 1
            };
        }

        private void ResetFailureCount(string source)
        {
            switch (source.ToUpperInvariant())
            {
                case "DC": Interlocked.Exchange(ref _failCountDc, 0); break;
                case "RF1": Interlocked.Exchange(ref _failCountRf1, 0); break;
                case "RF2": Interlocked.Exchange(ref _failCountRf2, 0); break;
            }
        }

        private async Task TryAutoDiscoverMissingSensorsAsync(string[] currentPorts)
        {
            if (Interlocked.Exchange(ref _isQcmAutoDiscoveryBusy, 1) == 1) return;

            try
            {
                // Şu anda kullanımda olan portları belirle
                var activePorts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (_stmDc?.IsOpen == true) activePorts.Add(_stmDc.PortName);
                if (_stmRf1?.IsOpen == true) activePorts.Add(_stmRf1.PortName);
                if (_stmRf2?.IsOpen == true) activePorts.Add(_stmRf2.PortName);

                // Boştaki / yeni takılan COM portları filtrele
                var candidates = currentPorts
                    .Where(p => !activePorts.Contains(p))
                    .ToArray();

                if (candidates.Length == 0) return;

                // Boştaki portları paralel tara
                var probeTasks = candidates.Select(async port =>
                {
                    Stm2Client? client = null;
                    try
                    {
                        client = new Stm2Client(port);
                        client.Open();
                        await Task.Delay(80).ConfigureAwait(false);

                        string? rawSn = await client.TryGetSNAsync(600).ConfigureAwait(false);
                        string sn = "";
                        if (!string.IsNullOrWhiteSpace(rawSn))
                        {
                            var m = Regex.Match(rawSn, @"\b\d{6}\b");
                            sn = m.Success ? m.Value : rawSn.Trim();
                        }

                        if (string.IsNullOrWhiteSpace(sn))
                        {
                            string? ping = await client.TryPingAsync(400).ConfigureAwait(false);
                            if (!string.IsNullOrWhiteSpace(ping))
                            {
                                var m = Regex.Match(ping, @"\b\d{6}\b");
                                sn = m.Success ? m.Value : "STM2";
                            }
                        }

                        // 115200'de yanıt yoksa 9600 baud dene
                        if (string.IsNullOrWhiteSpace(sn))
                        {
                            try
                            {
                                client.Close();
                                client.Dispose();
                                client = new Stm2Client(port, 9600);
                                client.Open();
                                await Task.Delay(80).ConfigureAwait(false);

                                rawSn = await client.TryGetSNAsync(400).ConfigureAwait(false);
                                if (!string.IsNullOrWhiteSpace(rawSn))
                                {
                                    var m = Regex.Match(rawSn, @"\b\d{6}\b");
                                    sn = m.Success ? m.Value : rawSn.Trim();
                                }
                                if (string.IsNullOrWhiteSpace(sn))
                                {
                                    string? ping = await client.TryPingAsync(300).ConfigureAwait(false);
                                    if (!string.IsNullOrWhiteSpace(ping))
                                    {
                                        var m = Regex.Match(ping, @"\b\d{6}\b");
                                        sn = m.Success ? m.Value : "STM2";
                                    }
                                }
                            }
                            catch { }
                        }

                        if (!string.IsNullOrWhiteSpace(sn))
                        {
                            client.SerialNumber = sn;
                            return (port, client, sn);
                        }

                        client.Dispose();
                        return (port, client: (Stm2Client?)null, sn: "");
                    }
                    catch
                    {
                        try { client?.Dispose(); } catch { }
                        return (port, client: (Stm2Client?)null, sn: "");
                    }
                });

                var results = await Task.WhenAll(probeTasks).ConfigureAwait(false);
                var discovered = results
                    .Where(r => r.client != null && !string.IsNullOrWhiteSpace(r.sn))
                    .Select(r => (port: r.port, client: r.client!, sn: r.sn))
                    .ToList();

                if (discovered.Count == 0) return;

                // Bilinen seri numaralarına göre boş slotlara ata
                if (_stmRf1 == null)
                {
                    var m = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_RF1));
                    if (m.client != null)
                    {
                        _stmRf1 = m.client;
                        _stmRf1.SerialNumber = STM2_SN_RF1;
                        ResetFailureCount("RF1");
                        UpdateSensorMetadata("RF1", m.port, STM2_SN_RF1, true);
                        LogInfo("QCM", $"RF1 STM-2 sensörü otomatik bağlandı ({m.port}, SN: {STM2_SN_RF1})");
                        discovered.Remove(m);
                    }
                }

                if (_stmRf2 == null)
                {
                    var m = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_RF2));
                    if (m.client != null)
                    {
                        _stmRf2 = m.client;
                        _stmRf2.SerialNumber = STM2_SN_RF2;
                        ResetFailureCount("RF2");
                        UpdateSensorMetadata("RF2", m.port, STM2_SN_RF2, true);
                        LogInfo("QCM", $"RF2 STM-2 sensörü otomatik bağlandı ({m.port}, SN: {STM2_SN_RF2})");
                        discovered.Remove(m);
                    }
                }

                if (_stmDc == null)
                {
                    var m = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_DC));
                    if (m.client != null)
                    {
                        _stmDc = m.client;
                        _stmDc.SerialNumber = STM2_SN_DC;
                        ResetFailureCount("DC");
                        UpdateSensorMetadata("DC", m.port, STM2_SN_DC, true);
                        LogInfo("QCM", $"DC STM-2 sensörü otomatik bağlandı ({m.port}, SN: {STM2_SN_DC})");
                        discovered.Remove(m);
                    }
                }

                // Kalanları ilk boş slota ata
                foreach (var item in discovered.ToList())
                {
                    string? slot = null;
                    if (_stmRf1 == null) slot = "RF1";
                    else if (_stmRf2 == null) slot = "RF2";
                    else if (_stmDc == null) slot = "DC";

                    if (slot == null)
                    {
                        try { item.client.Dispose(); } catch { }
                        continue;
                    }

                    if (slot == "RF1") { _stmRf1 = item.client; ResetFailureCount("RF1"); }
                    else if (slot == "RF2") { _stmRf2 = item.client; ResetFailureCount("RF2"); }
                    else { _stmDc = item.client; ResetFailureCount("DC"); }

                    UpdateSensorMetadata(slot, item.port, item.sn, true);
                    LogInfo("QCM", $"STM-2 sensörü bulundu ({item.port}, SN: {item.sn}) -> '{slot}' kaynağına bağlandı.");
                }

                UpdateQcmStatusSummary();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[QcmAutoDiscovery] Hata: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _isQcmAutoDiscoveryBusy, 0);
            }
        }

        private void UpdateQcmStatusSummary()
        {
            var dc = Preview.GetSensorBySource("DC");
            var rf1 = Preview.GetSensorBySource("RF1");
            var rf2 = Preview.GetSensorBySource("RF2");

            int count = (dc?.IsConnected == true ? 1 : 0)
                      + (rf1?.IsConnected == true ? 1 : 0)
                      + (rf2?.IsConnected == true ? 1 : 0);

            if (count == 0)
            {
                RunOnUi(() => State.QcmStatusText = "QCM: Bağlı değil");
                return;
            }

            string dcP = dc?.IsConnected == true ? $"DC: {dc.Port}" : "DC: —";
            string rf1P = rf1?.IsConnected == true ? $"RF1: {rf1.Port}" : "RF1: —";
            string rf2P = rf2?.IsConnected == true ? $"RF2: {rf2.Port}" : "RF2: —";

            RunOnUi(() => State.QcmStatusText = $"QCM ({count}/3) | {dcP} | {rf1P} | {rf2P}");
        }

        private async Task PollDevicesAndSourcesAsync()
        {
            if (!Modbus.IsConnected) return;

            try
            {
                // 1) Kaynak Durumları & Readback
                // RF1 D138..D140 (Status: D140)
                var rf1Regs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.RF1_FwP, 3);
                short rf1Fw = unchecked((short)rf1Regs[0]);
                short rf1Ref = unchecked((short)rf1Regs[1]);
                short rf1Stat = unchecked((short)rf1Regs[2]);

                RunOnUi(() =>
                {
                    State.Rf1.Read1 = rf1Fw.ToString();
                    State.Rf1.Read2 = rf1Ref.ToString();
                });

                // PLC geri bildirimi: RF1_Status (D140) okunarak durum yönetilir
                PollSingleSourceStatus("RF1", (ushort)RegisterId.RF1_Status, rf1Stat);

                if (rf1Stat == 1 && decimal.TryParse(State.Rf1.UsedWh, NumberStyles.Any, CultureInfo.InvariantCulture, out var rf1Wh))
                {
                    decimal updated = await TargetTracker.AddUsageAsync(State.Rf1.Target, rf1Wh, rf1Fw);
                    RunOnUi(() => State.Rf1.UsedWh = updated.ToString("0.00"));
                }

                // RF2 D288..D290 (Status: D290)
                var rf2Regs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.RF2_FwP, 3);
                short rf2Fw = unchecked((short)rf2Regs[0]);
                short rf2Ref = unchecked((short)rf2Regs[1]);
                short rf2Stat = unchecked((short)rf2Regs[2]);

                RunOnUi(() =>
                {
                    State.Rf2.Read1 = rf2Fw.ToString();
                    State.Rf2.Read2 = rf2Ref.ToString();
                });

                // PLC geri bildirimi: RF2_Status (D290) okunarak durum yönetilir
                PollSingleSourceStatus("RF2", (ushort)RegisterId.RF2_Status, rf2Stat);

                if (rf2Stat == 1 && decimal.TryParse(State.Rf2.UsedWh, NumberStyles.Any, CultureInfo.InvariantCulture, out var rf2Wh))
                {
                    decimal updated = await TargetTracker.AddUsageAsync(State.Rf2.Target, rf2Wh, rf2Fw);
                    RunOnUi(() => State.Rf2.UsedWh = updated.ToString("0.00"));
                }

                // DC D163..D165 + Status: D222
                var dcRegs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.DC_Read_Voltage, 3);
                short dcV = unchecked((short)dcRegs[0]);
                short dcC = unchecked((short)dcRegs[1]);
                short dcP = unchecked((short)dcRegs[2]);

                var dcOutRegs = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.DC_Status, 1);
                short dcStat = unchecked((short)dcOutRegs[0]);

                RunOnUi(() =>
                {
                    State.Dc.Read1 = dcP.ToString();
                    State.Dc.Read2 = dcV.ToString();
                    State.Dc.Read3 = dcC.ToString();
                });

                // PLC geri bildirimi: DC_Status (D222) okunarak durum yönetilir
                PollSingleSourceStatus("DC", (ushort)RegisterId.DC_Status, dcStat);

                if (dcStat == 1 && decimal.TryParse(State.Dc.UsedWh, NumberStyles.Any, CultureInfo.InvariantCulture, out var dcWh))
                {
                    decimal updated = await TargetTracker.AddUsageAsync(State.Dc.Target, dcWh, dcP);
                    RunOnUi(() => State.Dc.UsedWh = updated.ToString("0.00"));
                }

                // Turbo D152
                var turboReg = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.Turbo_Frequency, 1);
                RunOnUi(() => State.TurboFrequency = (turboReg[0] / 10).ToString());

                // Rezistans D294
                var ordelReg = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.Ordel_Instant_Value, 1);
                RunOnUi(() => State.HeaterRead = (ordelReg[0] / 10.0f).ToString("0.0"));

                // Emergency Stop D350
                var emergReg = await Modbus.ReadHoldingRegistersAsync((ushort)RegisterId.EmergencyStop, 1);
                short emergVal = unchecked((short)emergReg[0]);
                if (_emergencyStopLastStatus != emergVal)
                {
                    _emergencyStopLastStatus = emergVal;
                    bool active = emergVal == 1;
                    Interlocks.IsEmergencyStopActive = active;
                    RunOnUi(() => State.IsEmergencyStopActive = active);
                    if (active)
                    {
                        LogWarn("ALARM", "Acil Stop (D350) basıldı!");
                        CancelAllSourceWatchers();
                        _rf1LastStatus = 0;
                        _rf2LastStatus = 0;
                        _dcLastStatus = 0;
                        ApplySourceConfirmedState("DC", false);
                        ApplySourceConfirmedState("RF1", false);
                        ApplySourceConfirmedState("RF2", false);
                    }
                }

                // D100..D114 Manuel Cihaz Durumları
                var devRegs = await Modbus.ReadHoldingRegistersAsync(100, 15);
                for (int i = 0; i < devRegs.Length; i++)
                {
                    ushort addr = (ushort)(100 + i);
                    bool active = devRegs[i] != 0;
                    UpdateDeviceStateUi(addr, active);
                }
            }
            catch (Exception ex)
            {
                CheckAndHandleDisconnection(ex);
            }
        }

        private int _isDisconnecting;

        public void CheckAndHandleDisconnection(Exception? ex = null)
        {
            if (Interlocked.Exchange(ref _isDisconnecting, 1) == 1) return;

            try
            {
                // 1) Polling döngüsünü derhal durdur (kopuk sokete 250ms'de bir tekrar tekrar istek atıp hata basmasını engelle)
                _scheduler?.Stop();
                _scheduler?.Dispose();
                _scheduler = null;

                CancelAllSourceWatchers();
                _rf1LastStatus = null;
                _rf2LastStatus = null;
                _dcLastStatus = null;

                // 2) Aktif sekansları güvenle iptal et
                DcSequence.Cancel();
                Rf1Sequence.Cancel();
                Rf2Sequence.Cancel();

                // 3) Modbus soketini kapat
                Modbus.Disconnect();

                // 4) UI'ı anında temiz 'Bağlı değil' durumuna geçir
                RunOnUi(() =>
                {
                    State.IsConnected = false;
                    State.ConnectionStatusText = LastSettings != null ? $"Bağlı değil (Son: {LastSettings.IpAddress})" : "Bağlı değil";
                    State.PlcStatusText = "PLC: Bağlı değil";
                    State.PollingStatusText = "Polling: Durduruldu";
                    State.Dc.IsActive = false; State.Dc.Status = "OFF";
                    State.Rf1.IsActive = false; State.Rf1.Status = "OFF";
                    State.Rf2.IsActive = false; State.Rf2.Status = "OFF";
                    State.ResetGateValve();
                });

                LogWarn("PLC", "PLC bağlantısı koptu. Durum: Bağlı değil.");
            }
            finally
            {
                Interlocked.Exchange(ref _isDisconnecting, 0);
            }
        }

        private void UpdateDeviceStateUi(ushort address, bool active)
        {
            var dev = State.GetDeviceByAddress(address);
            if (dev != null)
            {
                RunOnUi(() => dev.IsActive = active);
            }

            switch (address)
            {
                case 100: RunOnUi(() => State.IsRotationActive = active); break;
                case 101: RunOnUi(() => State.IsHeaterActive = active); break;
                case 102: Interlocks.IsRf1PowerOn = active; break;
                case 103: Interlocks.IsRf2PowerOn = active; break;
                case 104: Interlocks.IsDcPowerOn = active; break;
                case 105:
                    Interlocks.IsMatchBoxOn = active;
                    RunOnUi(() => State.IsMatchBoxActive = active);
                    break;
                case 106:
                    RunOnUi(() =>
                    {
                        State.RoughingValve.IsOpen = active;
                        State.IsScrollPumpActive = active;
                    });
                    break;
                case 107: RunOnUi(() => State.IsTurboPumpActive = active); break;
                case 108:
                    Interlocks.IsChillerOn = active;
                    RunOnUi(() => State.IsChillerActive = active);
                    break;
                case 110: RunOnUi(() => State.VentValve.IsOpen = active); break;
                case 111: RunOnUi(() => State.BypassValve.IsOpen = active); break;
                case 112: RunOnUi(() => State.IsRf1ShutterOpen = active); break;
                case 113: RunOnUi(() => State.IsDcShutterOpen = active); break;
                case 114: RunOnUi(() => State.IsRf2ShutterOpen = active); break;
            }
        }

        public async Task ToggleDeviceAsync(ushort address)
        {
            if (!Modbus.IsConnected) return;

            var dev = State.GetDeviceByAddress(address);
            if (dev == null && address != 111) return;

            bool current = dev != null ? (dev.IsActive == true) : (State.BypassValve.IsOpen == true);
            bool target = !current;
            string devName = dev?.Name ?? (address == 111 ? "Bypass Valfi (111)" : $"Cihaz ({address})");

            // Güvenlik Kilitleri
            if (target)
            {
                if (address == 107 && !Interlocks.CanStartTurboVacuum())
                {
                    MessageBox.Show(Interlocks.GetStartBlockReason("Turbo"), "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (address == 104 && !Interlocks.CanStartDcPower())
                {
                    MessageBox.Show(Interlocks.GetStartBlockReason("DC"), "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (address == 102 && !Interlocks.CanStartRf1Power())
                {
                    MessageBox.Show(Interlocks.GetStartBlockReason("RF1"), "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (address == 103 && !Interlocks.CanStartRf2Power())
                {
                    MessageBox.Show(Interlocks.GetStartBlockReason("RF2"), "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (address == 105 && !Interlocks.CanTurnOffMatchBox())
                {
                    MessageBox.Show(Interlocks.GetStartBlockReason("MatchBoxOff"), "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                ushort val = (ushort)(target ? 1 : 0);
                await Modbus.WriteSingleRegisterAsync(address, val);
                RunOnUi(() =>
                {
                    if (dev != null) dev.IsActive = target;
                    if (address == 111) State.BypassValve.IsOpen = target;
                });
                LogInfo("DEVICE", $"{devName} -> {(target ? "START" : "STOP")}");
            }
            catch (Exception ex)
            {
                LogError("DEVICE", $"{devName} işlem hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        private (ushort cmdAddr, ushort statusAddr, SourceState state, DepositionSequenceController seq) GetSourceInfo(string sourceName)
        {
            return sourceName switch
            {
                "DC" => ((ushort)RegisterId.DC_Control_Command, (ushort)RegisterId.DC_Status, State.Dc, DcSequence),
                "RF1" => ((ushort)RegisterId.RF1_Control_Command, (ushort)RegisterId.RF1_Status, State.Rf1, Rf1Sequence),
                "RF2" => ((ushort)RegisterId.RF2_Control_Command, (ushort)RegisterId.RF2_Status, State.Rf2, Rf2Sequence),
                _ => throw new ArgumentException($"Geçersiz kaynak adı: {sourceName}", nameof(sourceName))
            };
        }

        private void SetLastStatus(string sourceName, short? status)
        {
            switch (sourceName)
            {
                case "RF1": _rf1LastStatus = status; break;
                case "RF2": _rf2LastStatus = status; break;
                case "DC": _dcLastStatus = status; break;
            }
        }

        private short? GetLastStatus(string sourceName)
        {
            return sourceName switch
            {
                "RF1" => _rf1LastStatus,
                "RF2" => _rf2LastStatus,
                "DC" => _dcLastStatus,
                _ => null
            };
        }

        private void CancelSourceWatcher(string sourceName)
        {
            switch (sourceName)
            {
                case "RF1":
                    try { _rf1WatchCts?.Cancel(); _rf1WatchCts?.Dispose(); } catch { }
                    _rf1WatchCts = null;
                    break;
                case "RF2":
                    try { _rf2WatchCts?.Cancel(); _rf2WatchCts?.Dispose(); } catch { }
                    _rf2WatchCts = null;
                    break;
                case "DC":
                    try { _dcWatchCts?.Cancel(); _dcWatchCts?.Dispose(); } catch { }
                    _dcWatchCts = null;
                    break;
            }
        }

        private void CancelAllSourceWatchers()
        {
            CancelSourceWatcher("RF1");
            CancelSourceWatcher("RF2");
            CancelSourceWatcher("DC");
            _rf1Transition = SourceTransition.None;
            _rf2Transition = SourceTransition.None;
            _dcTransition = SourceTransition.None;
        }

        private void SetWatchCts(string sourceName, CancellationTokenSource cts)
        {
            switch (sourceName)
            {
                case "RF1": _rf1WatchCts = cts; break;
                case "RF2": _rf2WatchCts = cts; break;
                case "DC": _dcWatchCts = cts; break;
            }
        }

        private async Task WriteSourceParametersAsync(string sourceName)
        {
            if (sourceName == "DC")
            {
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step1_SetW, (ushort)State.Dc.Step1.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step1_Ramp, (ushort)State.Dc.Step1.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step1_Dwell, (ushort)State.Dc.Step1.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step2_SetW, (ushort)State.Dc.Step2.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step2_Ramp, (ushort)State.Dc.Step2.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Step2_Dwell, (ushort)State.Dc.Step2.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_OffRamp, (ushort)State.Dc.OffRamp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Set_Voltage, (ushort)State.Dc.VoltageSet);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.DC_Set_Current, (ushort)State.Dc.CurrentSet);
            }
            else if (sourceName == "RF1")
            {
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step1_SetW, (ushort)State.Rf1.Step1.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step1_Ramp, (ushort)State.Rf1.Step1.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step1_Dwell, (ushort)State.Rf1.Step1.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step2_SetW, (ushort)State.Rf1.Step2.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step2_Ramp, (ushort)State.Rf1.Step2.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_Step2_Dwell, (ushort)State.Rf1.Step2.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF1_OffRamp, (ushort)State.Rf1.OffRamp);
            }
            else if (sourceName == "RF2")
            {
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step1_SetW, (ushort)State.Rf2.Step1.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step1_Ramp, (ushort)State.Rf2.Step1.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step1_Dwell, (ushort)State.Rf2.Step1.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step2_SetW, (ushort)State.Rf2.Step2.Setpoint);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step2_Ramp, (ushort)State.Rf2.Step2.Ramp);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_Step2_Dwell, (ushort)State.Rf2.Step2.Dwell);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RF2_OffRamp, (ushort)State.Rf2.OffRamp);
            }
        }

        private void StartSourceSequence(string sourceName)
        {
            switch (sourceName)
            {
                case "DC":
                    if (!DcSequence.IsRunning)
                    {
                        DcSequence.Start(
                            () => DepositionSequenceController.ComputeStep1Duration(State.Dc.Step1.Setpoint, State.Dc.Step1.Ramp, State.Dc.Step1.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationPlanned(State.Dc.Step1.Setpoint, State.Dc.Step2.Setpoint, State.Dc.Step2.Ramp, State.Dc.Step2.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationFromPower(double.TryParse(State.Dc.Read1, out var p) ? p : 0, State.Dc.Step2.Setpoint, State.Dc.Step2.Ramp, State.Dc.Step2.Dwell)
                        );
                    }
                    break;
                case "RF1":
                    if (!Rf1Sequence.IsRunning)
                    {
                        Rf1Sequence.Start(
                            () => DepositionSequenceController.ComputeStep1Duration(State.Rf1.Step1.Setpoint, State.Rf1.Step1.Ramp, State.Rf1.Step1.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationPlanned(State.Rf1.Step1.Setpoint, State.Rf1.Step2.Setpoint, State.Rf1.Step2.Ramp, State.Rf1.Step2.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationFromPower(double.TryParse(State.Rf1.Read1, out var p) ? p : 0, State.Rf1.Step2.Setpoint, State.Rf1.Step2.Ramp, State.Rf1.Step2.Dwell)
                        );
                    }
                    break;
                case "RF2":
                    if (!Rf2Sequence.IsRunning)
                    {
                        Rf2Sequence.Start(
                            () => DepositionSequenceController.ComputeStep1Duration(State.Rf2.Step1.Setpoint, State.Rf2.Step1.Ramp, State.Rf2.Step1.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationPlanned(State.Rf2.Step1.Setpoint, State.Rf2.Step2.Setpoint, State.Rf2.Step2.Ramp, State.Rf2.Step2.Dwell),
                            () => DepositionSequenceController.ComputeStep2DurationFromPower(double.TryParse(State.Rf2.Read1, out var p) ? p : 0, State.Rf2.Step2.Setpoint, State.Rf2.Step2.Ramp, State.Rf2.Step2.Dwell)
                        );
                    }
                    break;
            }
        }

        private void CancelSourceSequence(string sourceName)
        {
            switch (sourceName)
            {
                case "DC":
                    if (DcSequence.IsRunning) DcSequence.Cancel();
                    break;
                case "RF1":
                    if (Rf1Sequence.IsRunning) Rf1Sequence.Cancel();
                    break;
                case "RF2":
                    if (Rf2Sequence.IsRunning) Rf2Sequence.Cancel();
                    break;
            }
        }

        private void ApplySourceConfirmedState(string sourceName, bool isActive)
        {
            var (_, _, sourceState, _) = GetSourceInfo(sourceName);
            SetLastStatus(sourceName, (short)(isActive ? 1 : 0));

            RunOnUi(() =>
            {
                sourceState.IsActive = isActive;
                sourceState.Status = isActive ? "ON" : "OFF";
                if (!isActive) sourceState.UpdateIdlePreviews();
            });

            if (isActive)
            {
                StartSourceSequence(sourceName);
            }
            else
            {
                CancelSourceSequence(sourceName);
            }
        }

        private void PollSingleSourceStatus(string sourceName, ushort statusAddr, short currentStatus)
        {
            // 1) Eğer bu kaynak şu anda aktif bir başlatma veya durdurma geçişi bekliyorsa,
            // periyodik arka plan poller ASLA araya girip süreci bozmamalı!
            if (GetTransition(sourceName) != SourceTransition.None) return;

            short? last = GetLastStatus(sourceName);
            // 2) Değer değişmediyse hiçbir şey yapma!
            if (last == currentStatus) return;

            // 3) Gerçek harici PLC durum değişimi:
            if (currentStatus == 1)
            {
                ApplySourceConfirmedState(sourceName, true);
                LogInfo("SOURCE", $"[{sourceName}] PLC durum teyidi değişti: AÇIK (1). Proses devrede.");
            }
            else if (currentStatus == 0)
            {
                ApplySourceConfirmedState(sourceName, false);
                LogInfo("SOURCE", $"[{sourceName}] PLC durum teyidi değişti: KAPALI (0). Proses durduruldu.");
            }
            else
            {
                var (_, _, sourceState, _) = GetSourceInfo(sourceName);
                RunOnUi(() =>
                {
                    sourceState.IsActive = null;
                    sourceState.Status = "PENDING...";
                });
                SetLastStatus(sourceName, currentStatus);
            }
        }

        private async Task<bool> WaitForStatusAsync(string sourceName, ushort statusAddr, short expected, int timeoutMs)
        {
            CancelSourceWatcher(sourceName);

            var cts = new CancellationTokenSource();
            SetWatchCts(sourceName, cts);
            var ct = cts.Token;

            var sw = Stopwatch.StartNew();
            const int pollMs = 150;

            try
            {
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    ct.ThrowIfCancellationRequested();
                    if (!Modbus.IsConnected) return false;

                    try
                    {
                        var regs = await Modbus.ReadHoldingRegistersAsync(statusAddr, 1, ct);
                        if (regs != null && regs.Length > 0 && unchecked((short)regs[0]) == expected)
                        {
                            return true;
                        }
                    }
                    catch (OperationCanceledException) { throw; }
                    catch { }

                    await Task.Delay(pollMs, ct);
                }
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            return false;
        }

        public async Task StartSourceAsync(string sourceName)
        {
            if (!Modbus.IsConnected) return;

            string? blockReason = sourceName switch
            {
                "DC" => Interlocks.GetStartBlockReason("DC"),
                "RF1" => Interlocks.GetStartBlockReason("RF1"),
                "RF2" => Interlocks.GetStartBlockReason("RF2"),
                _ => null
            };

            if (blockReason != null)
            {
                MessageBox.Show(blockReason, "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var (cmdAddr, statusAddr, sourceState, _) = GetSourceInfo(sourceName);

            if (GetTransition(sourceName) != SourceTransition.None || sourceState.IsActive == true)
            {
                LogWarn("SOURCE", $"[{sourceName}] Başlatma yoksayıldı: Kaynak zaten aktif veya geçiş sürecinde.");
                return;
            }

            SetTransition(sourceName, SourceTransition.Starting);

            try
            {
                // 1) Parametreleri başlatma öncesi PLC'ye yaz
                await WriteSourceParametersAsync(sourceName);

                // 2) UI'ı geçiş durumuna (STARTING...) al
                RunOnUi(() =>
                {
                    sourceState.IsActive = null;
                    sourceState.Status = "STARTING...";
                });

                // 3) START komutunu PLC registerına yaz (DC=0, RF1=130, RF2=280 -> 1)
                await Modbus.WriteSingleRegisterAsync(cmdAddr, 1);
                LogInfo("SOURCE", $"[{sourceName}] START komutu yazıldı ({cmdAddr}=1). PLC durum teyidi ({statusAddr} -> 1) bekleniyor...");

                // 4) PLC donanım teyidini bekle (RF1_Status == 1)
                // Bu teyit gelene kadar SÜRE VE PROSES BAŞLAMAZ!
                bool confirmed = await WaitForStatusAsync(sourceName, statusAddr, expected: 1, timeoutMs: 30_000);

                SetTransition(sourceName, SourceTransition.None);

                if (confirmed)
                {
                    ApplySourceConfirmedState(sourceName, true);
                    LogInfo("SOURCE", $"[{sourceName}] PLC durum teyidi alındı ({statusAddr}=1). Süre ve proses başlatıldı.");
                }
                else
                {
                    ApplySourceConfirmedState(sourceName, false);
                    try { await Modbus.WriteSingleRegisterAsync(cmdAddr, 0); } catch { }
                    LogWarn("SOURCE", $"[{sourceName}] Başlatma zaman aşımı (30 sn). PLC durum ({statusAddr}) 1 olmadı, komut durduruldu.");
                }
            }
            catch (Exception ex)
            {
                SetTransition(sourceName, SourceTransition.None);
                LogError("SOURCE", $"[{sourceName}] Başlatma hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
                ApplySourceConfirmedState(sourceName, false);
            }
        }

        public async Task StopSourceAsync(string sourceName)
        {
            if (!Modbus.IsConnected) return;

            var (cmdAddr, statusAddr, sourceState, _) = GetSourceInfo(sourceName);

            if (GetTransition(sourceName) == SourceTransition.Stopping)
            {
                return;
            }

            // 1) Sayacı ve süreci derhal durdur
            CancelSourceSequence(sourceName);
            SetTransition(sourceName, SourceTransition.Stopping);

            try
            {
                // 2) UI'ı geçiş durumuna (STOPPING...) al
                RunOnUi(() =>
                {
                    sourceState.IsActive = null;
                    sourceState.Status = "STOPPING...";
                });

                // 3) STOP komutunu PLC registerına yaz (DC=0, RF1=130, RF2=280 -> 0)
                await Modbus.WriteSingleRegisterAsync(cmdAddr, 0);
                LogInfo("SOURCE", $"[{sourceName}] STOP komutu yazıldı ({cmdAddr}=0). PLC kapatma teyidi ({statusAddr} -> 0) bekleniyor...");

                // 4) PLC donanım teyidini bekle (RF1_Status == 0)
                // Bu süreçte status 1 olsa bile ASLA tekrar başlatılmaz!
                bool confirmed = await WaitForStatusAsync(sourceName, statusAddr, expected: 0, timeoutMs: 45_000);

                SetTransition(sourceName, SourceTransition.None);
                ApplySourceConfirmedState(sourceName, false);

                if (confirmed)
                {
                    LogInfo("SOURCE", $"[{sourceName}] PLC kapatma teyidi alındı ({statusAddr}=0). Kaynak tamamen durduruldu.");
                }
                else
                {
                    try { await Modbus.WriteSingleRegisterAsync(cmdAddr, 0); } catch { }
                    LogWarn("SOURCE", $"[{sourceName}] Kapatma teyidi zaman aşımı (45 sn). Güvenli OFF uygulandı.");
                }
            }
            catch (Exception ex)
            {
                SetTransition(sourceName, SourceTransition.None);
                LogError("SOURCE", $"[{sourceName}] Durdurma hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
                ApplySourceConfirmedState(sourceName, false);
            }
        }

        public async Task ToggleValveAsync(int mfcId)
        {
            if (!Modbus.IsConnected) return;

            ushort writeAddr = mfcId switch
            {
                1 => (ushort)RegisterId.D1WriteValvePosition,
                2 => (ushort)RegisterId.D2WriteValvePosition,
                3 => (ushort)RegisterId.D3WriteValvePosition,
                4 => (ushort)RegisterId.D4WriteValvePosition,
                _ => (ushort)RegisterId.D5WriteValvePosition
            };

            bool current = mfcId switch
            {
                1 => State.Rf2.Argon.Valve.IsOpen == true,
                2 => State.Rf2.Oxygen.Valve.IsOpen == true,
                3 => State.Rf1.Argon.Valve.IsOpen == true,
                4 => State.Dc.Argon.Valve.IsOpen == true,
                _ => State.Dc.Oxygen.Valve.IsOpen == true
            };

            ushort target = (ushort)(current ? 2 : 1); // 1=Open, 2=Close

            try
            {
                await Modbus.WriteSingleRegisterAsync(writeAddr, target);
                bool isOpen = target == 1;

                RunOnUi(() =>
                {
                    switch (mfcId)
                    {
                        case 1: State.Rf2.Argon.Valve.IsOpen = isOpen; break;
                        case 2: State.Rf2.Oxygen.Valve.IsOpen = isOpen; break;
                        case 3: State.Rf1.Argon.Valve.IsOpen = isOpen; break;
                        case 4: State.Dc.Argon.Valve.IsOpen = isOpen; break;
                        case 5: State.Dc.Oxygen.Valve.IsOpen = isOpen; break;
                    }
                });

                LogInfo("MFC", $"MFC {mfcId} Valf -> {(isOpen ? "AÇIK" : "KAPALI")}");
            }
            catch (Exception ex)
            {
                LogError("MFC", $"Valf komut hatası (MFC {mfcId}): {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        public async Task SetMfcSetpointAsync(int mfcId, float targetFlow)
        {
            if (!Modbus.IsConnected) return;

            ushort writeAddr = mfcId switch
            {
                1 => (ushort)RegisterId.D1WriteSetPoint,
                2 => (ushort)RegisterId.D2WriteSetPoint,
                3 => (ushort)RegisterId.D3WriteSetPoint,
                4 => (ushort)RegisterId.D4WriteSetPoint,
                _ => (ushort)RegisterId.D5WriteSetPoint
            };

            try
            {
                var regs = ModbusTypeExtensions.ToRegisters_CDAB(targetFlow);
                await Modbus.WriteMultipleRegistersAsync(writeAddr, regs);
                LogInfo("MFC", $"MFC {mfcId} Setpoint -> {targetFlow:0.##} sccm");
            }
            catch (Exception ex)
            {
                LogError("MFC", $"MFC {mfcId} Setpoint yazma hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        public void StartQcmPolling()
        {
            if (_qcmPollingCts != null) return;
            _qcmPollingCts = new CancellationTokenSource();
            var token = _qcmPollingCts.Token;
            _ = Task.Run(() => QcmPollingLoopAsync(token), token);
        }

        public void StopQcmPolling()
        {
            try
            {
                _qcmPollingCts?.Cancel();
                _qcmPollingCts?.Dispose();
            }
            catch { }
            _qcmPollingCts = null;
        }

        private async Task QcmPollingLoopAsync(CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!await timer.WaitForNextTickAsync(ct)) break;
                    await PollStm2SensorsAsync();
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[QcmPolling] Hata: {ex.Message}");
                }
            }
        }

        private void UpdateSensorMetadata(string source, string port, string serialNo, bool isConnected)
        {
            var s = Preview.GetSensorBySource(source);
            if (s != null)
            {
                RunOnUi(() =>
                {
                    s.Port = port;
                    s.SerialNo = serialNo;
                    s.IsConnected = isConnected;
                });
            }
        }

        private void ResetAllSensorsMetadata()
        {
            RunOnUi(() =>
            {
                foreach (var s in Preview.Sensors)
                {
                    s.Reset();
                }
            });
        }

        public async Task AutoConnectStm2BySerialAsync()
        {
            DisposeStm2Clients();

            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length == 0)
            {
                StartQcmPolling();
                UpdateQcmStatusSummary();
                LogWarn("QCM", "Sistemde kullanılabilir seri (COM) port bulunamadı (sensör takıldığında otomatik algılanacaktır).");
                return;
            }

            LogInfo("QCM", $"COM portlar paralel taranıyor: {string.Join(", ", portNames)}...");
            RunOnUi(() => State.QcmStatusText = "QCM: Portlar taranıyor...");

            // Tüm COM portları PARALEL tara (Hızlı algılama: < 1 saniye)
            var probeTasks = portNames.Select(async port =>
            {
                Stm2Client? client = null;
                try
                {
                    client = new Stm2Client(port);
                    client.Open();
                    // Port stabilizasyonu
                    await Task.Delay(100).ConfigureAwait(false);

                    // 1) Seri numarasını oku (@A komutu)
                    string? rawSn = await client.TryGetSNAsync(700).ConfigureAwait(false);
                    string sn = "";

                    if (!string.IsNullOrWhiteSpace(rawSn))
                    {
                        var m = Regex.Match(rawSn, @"\b\d{6}\b");
                        sn = m.Success ? m.Value : rawSn.Trim();
                    }

                    // 2) Eğer SN dönmediyse Ping (@) dene
                    if (string.IsNullOrWhiteSpace(sn))
                    {
                        string? ping = await client.TryPingAsync(500).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(ping))
                        {
                            var m = Regex.Match(ping, @"\b\d{6}\b");
                            sn = m.Success ? m.Value : "STM2";
                        }
                    }

                    // 3) Eğer yine boşsa Thickness (S) komutu dene
                    if (string.IsNullOrWhiteSpace(sn))
                    {
                        string? thk = await client.TryGetThicknessAsync(500).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(thk))
                        {
                            sn = "STM2";
                        }
                    }

                    // 4) 115200'de yanıt alınamadıysa INFICON fabrika varsayılanı olan 9600 baud dene
                    if (string.IsNullOrWhiteSpace(sn))
                    {
                        try
                        {
                            client.Close();
                            client.Dispose();
                            client = new Stm2Client(port, 9600);
                            client.Open();
                            await Task.Delay(80).ConfigureAwait(false);

                            rawSn = await client.TryGetSNAsync(500).ConfigureAwait(false);
                            if (!string.IsNullOrWhiteSpace(rawSn))
                            {
                                var m = Regex.Match(rawSn, @"\b\d{6}\b");
                                sn = m.Success ? m.Value : rawSn.Trim();
                            }
                            if (string.IsNullOrWhiteSpace(sn))
                            {
                                string? ping = await client.TryPingAsync(400).ConfigureAwait(false);
                                if (!string.IsNullOrWhiteSpace(ping))
                                {
                                    var m = Regex.Match(ping, @"\b\d{6}\b");
                                    sn = m.Success ? m.Value : "STM2";
                                }
                            }
                        }
                        catch { }
                    }

                    if (!string.IsNullOrWhiteSpace(sn))
                    {
                        client.SerialNumber = sn;
                        // STM-2 bulundu! Açık portu koru, tekrar kapatıp açma hatasından kaçın
                        return (port, client, sn);
                    }

                    // Yanıt yoksa bu portta STM-2 yok, kapat
                    client.Dispose();
                    return (port, client: (Stm2Client?)null, sn: "");
                }
                catch
                {
                    try { client?.Dispose(); } catch { }
                    return (port, client: (Stm2Client?)null, sn: "");
                }
            });

            var probeResults = await Task.WhenAll(probeTasks).ConfigureAwait(false);
            var discovered = probeResults
                .Where(r => r.client != null && !string.IsNullOrWhiteSpace(r.sn))
                .Select(r => (port: r.port, client: r.client!, sn: r.sn))
                .ToList();

            int foundCount = 0;

            // 1) Bilinen seri numaralarını eşle (RF1: 700950, RF2: 700918, DC: 700969)
            var rf1Match = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_RF1));
            if (rf1Match.client != null)
            {
                _stmRf1 = rf1Match.client;
                _stmRf1.SerialNumber = STM2_SN_RF1;
                foundCount++;
                UpdateSensorMetadata("RF1", rf1Match.port, STM2_SN_RF1, true);
                LogInfo("QCM", $"RF1 STM-2 sensörü bağlandı ({rf1Match.port}, SN: {STM2_SN_RF1})");
                discovered.Remove(rf1Match);
            }

            var rf2Match = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_RF2));
            if (rf2Match.client != null)
            {
                _stmRf2 = rf2Match.client;
                _stmRf2.SerialNumber = STM2_SN_RF2;
                foundCount++;
                UpdateSensorMetadata("RF2", rf2Match.port, STM2_SN_RF2, true);
                LogInfo("QCM", $"RF2 STM-2 sensörü bağlandı ({rf2Match.port}, SN: {STM2_SN_RF2})");
                discovered.Remove(rf2Match);
            }

            var dcMatch = discovered.FirstOrDefault(d => d.sn.Contains(STM2_SN_DC));
            if (dcMatch.client != null)
            {
                _stmDc = dcMatch.client;
                _stmDc.SerialNumber = STM2_SN_DC;
                foundCount++;
                UpdateSensorMetadata("DC", dcMatch.port, STM2_SN_DC, true);
                LogInfo("QCM", $"DC STM-2 sensörü bağlandı ({dcMatch.port}, SN: {STM2_SN_DC})");
                discovered.Remove(dcMatch);
            }

            // 2) Kalan / Test amaçlı takılan STM-2 sensörlerini boş slotlara ata (RF1 -> RF2 -> DC)
            foreach (var item in discovered.ToList())
            {
                string? targetSlot = null;
                if (_stmRf1 == null) targetSlot = "RF1";
                else if (_stmRf2 == null) targetSlot = "RF2";
                else if (_stmDc == null) targetSlot = "DC";

                if (targetSlot == null)
                {
                    // 3 slot da dolu, fazla cihazı kapat
                    try { item.client.Dispose(); } catch { }
                    continue;
                }

                if (targetSlot == "RF1") _stmRf1 = item.client;
                else if (targetSlot == "RF2") _stmRf2 = item.client;
                else _stmDc = item.client;

                foundCount++;
                UpdateSensorMetadata(targetSlot, item.port, item.sn, true);
                LogInfo("QCM", $"STM-2 sensörü bulundu ({item.port}, SN: {item.sn}) -> '{targetSlot}' kaynağına bağlandı.");
                discovered.Remove(item);
            }

            StartQcmPolling();
            UpdateQcmStatusSummary();

            if (foundCount > 0)
            {
                LogInfo("QCM", $"QCM taraması tamamlandı: {foundCount} sensör bağlandı ve okuma başlatıldı.");
            }
            else
            {
                LogWarn("QCM", "Cevap veren STM-2 QCM sensörü bulunamadı (sensör takıldığında otomatik algılanacaktır).");
            }
        }

        public async Task<bool> ManualConnectStm2Async(string port, string targetSource = "RF1")
        {
            Stm2Client? client = null;
            try
            {
                client = new Stm2Client(port);
                client.Open();
                await Task.Delay(100);

                string? rawSn = await client.TryGetSNAsync(800);
                string sn = "";
                if (!string.IsNullOrWhiteSpace(rawSn))
                {
                    var m = Regex.Match(rawSn, @"\b\d{6}\b");
                    sn = m.Success ? m.Value : rawSn.Trim();
                }

                if (string.IsNullOrWhiteSpace(sn))
                {
                    var ping = await client.TryPingAsync(600);
                    if (!string.IsNullOrWhiteSpace(ping))
                    {
                        var m = Regex.Match(ping, @"\b\d{6}\b");
                        sn = m.Success ? m.Value : "STM2";
                    }
                }

                if (string.IsNullOrWhiteSpace(sn))
                {
                    var thk = await client.TryGetThicknessAsync(600);
                    if (!string.IsNullOrWhiteSpace(thk)) sn = "STM2";
                }

                if (string.IsNullOrWhiteSpace(sn)) sn = "STM2-MANUAL";

                client.SerialNumber = sn.Trim();

                switch (targetSource.ToUpperInvariant())
                {
                    case "RF1":
                        try { _stmRf1?.Dispose(); } catch { }
                        _stmRf1 = client;
                        break;
                    case "RF2":
                        try { _stmRf2?.Dispose(); } catch { }
                        _stmRf2 = client;
                        break;
                    default:
                        try { _stmDc?.Dispose(); } catch { }
                        _stmDc = client;
                        break;
                }

                UpdateSensorMetadata(targetSource, port, sn.Trim(), true);
                StartQcmPolling();
                UpdateQcmStatusSummary();
                LogInfo("QCM", $"Manuel bağlantı başarılı: {port} -> {targetSource} (SN: {sn.Trim()})");
                return true;
            }
            catch (Exception ex)
            {
                try { client?.Dispose(); } catch { }
                LogError("QCM", $"Manuel bağlantı hatası ({port}): {ex.Message}");
                return false;
            }
        }

        public void DisposeStm2Clients()
        {
            try { _stmRf1?.Dispose(); } catch { }
            try { _stmRf2?.Dispose(); } catch { }
            try { _stmDc?.Dispose(); } catch { }
            _stmRf1 = null;
            _stmRf2 = null;
            _stmDc = null;
            _failCountRf1 = 0;
            _failCountRf2 = 0;
            _failCountDc = 0;

            ResetAllSensorsMetadata();
            UpdateQcmStatusSummary();
        }

        private static string FormatSci(float value)
        {
            if (value <= 0 || float.IsNaN(value) || float.IsInfinity(value)) return "—";

            float abs = Math.Abs(value);
            int exponent = (int)Math.Floor(Math.Log10(abs));
            double mantissa = abs / Math.Pow(10, exponent);

            // Mantissa sınır düzeltmesi (yuvarlama sonucu 10.00 olmaması için)
            if (mantissa >= 9.995)
            {
                mantissa /= 10.0;
                exponent++;
            }
            else if (mantissa < 0.9995 && mantissa > 0)
            {
                mantissa *= 10.0;
                exponent--;
            }

            string expSup = ToSuperscript(exponent);
            return $"{mantissa.ToString("0.00", CultureInfo.InvariantCulture)} × 10{expSup}";
        }

        private static string ToSuperscript(int number)
        {
            const string superscripts = "⁰¹²³⁴⁵⁶⁷⁸⁹";
            string s = number.ToString(CultureInfo.InvariantCulture);
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '-') sb.Append('⁻');
                else if (c == '+') sb.Append('⁺');
                else if (char.IsDigit(c)) sb.Append(superscripts[c - '0']);
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static string FormatAngstromToKa(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "—";
            var clean = raw.Trim().Replace(',', '.');
            if (float.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                return (v / 1000.0f).ToString("0.00", CultureInfo.InvariantCulture);
            return "—";
        }

        private static string ParseOrDash(string s) => string.IsNullOrWhiteSpace(s) ? "—" : s.Trim();

        private void RunOnUi(Action action)
        {
            if (Application.Current?.Dispatcher != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                    action();
                else
                    Application.Current.Dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void LogInfo(string code, string message) =>
            Logger.Publish(new LogEvent { Level = LogLevel.Info, Code = code, Message = message, RegistrationNo = _registrationNo });

        private void LogWarn(string code, string message) =>
            Logger.Publish(new LogEvent { Level = LogLevel.Warn, Code = code, Message = message, RegistrationNo = _registrationNo });

        private void LogError(string code, string message) =>
            Logger.Publish(new LogEvent { Level = LogLevel.Error, Code = code, Message = message, RegistrationNo = _registrationNo });

        public async Task SetOrdelTemperatureAsync(decimal temp)
        {
            if (!Modbus.IsConnected) return;
            try
            {
                ushort val = (ushort)(temp * 10m);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.Ordel_SetValue, val);
                RunOnUi(() => State.HeaterSet = temp);
                LogInfo("PLC", $"Rezistans hedef sıcaklık ayarlandı: {temp:0.0} °C");
            }
            catch (Exception ex)
            {
                LogError("PLC", $"Rezistans sıcaklık ayarlama hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        public async Task SetRotationMotorAsync(decimal rpm)
        {
            if (!Modbus.IsConnected) return;
            try
            {
                ushort val = (ushort)rpm;
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.RotationMotor_Rpm, val);
                RunOnUi(() => State.RotationSet = rpm);
                LogInfo("PLC", $"Rotation Motor RPM ayarlandı: {rpm} rpm");
            }
            catch (Exception ex)
            {
                LogError("PLC", $"Rotation Motor RPM ayarlama hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        public async Task SetGateValvePositionAsync(decimal pos)
        {
            if (!Modbus.IsConnected) return;
            try
            {
                short targetPos = (short)Math.Clamp((int)pos, 0, 100);
                short regTargetPos = targetPos == 100 ? (short)132 : targetPos;
                short currentPos = (short)State.GateValve.Position;

                // 1) Donanım uyumlu hız hesaplama ve D153'e yazma
                short speed = CalculateGateValveSpeed(currentPos, regTargetPos, targetPos);
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.GateValve_Speed, unchecked((ushort)speed));

                // 2) Hedef konumu D150'ye yazma
                await Modbus.WriteSingleRegisterAsync((ushort)RegisterId.GateValve_Position, unchecked((ushort)regTargetPos));

                // 3) UI iyimser durum güncellemesi (500ms içinde PLC polling kesin teyit edecektir)
                RunOnUi(() =>
                {
                    State.GateSet = targetPos;
                    if (targetPos > currentPos)
                    {
                        State.GateValve.StatusText = $"AÇILIYOR (%{currentPos})";
                        State.GateValve.StateKind = "Opening";
                        State.GateValve.IsMoving = true;
                        State.GateValve.IsOpen = true;
                    }
                    else if (targetPos < currentPos)
                    {
                        State.GateValve.StatusText = $"KAPANIYOR (%{currentPos})";
                        State.GateValve.StateKind = "Closing";
                        State.GateValve.IsMoving = true;
                        State.GateValve.IsOpen = currentPos > 0;
                    }
                    else
                    {
                        if (targetPos == 0)
                        {
                            State.GateValve.StatusText = "KAPALI";
                            State.GateValve.StateKind = "Closed";
                            State.GateValve.IsMoving = false;
                            State.GateValve.IsOpen = false;
                        }
                        else
                        {
                            State.GateValve.StatusText = "AÇIK";
                            State.GateValve.StateKind = "Open";
                            State.GateValve.IsMoving = false;
                            State.GateValve.IsOpen = true;
                        }
                    }
                });

                LogInfo("PLC", $"Gate Valve hedef ayarlandı: %{targetPos} (Hız: {speed})");
            }
            catch (Exception ex)
            {
                LogError("PLC", $"Gate Valve pozisyon ayarlama hatası: {ex.Message}");
                CheckAndHandleDisconnection(ex);
            }
        }

        private static short CalculateGateValveSpeed(short currentPosition, short registerTargetPosition, short targetPosition)
        {
            if (targetPosition is 0 or 100)
                return 80;

            if (targetPosition is >= 1 and <= 35)
                return 8;

            int positionDifference = Math.Abs((int)registerTargetPosition - currentPosition);

            if (positionDifference is >= 1 and <= 4)
                return 10;

            return 32;
        }

        private Stm2Client? GetClientForSource(string source)
        {
            var client = source?.ToUpperInvariant() switch
            {
                "RF1" => _stmRf1,
                "RF2" => _stmRf2,
                _ => _stmDc
            };

            if (client?.IsOpen == true) return client;

            // Tekli sensör test modu: Sistemde sadece 1 adet açık sensör varsa o sensörü kullan
            var openClients = new[] { _stmRf1, _stmRf2, _stmDc }.Where(c => c?.IsOpen == true).ToList();
            if (openClients.Count == 1) return openClients[0];

            return null;
        }

        public async Task ResetSensorThicknessAsync(string source)
        {
            var client = GetClientForSource(source);

            if (client?.IsOpen != true)
            {
                LogWarn("QCM", $"{source} sensörü bağlı değil, kalınlık sıfırlanamadı.");
                return;
            }

            try
            {
                await client.ZeroThicknessAsync();
                var s = Preview.GetSensorBySource(source);
                if (s != null)
                {
                    RunOnUi(() => s.Thickness = "0.00");
                }
                LogInfo("QCM", $"{source} sensör kalınlığı sıfırlandı.");
            }
            catch (Exception ex)
            {
                LogError("QCM", $"{source} kalınlık sıfırlama hatası: {ex.Message}");
            }
        }

        public async Task ConfigureStm2ParametersAsync(string source, float density, float zratio, float tooling)
        {
            var client = GetClientForSource(source);

            if (client?.IsOpen == true)
            {
                try
                {
                    await client.WriteDensityAsync(density);
                    await client.WriteZRatioAsync(zratio);
                    await client.WriteToolingAsync(tooling);
                    await client.ZeroThicknessAsync();
                    LogInfo("QCM", $"[{source}] STM-2 parametreleri yazıldı: Density={density:0.00}, ZRatio={zratio:0.000}, Tooling={tooling:0.0}%. Kalınlık sıfırlandı.");
                }
                catch (Exception ex)
                {
                    LogError("QCM", $"[{source}] STM-2 parametre yazma hatası: {ex.Message}");
                }
            }
            else
            {
                LogWarn("QCM", $"[{source}] STM-2 sensörü bağlı değil, parametreler donanıma yazılamadı.");
            }
        }

        public async Task StartLayerExecutionAsync(LayerRow layer)
        {
            if (!Interlocks.IsChillerOn)
            {
                throw new InvalidOperationException("Chiller kapalı! Katman başlatılabilmesi için Chiller cihazının çalışıyor (START) olması gereklidir.");
            }

            var mats = await AppDataManager.LoadMaterialsAsync();
            var procs = await AppDataManager.LoadProcessesAsync();
            var proc = procs.FirstOrDefault(p => p.Id == layer.Id || p.Name.Equals(layer.Process, StringComparison.OrdinalIgnoreCase));
            var mat = proc != null ? mats.FirstOrDefault(m => m.Id == proc.MaterialId || m.Name.Equals(layer.Material, StringComparison.OrdinalIgnoreCase))
                                   : mats.FirstOrDefault(m => m.Name.Equals(layer.Material, StringComparison.OrdinalIgnoreCase));

            float dens = mat?.Density ?? 1.0f;
            float zr = mat?.ZRatio ?? 1.0f;
            float tool = proc?.Tooling ?? 100.0f;

            // 1) QCM STM-2 sensörüne parametreleri yaz ve kalınlığı sıfırla
            await ConfigureStm2ParametersAsync(layer.Source, dens, zr, tool);

            // 2) İlgili kaynak için prosesi başlat
            await StartSourceAsync(layer.Source);

            // 3) Çalışma durumunu kaydet
            ActiveRunningLayer = layer;
            RunOnUi(() =>
            {
                UpdateLayerStatusInPreview(layer, "RUNNING");
                LayerRunningStateChanged?.Invoke(true, layer);
            });

            LogInfo("QCM", $"Layer başlatıldı: Proses='{layer.Process}', Kaynak={layer.Source}, Hedef={layer.Thickness} kÅ");
        }

        public async Task StopLayerExecutionAsync(bool manual = false)
        {
            var layer = ActiveRunningLayer;
            if (layer == null) return;

            ActiveRunningLayer = null;

            try
            {
                await StopSourceAsync(layer.Source);
            }
            catch (Exception ex)
            {
                LogError("QCM", $"Layer durdurulurken hata: {ex.Message}");
            }

            RunOnUi(() =>
            {
                UpdateLayerStatusInPreview(layer, manual ? "STOPPED" : "DONE");
                LayerRunningStateChanged?.Invoke(false, layer);
            });

            LogInfo("QCM", $"Layer durduruldu ({(manual ? "Manuel" : "Otomatik / Hedef Kalınlık")}): Proses='{layer.Process}'");
        }

        private void UpdateLayerStatusInPreview(LayerRow targetLayer, string status)
        {
            for (int i = 0; i < Preview.Layers.Count; i++)
            {
                if (Preview.Layers[i].Id == targetLayer.Id || (Preview.Layers[i].Number == targetLayer.Number && Preview.Layers[i].Process == targetLayer.Process))
                {
                    Preview.Layers[i] = Preview.Layers[i] with { Status = status };
                    if (Preview.SelectedLayer?.Id == targetLayer.Id)
                    {
                        Preview.SelectedLayer = Preview.Layers[i];
                    }
                    break;
                }
            }
        }

        public async Task SaveMaterialAsync(MaterialRow row)
        {
            var mats = await AppDataManager.LoadMaterialsAsync().ConfigureAwait(false);
            var existing = mats.FirstOrDefault(m => m.Id == row.Id);
            float.TryParse(row.Density, NumberStyles.Any, CultureInfo.InvariantCulture, out var dens);
            float.TryParse(row.ZRatio, NumberStyles.Any, CultureInfo.InvariantCulture, out var zr);

            if (existing != null)
            {
                existing.Name = row.Name;
                existing.Density = dens;
                existing.ZRatio = zr;
            }
            else
            {
                mats.Add(new MaterialItem
                {
                    Id = string.IsNullOrWhiteSpace(row.Id) || row.Id == "—" ? Guid.NewGuid().ToString() : row.Id,
                    Name = row.Name,
                    Density = dens,
                    ZRatio = zr
                });
            }

            await AppDataManager.SaveMaterialsAsync(mats).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteMaterialAsync(string id)
        {
            var procs = await AppDataManager.LoadProcessesAsync().ConfigureAwait(false);
            if (procs.Any(p => p.MaterialId == id))
            {
                RunOnUi(() => MessageBox.Show("Bu materyal Process listesinde kullanılıyor. Silinemez.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning));
                return;
            }

            var mats = await AppDataManager.LoadMaterialsAsync().ConfigureAwait(false);
            mats.RemoveAll(m => m.Id == id);
            await AppDataManager.SaveMaterialsAsync(mats).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task SaveProcessAsync(ProcessRow row)
        {
            var mats = await AppDataManager.LoadMaterialsAsync().ConfigureAwait(false);
            var procs = await AppDataManager.LoadProcessesAsync().ConfigureAwait(false);
            var mat = mats.FirstOrDefault(m => m.Name.Equals(row.Material, StringComparison.OrdinalIgnoreCase)) ?? mats.FirstOrDefault();

            var toolingClean = row.Tooling.Replace("%", "").Trim();
            float.TryParse(toolingClean, NumberStyles.Any, CultureInfo.InvariantCulture, out var toolVal);

            var existing = procs.FirstOrDefault(p => p.Id == row.Id);
            if (existing != null)
            {
                existing.Name = row.Name;
                existing.MaterialId = mat?.Id ?? "";
                existing.MaterialName = mat?.Name ?? row.Material ?? "";
                existing.Tooling = toolVal;
            }
            else
            {
                procs.Add(new ProcessItem
                {
                    Id = string.IsNullOrWhiteSpace(row.Id) || row.Id == "—" ? Guid.NewGuid().ToString() : row.Id,
                    Name = row.Name,
                    MaterialId = mat?.Id ?? "",
                    MaterialName = mat?.Name ?? row.Material ?? "",
                    Tooling = toolVal
                });
            }

            await AppDataManager.SaveProcessesAsync(procs).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteProcessAsync(string id)
        {
            var layers = await AppDataManager.LoadLayersAsync().ConfigureAwait(false);
            if (layers.Any(l => l.ProcessId == id))
            {
                RunOnUi(() => MessageBox.Show("Bu proses Layer listesinde kullanılıyor. Silinemez.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning));
                return;
            }

            var procs = await AppDataManager.LoadProcessesAsync().ConfigureAwait(false);
            procs.RemoveAll(p => p.Id == id);
            await AppDataManager.SaveProcessesAsync(procs).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task SaveLayerAsync(LayerRow row)
        {
            var procs = await AppDataManager.LoadProcessesAsync().ConfigureAwait(false);
            var layers = await AppDataManager.LoadLayersAsync().ConfigureAwait(false);
            var proc = procs.FirstOrDefault(p => p.Name.Equals(row.Process, StringComparison.OrdinalIgnoreCase)) ?? procs.FirstOrDefault();

            float.TryParse(row.Thickness, NumberStyles.Any, CultureInfo.InvariantCulture, out var thk);
            Enum.TryParse<DepositionSource>(row.Source, true, out var src);

            bool rf1 = string.Equals(row.Sensor, "RF1", StringComparison.OrdinalIgnoreCase) || (string.IsNullOrWhiteSpace(row.Sensor) && src == DepositionSource.RF1);
            bool rf2 = string.Equals(row.Sensor, "RF2", StringComparison.OrdinalIgnoreCase) || (string.IsNullOrWhiteSpace(row.Sensor) && src == DepositionSource.RF2);
            bool dc = string.Equals(row.Sensor, "DC", StringComparison.OrdinalIgnoreCase) || (string.IsNullOrWhiteSpace(row.Sensor) && src == DepositionSource.DC);

            var existing = layers.FirstOrDefault(l => l.Id == row.Id);
            if (existing != null)
            {
                existing.ProcessId = proc?.Id ?? "";
                existing.FinalThickness = thk;
                existing.Source = src;
                existing.RF1SensorEnabled = rf1;
                existing.RF2SensorEnabled = rf2;
                existing.DCSensorEnabled = dc;
            }
            else
            {
                layers.Add(new LayerItem
                {
                    Id = string.IsNullOrWhiteSpace(row.Id) || row.Id == "—" ? Guid.NewGuid().ToString() : row.Id,
                    ProcessId = proc?.Id ?? "",
                    FinalThickness = thk,
                    Source = src,
                    RF1SensorEnabled = rf1,
                    RF2SensorEnabled = rf2,
                    DCSensorEnabled = dc
                });
            }

            await AppDataManager.SaveLayersAsync(layers).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteLayerAsync(string id)
        {
            var layers = await AppDataManager.LoadLayersAsync().ConfigureAwait(false);
            layers.RemoveAll(l => l.Id == id);
            await AppDataManager.SaveLayersAsync(layers).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);
        }

        public async Task SaveTargetAsync(TargetRow row)
        {
            var targets = await AppDataManager.LoadTargetsAsync().ConfigureAwait(false);
            var cleanWh = (row.UsedWh ?? "").Trim().Replace(',', '.');
            decimal.TryParse(cleanWh, NumberStyles.Any, CultureInfo.InvariantCulture, out var used);

            var existing = targets.FirstOrDefault(t => (!string.IsNullOrWhiteSpace(row.Id) && t.Id == row.Id)
                                                    || string.Equals(t.Name, row.Name, StringComparison.OrdinalIgnoreCase));
            string? oldName = existing?.Name;

            if (existing != null)
            {
                existing.Name = row.Name.Trim();
                existing.Source = string.IsNullOrWhiteSpace(row.Source) ? "—" : row.Source.Trim();
                existing.UsedWh = used;
            }
            else
            {
                targets.Add(new TargetItem
                {
                    Id = string.IsNullOrWhiteSpace(row.Id) || row.Id == "—" ? Guid.NewGuid().ToString() : row.Id,
                    Name = row.Name.Trim(),
                    Source = string.IsNullOrWhiteSpace(row.Source) ? "—" : row.Source.Trim(),
                    UsedWh = used
                });
            }

            await AppDataManager.SaveTargetsAsync(targets).ConfigureAwait(false);
            await LoadAllStorageDataAsync().ConfigureAwait(false);

            // DC, RF1, RF2 aktif kaynaklarında kullanılan hedef varsa eşitle
            RunOnUi(() =>
            {
                string syncOld = !string.IsNullOrWhiteSpace(oldName) ? oldName : row.Name;
                if (string.Equals(State.Dc.Target, syncOld, StringComparison.OrdinalIgnoreCase))
                {
                    State.Dc.Target = row.Name.Trim();
                    State.Dc.UsedWh = used.ToString("0.00");
                }
                if (string.Equals(State.Rf1.Target, syncOld, StringComparison.OrdinalIgnoreCase))
                {
                    State.Rf1.Target = row.Name.Trim();
                    State.Rf1.UsedWh = used.ToString("0.00");
                }
                if (string.Equals(State.Rf2.Target, syncOld, StringComparison.OrdinalIgnoreCase))
                {
                    State.Rf2.Target = row.Name.Trim();
                    State.Rf2.UsedWh = used.ToString("0.00");
                }
            });
        }

        public async Task DeleteTargetAsync(string id, string name = "")
        {
            var targets = await AppDataManager.LoadTargetsAsync().ConfigureAwait(false);
            var toRemove = targets.FirstOrDefault(t => (!string.IsNullOrWhiteSpace(id) && t.Id == id)
                                                   || (!string.IsNullOrWhiteSpace(name) && string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)));
            if (toRemove != null)
            {
                string removedName = toRemove.Name;
                targets.Remove(toRemove);
                await AppDataManager.SaveTargetsAsync(targets).ConfigureAwait(false);
                await LoadAllStorageDataAsync().ConfigureAwait(false);

                // Silinen target aktif bir kaynaktaysa kaynağı boşa çıkar
                RunOnUi(() =>
                {
                    if (string.Equals(State.Dc.Target, removedName, StringComparison.OrdinalIgnoreCase))
                    {
                        State.Dc.Target = "—";
                        State.Dc.UsedWh = "0.00";
                    }
                    if (string.Equals(State.Rf1.Target, removedName, StringComparison.OrdinalIgnoreCase))
                    {
                        State.Rf1.Target = "—";
                        State.Rf1.UsedWh = "0.00";
                    }
                    if (string.Equals(State.Rf2.Target, removedName, StringComparison.OrdinalIgnoreCase))
                    {
                        State.Rf2.Target = "—";
                        State.Rf2.UsedWh = "0.00";
                    }
                });
            }
        }

        public void Dispose()
        {
            StopQcmPolling();
            DisposeStm2Clients();
            Disconnect();
        }
    }
}
