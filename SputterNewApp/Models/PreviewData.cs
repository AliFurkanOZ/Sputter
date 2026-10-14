using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SputterNewApp.Models
{
    public sealed class PreviewData : ObservableState
    {
        public ObservableCollection<MaterialRow> Materials { get; } = new();
        public ObservableCollection<ProcessRow> Processes { get; } = new();
        public ObservableCollection<LayerRow> Layers { get; } = new();
        public ObservableCollection<TargetRow> Targets { get; } = new();
        public ObservableCollection<LogRow> Events { get; } = new();
        public ObservableCollection<SensorRow> Sensors { get; } = new()
        {
            new SensorRow("DC"),
            new SensorRow("RF1"),
            new SensorRow("RF2")
        };

        private LayerRow? _selectedLayer;
        public LayerRow? SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                Set(ref _selectedLayer, value);
                Notify(nameof(CurrentLayerProcess));
                Notify(nameof(CurrentLayerMaterial));
                Notify(nameof(SelectedMaterial));
                Notify(nameof(SelectedProcess));
            }
        }

        public ProcessRow? CurrentLayerProcess
            => Processes.FirstOrDefault(row => string.Equals(row.Name, SelectedLayer?.Process, StringComparison.OrdinalIgnoreCase));

        public MaterialRow? CurrentLayerMaterial
        {
            get
            {
                var proc = CurrentLayerProcess;
                var matName = !string.IsNullOrWhiteSpace(SelectedLayer?.Material) && SelectedLayer?.Material != "—"
                    ? SelectedLayer?.Material
                    : proc?.Material;
                return Materials.FirstOrDefault(row => string.Equals(row.Name, matName, StringComparison.OrdinalIgnoreCase));
            }
        }

        private MaterialRow? _selectedMaterial;
        public MaterialRow? SelectedMaterial
        {
            get => _selectedMaterial ?? CurrentLayerMaterial ?? Materials.FirstOrDefault();
            set => Set(ref _selectedMaterial, value);
        }

        private ProcessRow? _selectedProcess;
        public ProcessRow? SelectedProcess
        {
            get => _selectedProcess ?? CurrentLayerProcess ?? Processes.FirstOrDefault();
            set => Set(ref _selectedProcess, value);
        }

        public PreviewData()
        {
            // Başlangıç varsayılanları (varsa)
            Materials.Add(new MaterialRow("1", "Silver", "10.50", "0.529"));
            Materials.Add(new MaterialRow("2", "Aluminum", "2.70", "1.080"));
            Materials.Add(new MaterialRow("3", "Titanium oxide", "3.97", "0.336"));

            Processes.Add(new ProcessRow("1", "Ag deposition", "Silver", "100 %"));
            Processes.Add(new ProcessRow("2", "Al coating", "Aluminum", "95 %"));
            Processes.Add(new ProcessRow("3", "Oxide coating", "Titanium oxide", "100 %"));

            Layers.Add(new LayerRow("1", "Ag deposition", "Silver", "0.50", "DC", "DC", "Manual", "—"));
            Layers.Add(new LayerRow("2", "Al coating", "Aluminum", "1.20", "RF1", "RF1", "Auto", "—"));
            Layers.Add(new LayerRow("3", "Oxide coating", "Titanium oxide", "0.80", "RF2", "RF2", "Auto", "—"));

            Targets.Add(new TargetRow("Ag", "DC", "30.38"));
            Targets.Add(new TargetRow("SiO₂", "RF1", "3179.62"));
            Targets.Add(new TargetRow("TiO₂", "RF2", "3287.48"));
            Targets.Add(new TargetRow("WO₃", "—", "913.60"));

            SelectedLayer = Layers.FirstOrDefault();
            SelectedMaterial = Materials.FirstOrDefault();
            SelectedProcess = Processes.FirstOrDefault();
        }

        public SensorRow? GetSensorBySource(string source)
            => Sensors.FirstOrDefault(s => string.Equals(s.Source, source, StringComparison.OrdinalIgnoreCase));
    }

    public sealed record MaterialRow(string Number, string Name, string Density, string ZRatio, string Id = "");
    public sealed record ProcessRow(string Number, string Name, string Material, string Tooling, string Id = "");
    public sealed record LayerRow(string Number, string Process, string Material, string Thickness, string Source, string Sensor, string StartMode, string Status, string Id = "");
    public sealed record TargetRow(string Name, string Source, string UsedWh, string Id = "");
    public sealed record LogRow(string Time, string Level, string Code, string Message, string Source, string Registration);

    public sealed class SensorRow : ObservableState
    {
        public string Source { get; }
        private string _port = "—";
        private string _serialNo = "—";
        private string _status = "Bağlı değil";
        private string _rate = "—", _thickness = "—", _life = "—";
        private bool _isConnected;

        public string Port { get => _port; set => Set(ref _port, value); }
        public string SerialNo { get => _serialNo; set => Set(ref _serialNo, value); }
        public string Status { get => _status; set => Set(ref _status, value); }
        public string Rate { get => _rate; set => Set(ref _rate, value); }
        public string Thickness { get => _thickness; set => Set(ref _thickness, value); }
        public string Life { get => _life; set => Set(ref _life, value); }
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (Set(ref _isConnected, value))
                {
                    Status = value ? "Bağlı" : "Bağlı değil";
                }
            }
        }

        public SensorRow(string source)
        {
            Source = source;
        }

        public void Reset()
        {
            Port = "—";
            SerialNo = "—";
            Rate = "—";
            Thickness = "—";
            Life = "—";
            IsConnected = false;
            Status = "Bağlı değil";
        }
    }
}
