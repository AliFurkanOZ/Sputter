namespace SputterControlApp.Wpf.Models;

// Recipe rows are display fixtures, never loaded from the production application's files.
public sealed class PreviewData : ObservableState
{
    public PreviewData() => _selectedLayer = Layers[0];
    public IReadOnlyList<MaterialRow> Materials { get; } = [new("M01", "Silver", "10.50", "0.529"), new("M02", "Aluminum", "2.70", "1.080"), new("M03", "Titanium oxide", "3.97", "0.336")];
    public IReadOnlyList<ProcessRow> Processes { get; } = [new("P01", "Ag deposition", "Silver", "100 %"), new("P02", "Al coating", "Aluminum", "95 %"), new("P03", "Oxide coating", "Titanium oxide", "100 %")];
    public IReadOnlyList<LayerRow> Layers { get; } = [new("1", "Ag deposition", "Silver", "0.50", "DC", "DC", "Manual", "—"), new("2", "Al coating", "Aluminum", "1.20", "RF1", "RF1", "Auto", "—"), new("3", "Oxide coating", "Titanium oxide", "0.80", "RF2", "RF2", "Auto", "—")];
    public IReadOnlyList<TargetRow> Targets { get; } = [new("Ag", "DC", "30.38"), new("SiO₂", "RF1", "3179.62"), new("TiO₂", "RF2", "3287.48"), new("WO₃", "—", "913.60")];
    public IReadOnlyList<LogRow> Events { get; } = [
        new("—", "INFO", "UI-001", "Arayüz açıldı", "UI", "—"),
        new("—", "INFO", "UI-002", "Cihaz bağlantısı yok", "UI", "—"),
        new("—", "INFO", "UI-003", "Reçete listeleri örnek veriler içeriyor", "UI", "—")];
    public IReadOnlyList<SensorRow> Sensors { get; } = [new("RF1"), new("RF2"), new("DC")];
    private LayerRow? _selectedLayer;
    public LayerRow? SelectedLayer
    {
        get => _selectedLayer;
        set { Set(ref _selectedLayer, value); Notify(nameof(SelectedMaterial)); Notify(nameof(SelectedProcess)); }
    }
    public MaterialRow? SelectedMaterial => Materials.FirstOrDefault(row => row.Name == SelectedLayer?.Material);
    public ProcessRow? SelectedProcess => Processes.FirstOrDefault(row => row.Name == SelectedLayer?.Process);
}
public sealed record MaterialRow(string Number, string Name, string Density, string ZRatio);
public sealed record ProcessRow(string Number, string Name, string Material, string Tooling);
public sealed record LayerRow(string Number, string Process, string Material, string Thickness, string Source, string Sensor, string StartMode, string Status);
public sealed record TargetRow(string Name, string Source, string UsedWh);
public sealed record LogRow(string Time, string Level, string Code, string Message, string Source, string Registration);
public sealed class SensorRow(string source) : ObservableState
{
    public string Source { get; } = source;
    private string _rate = "—", _thickness = "—", _life = "—";
    public string Rate { get => _rate; set => Set(ref _rate, value); }
    public string Thickness { get => _thickness; set => Set(ref _thickness, value); }
    public string Life { get => _life; set => Set(ref _life, value); }
}
