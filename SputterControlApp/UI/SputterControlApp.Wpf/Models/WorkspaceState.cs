using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SputterControlApp.Wpf.Models;

// UI state only. A future service adapter can populate these properties on the UI thread.
public abstract class ObservableState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public sealed class SourceState(string name, string target, string life)
    : ObservableState
{
    public string Name { get; } = name;
    public bool IsDc => Name == "DC";
    public bool HasOxygen => Name != "RF1";
    public string SettingsSizeGroup => IsDc ? "DcSettings" : "RfSettings";
    public string SetUnit => IsDc ? "Set (mA)" : "Set (W)";
    public string RampUnit => IsDc ? "Ramp (mA/s)" : "Ramp (W/s)";
    private string _target = target, _life = life;
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
    private string _read1 = "—", _read2 = "—", _read3 = "—", _status = "—";
    public string Read1 { get => _read1; set => Set(ref _read1, value); }
    public string Read2 { get => _read2; set => Set(ref _read2, value); }
    public string Read3 { get => _read3; set => Set(ref _read3, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public string ReadLabel1 => IsDc ? "Power (W)" : "Forward (W)";
    public string ReadLabel2 => IsDc ? "Voltage (V)" : "Reflected (W)";
    public string ReadLabel3 => IsDc ? "Current (mA)" : "Set (W)";
}

public sealed class StepState(decimal setpoint, decimal ramp, decimal dwell) : ObservableState
{
    private decimal _setpoint = setpoint, _ramp = ramp, _dwell = dwell;
    private string _remaining = "—";
    public decimal Setpoint { get => _setpoint; set => Set(ref _setpoint, value); }
    public decimal Ramp { get => _ramp; set => Set(ref _ramp, value); }
    public decimal Dwell { get => _dwell; set => Set(ref _dwell, value); }
    public string Remaining { get => _remaining; set => Set(ref _remaining, value); }
}
public sealed class GasState : ObservableState
{
    public ValveState Valve { get; } = new();
    private decimal _setpoint;
    private string _actual = "—";
    public decimal Setpoint { get => _setpoint; set => Set(ref _setpoint, value); }
    public string Actual { get => _actual; set => Set(ref _actual, value); }
}
public sealed class DeviceState(string name) : ObservableState
{
    public string Name { get; } = name;
    private string _status = "BİLİNMİYOR";
    public string Status { get => _status; set => Set(ref _status, value); }
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
    public IReadOnlyList<DeviceState> Devices { get; } = new[] { "Scroll Vacuum", "Chiller", "Turbo / EVP Vacuum", "Chamber Vent", "RF1 Power", "RF1 Shutter", "RF2 Power", "RF2 Shutter", "DC Power", "DC Shutter", "Match Box Power", "Rezistans", "Rotation Motor" }.Select(n => new DeviceState(n)).ToArray();
    private decimal _rotation, _heater = 25, _gate;
    public decimal RotationSet { get => _rotation; set => Set(ref _rotation, value); }
    public decimal HeaterSet { get => _heater; set => Set(ref _heater, value); }
    public decimal GateSet { get => _gate; set => Set(ref _gate, value); }
    private string _pressure = "—", _capacitance = "—", _turbo = "—", _heaterRead = "—";
    public string Pressure { get => _pressure; set => Set(ref _pressure, value); }
    public string Capacitance { get => _capacitance; set => Set(ref _capacitance, value); }
    public string TurboFrequency { get => _turbo; set => Set(ref _turbo, value); }
    public string HeaterRead { get => _heaterRead; set => Set(ref _heaterRead, value); }
}

public sealed class ValveState : ObservableState
{
    private bool? _isOpen;
    public bool? IsOpen { get => _isOpen; set => Set(ref _isOpen, value); }
}
