using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SputterNewApp.Models
{
    public abstract class ObservableState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }

    public sealed class SourceState : ObservableState
    {
        public string Name { get; }
        public bool IsDc => Name == "DC";
        public bool HasOxygen => Name != "RF1";
        public string SettingsSizeGroup => IsDc ? "DcSettings" : "RfSettings";
        public string SetUnit => IsDc ? "Set (mA)" : "Set (W)";
        public string RampUnit => IsDc ? "Ramp (mA/s)" : "Ramp (W/s)";

        private string _target;
        private string _life;
        public string Target { get => _target; set => Set(ref _target, value); }
        public string UsedWh { get => _life; set => Set(ref _life, value); }

        public StepState Step1 { get; } = new(10, 1, 1);
        public StepState Step2 { get; } = new(100, 1, 5);

        private decimal _offRamp = 1, _voltage = 200, _current = 50, _power = 100;
        public decimal OffRamp { get => _offRamp; set => Set(ref _offRamp, value); }
        public decimal VoltageSet { get => _voltage; set => Set(ref _voltage, value); }
        public decimal CurrentSet { get => _current; set => Set(ref _current, value); }
        public decimal PowerSet { get => _power; set => Set(ref _power, value); }

        public GasState Argon { get; } = new();
        public GasState Oxygen { get; } = new();

        private string _read1 = "—", _read2 = "—", _read3 = "—", _status = "OFF";
        public string Read1 { get => _read1; set => Set(ref _read1, value); }
        public string Read2 { get => _read2; set => Set(ref _read2, value); }
        public string Read3 { get => _read3; set => Set(ref _read3, value); }
        public string Status
        {
            get => _status;
            set
            {
                if (Set(ref _status, value))
                {
                    Notify(nameof(ToggleButtonIcon));
                    Notify(nameof(ToggleButtonText));
                    Notify(nameof(ToggleButtonTooltip));
                }
            }
        }

        public string ReadLabel1 => IsDc ? "Power (W)" : "Forward (W)";
        public string ReadLabel2 => IsDc ? "Voltage (V)" : "Reflected (W)";
        public string ReadLabel3 => IsDc ? "Current (mA)" : "Set (W)";

        private bool? _isActive = false;
        public bool? IsActive
        {
            get => _isActive;
            set
            {
                if (Set(ref _isActive, value))
                {
                    Notify(nameof(IsStep1Enabled));
                    Notify(nameof(CanStart));
                    Notify(nameof(CanStop));
                    Notify(nameof(CanToggle));
                    Notify(nameof(ToggleButtonIcon));
                    Notify(nameof(ToggleButtonText));
                    Notify(nameof(ToggleButtonTooltip));
                }
            }
        }

        public bool IsStep1Enabled => IsActive != true;
        public bool CanStart => IsActive == false;
        public bool CanStop => IsActive == true;
        public bool CanToggle => IsActive != null;

        public string ToggleButtonIcon => IsActive switch
        {
            true => "■",
            false => "▶",
            _ => "⏳"
        };

        public string ToggleButtonText
        {
            get
            {
                if (IsActive == true) return $"{Name} AÇIK (DURDUR)";
                if (IsActive == false) return $"{Name} KAPALI (BAŞLAT)";
                if (Status == "STOPPING...") return $"{Name} DURDURULUYOR...";
                if (Status == "STARTING...") return $"{Name} BAŞLATILIYOR...";
                return $"{Name} {Status}";
            }
        }

        public string ToggleButtonTooltip
        {
            get
            {
                if (IsActive == true) return $"{Name} kaynağı şu an açık/çalışıyor. Durdurmak için tıklayın (STOP).";
                if (IsActive == false) return $"{Name} kaynağı şu an kapalı. Başlatmak için tıklayın (START).";
                if (Status == "STOPPING...") return $"{Name} durduruluyor, kapanma teyidi bekleniyor...";
                if (Status == "STARTING...") return $"{Name} başlatılıyor, donanım teyidi bekleniyor...";
                return $"{Name} geçiş işlemi yapılıyor, lütfen bekleyin...";
            }
        }

        public void UpdateIdlePreviews()
        {
            var dur1 = Services.DepositionSequenceController.ComputeStep1Duration(Step1.Setpoint, Step1.Ramp, Step1.Dwell);
            var dur2 = Services.DepositionSequenceController.ComputeStep2DurationPlanned(Step1.Setpoint, Step2.Setpoint, Step2.Ramp, Step2.Dwell);
            Step1.Remaining = Services.DepositionSequenceController.FormatStepProgress(dur1, dur1);
            Step2.Remaining = Services.DepositionSequenceController.FormatStepProgress(dur2, dur2);
        }

        public SourceState(string name, string target, string life)
        {
            Name = name;
            _target = target;
            _life = life;

            Step1.ValueChanged += () =>
            {
                if (IsActive != true) UpdateIdlePreviews();
            };
            Step2.ValueChanged += () =>
            {
                if (IsActive != true) UpdateIdlePreviews();
            };

            UpdateIdlePreviews();
        }
    }

    public sealed class StepState : ObservableState
    {
        private decimal _setpoint, _ramp, _dwell;
        private string _remaining = "—";

        public event Action? ValueChanged;

        public decimal Setpoint
        {
            get => _setpoint;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_setpoint, value)) return;
                _setpoint = value;
                Notify(nameof(Setpoint));
                ValueChanged?.Invoke();
            }
        }

        public decimal Ramp
        {
            get => _ramp;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_ramp, value)) return;
                _ramp = value;
                Notify(nameof(Ramp));
                ValueChanged?.Invoke();
            }
        }

        public decimal Dwell
        {
            get => _dwell;
            set
            {
                if (EqualityComparer<decimal>.Default.Equals(_dwell, value)) return;
                _dwell = value;
                Notify(nameof(Dwell));
                ValueChanged?.Invoke();
            }
        }

        public string Remaining { get => _remaining; set => Set(ref _remaining, value); }

        public StepState(decimal setpoint, decimal ramp, decimal dwell)
        {
            _setpoint = setpoint;
            _ramp = ramp;
            _dwell = dwell;
        }
    }

    public sealed class GasState : ObservableState
    {
        public ValveState Valve { get; } = new();
        private decimal _setpoint;
        private string _actual = "—";

        public decimal Setpoint { get => _setpoint; set => Set(ref _setpoint, value); }
        public string Actual { get => _actual; set => Set(ref _actual, value); }
    }

    public sealed class DeviceState : ObservableState
    {
        public string Name { get; }
        public ushort Address { get; }

        private string _status = "BİLİNMİYOR";
        public string Status { get => _status; set => Set(ref _status, value); }

        private bool? _isActive = null;
        public bool? IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    Status = value == true ? "START" : value == false ? "STOP" : "BİLİNMİYOR";
                }
            }
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            Notify(name ?? "");
            return true;
        }

        public DeviceState(string name, ushort address)
        {
            Name = name;
            Address = address;
        }
    }

    public sealed class WorkspaceState : ObservableState
    {
        public ValveState GateValve { get; } = new();
        public ValveState RoughingValve { get; } = new();
        public ValveState BypassValve { get; } = new();
        public ValveState VentValve { get; } = new();
        public ValveState TransferValve { get; } = new();

        public SourceState Dc { get; } = new("DC", "Ag", "30.38");
        public SourceState Rf1 { get; } = new("RF1", "SiO₂", "3179.62");
        public SourceState Rf2 { get; } = new("RF2", "TiO₂", "3287.48");

        public IReadOnlyList<DeviceState> Devices { get; } = new[]
        {
            new DeviceState("Rotation Motor", 100),
            new DeviceState("Rezistans", 101),
            new DeviceState("RF1 Power", 102),
            new DeviceState("RF2 Power", 103),
            new DeviceState("DC Power", 104),
            new DeviceState("Match Box Power", 105),
            new DeviceState("Scroll Vacuum", 106),
            new DeviceState("Turbo / EVP Vacuum", 107),
            new DeviceState("Chiller", 108),
            new DeviceState("Chamber Vent", 110),
            new DeviceState("RF1 Shutter", 112),
            new DeviceState("DC Shutter", 113),
            new DeviceState("RF2 Shutter", 114)
        };

        private decimal _rotation, _heater = 25, _gate;
        public decimal RotationSet { get => _rotation; set => Set(ref _rotation, value); }
        public decimal HeaterSet { get => _heater; set => Set(ref _heater, value); }
        public decimal GateSet { get => _gate; set => Set(ref _gate, value); }

        private string _pressure = "—", _capacitance = "—", _turbo = "—", _heaterRead = "—";
        public string Pressure { get => _pressure; set => Set(ref _pressure, value); }
        public string Capacitance { get => _capacitance; set => Set(ref _capacitance, value); }
        public string TurboFrequency { get => _turbo; set => Set(ref _turbo, value); }
        public string HeaterRead { get => _heaterRead; set => Set(ref _heaterRead, value); }

        private string _connectionStatusText = "Bağlı değil";
        public string ConnectionStatusText { get => _connectionStatusText; set => Set(ref _connectionStatusText, value); }

        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; set => Set(ref _isConnected, value); }

        private string _pollingStatusText = "Polling: —";
        public string PollingStatusText { get => _pollingStatusText; set => Set(ref _pollingStatusText, value); }

        private string _qcmStatusText = "QCM: —";
        public string QcmStatusText { get => _qcmStatusText; set => Set(ref _qcmStatusText, value); }

        private string _plcStatusText = "PLC: —";
        public string PlcStatusText { get => _plcStatusText; set => Set(ref _plcStatusText, value); }

        private bool _isEmergencyStopActive = false;
        public bool IsEmergencyStopActive { get => _isEmergencyStopActive; set => Set(ref _isEmergencyStopActive, value); }

        private bool _isScrollPumpActive;
        public bool IsScrollPumpActive { get => _isScrollPumpActive; set => Set(ref _isScrollPumpActive, value); }

        private bool _isTurboPumpActive;
        public bool IsTurboPumpActive { get => _isTurboPumpActive; set => Set(ref _isTurboPumpActive, value); }

        private bool _isRotationActive;
        public bool IsRotationActive
        {
            get => _isRotationActive;
            set
            {
                if (Set(ref _isRotationActive, value))
                    Notify(nameof(IsRotationSpinning));
            }
        }

        private bool _isHeaterActive;
        public bool IsHeaterActive { get => _isHeaterActive; set => Set(ref _isHeaterActive, value); }

        private bool _isChillerActive;
        public bool IsChillerActive { get => _isChillerActive; set => Set(ref _isChillerActive, value); }

        private bool _isMatchBoxActive;
        public bool IsMatchBoxActive { get => _isMatchBoxActive; set => Set(ref _isMatchBoxActive, value); }

        private bool _isRf1ShutterOpen;
        public bool IsRf1ShutterOpen
        {
            get => _isRf1ShutterOpen;
            set
            {
                if (Set(ref _isRf1ShutterOpen, value))
                {
                    Notify(nameof(IsRf1PlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            }
        }

        private bool _isDcShutterOpen;
        public bool IsDcShutterOpen
        {
            get => _isDcShutterOpen;
            set
            {
                if (Set(ref _isDcShutterOpen, value))
                {
                    Notify(nameof(IsDcPlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            }
        }

        private bool _isRf2ShutterOpen;
        public bool IsRf2ShutterOpen
        {
            get => _isRf2ShutterOpen;
            set
            {
                if (Set(ref _isRf2ShutterOpen, value))
                {
                    Notify(nameof(IsRf2PlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            }
        }

        private short _rotationRpm;
        public short RotationRpm
        {
            get => _rotationRpm;
            set
            {
                if (Set(ref _rotationRpm, value))
                    Notify(nameof(IsRotationSpinning));
            }
        }
        public bool IsRotationSpinning => IsRotationActive || _rotationRpm > 0;

        public double TurboFrequencyNumeric => double.TryParse(_turbo, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : 0;
        public bool IsTurboSpinning => TurboFrequencyNumeric > 5.0;

        private double _rawPressureTorr = 760;
        public double RawPressureTorr
        {
            get => _rawPressureTorr;
            set
            {
                if (Set(ref _rawPressureTorr, value))
                {
                    Notify(nameof(VacuumRegimeText));
                    Notify(nameof(VacuumRegimeKind));
                    Notify(nameof(IsHighVacuumReady));
                }
            }
        }

        public string VacuumRegimeText => RawPressureTorr switch
        {
            <= 0 => "BİLİNMİYOR",
            < 1e-4 => "YÜKSEK VAKUM",
            < 1.0 => "KABA VAKUM",
            _ => "ATMOSFER BASINCI"
        };

        public string VacuumRegimeKind => RawPressureTorr switch
        {
            <= 0 => "Unknown",
            < 1e-4 => "HighVacuum",
            < 1.0 => "RoughVacuum",
            _ => "Atmosphere"
        };

        public bool IsHighVacuumReady => RawPressureTorr > 0 && RawPressureTorr < 1e-4;

        public bool IsRf1PlasmaActive => Rf1.IsActive == true && IsRf1ShutterOpen;
        public bool IsDcPlasmaActive => Dc.IsActive == true && IsDcShutterOpen;
        public bool IsRf2PlasmaActive => Rf2.IsActive == true && IsRf2ShutterOpen;
        public bool IsAnyPlasmaActive => IsRf1PlasmaActive || IsDcPlasmaActive || IsRf2PlasmaActive;

        public WorkspaceState()
        {
            Rf1.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SourceState.IsActive))
                {
                    Notify(nameof(IsRf1PlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            };
            Dc.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SourceState.IsActive))
                {
                    Notify(nameof(IsDcPlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            };
            Rf2.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SourceState.IsActive))
                {
                    Notify(nameof(IsRf2PlasmaActive));
                    Notify(nameof(IsAnyPlasmaActive));
                }
            };
        }

        public DeviceState? GetDeviceByAddress(ushort address)
            => Devices.FirstOrDefault(d => d.Address == address);

        public void UpdateGateValve(short rawPos, short stateCode)
        {
            int actualPos = rawPos == 132 ? 100 : Math.Clamp((int)rawPos, 0, 100);
            int target = Math.Clamp((int)GateSet, 0, 100);

            GateValve.Position = actualPos;
            GateValve.StateCode = stateCode;

            string statusText;
            string stateKind;
            bool? isOpen;

            switch (stateCode)
            {
                case 4998: // Opening (Açılıyor)
                    statusText = $"AÇILIYOR (%{actualPos})";
                    stateKind = "Opening";
                    isOpen = true;
                    GateValve.IsMoving = true;
                    break;

                case 4999: // Closing (Kapanıyor)
                    statusText = $"KAPANIYOR (%{actualPos})";
                    stateKind = "Closing";
                    isOpen = actualPos > 0;
                    GateValve.IsMoving = true;
                    break;

                case 0: // Stop (Durdu)
                default:
                    GateValve.IsMoving = false;
                    if (actualPos == 0)
                    {
                        statusText = "KAPALI";
                        stateKind = "Closed";
                        isOpen = false;
                    }
                    else
                    {
                        statusText = "AÇIK";
                        stateKind = "Open";
                        isOpen = true;
                    }
                    break;
            }

            GateValve.StatusText = statusText;
            GateValve.StateKind = stateKind;
            GateValve.IsOpen = isOpen;
            GateValve.ToolTipText = $"Gate Valve: {statusText} | Anlık Konum: %{actualPos} | Hedef: %{GateSet} | Durum Kodu: {stateCode}";
        }

        public void ResetGateValve()
        {
            GateValve.StatusText = "BİLİNMİYOR";
            GateValve.StateKind = "Unknown";
            GateValve.IsOpen = null;
            GateValve.IsMoving = false;
            GateValve.ToolTipText = "Gate Valve: Bağlı değil";
        }
    }

    public sealed class ValveState : ObservableState
    {
        private bool? _isOpen;
        private string _statusText = "KAPALI";
        private string _stateKind = "Closed";
        private int _position = 0;
        private int _stateCode = 0;
        private bool _isMoving = false;
        private string _toolTipText = "Gate Valve güncel çalışma durumu";

        public bool? IsOpen { get => _isOpen; set => Set(ref _isOpen, value); }
        public string StatusText { get => _statusText; set => Set(ref _statusText, value); }
        public string StateKind { get => _stateKind; set => Set(ref _stateKind, value); }
        public int Position { get => _position; set => Set(ref _position, value); }
        public int StateCode { get => _stateCode; set => Set(ref _stateCode, value); }
        public bool IsMoving { get => _isMoving; set => Set(ref _isMoving, value); }
        public string ToolTipText { get => _toolTipText; set => Set(ref _toolTipText, value); }
    }
}
