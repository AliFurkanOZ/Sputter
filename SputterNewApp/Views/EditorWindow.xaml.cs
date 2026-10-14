using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SputterNewApp.Core.Models;
using SputterNewApp.Models;

namespace SputterNewApp.Views
{
    public partial class EditorWindow : Window
    {
        private readonly string _kind;
        private readonly object? _selected;
        private readonly PreviewData? _previewData;
        private readonly Dictionary<string, FrameworkElement> _inputs = new();

        public object? ResultItem { get; private set; }

        public EditorWindow(string kind, object? selected = null) : this(kind, selected, null)
        {
        }

        public EditorWindow(string kind, object? selected = null, PreviewData? previewData = null)
        {
            InitializeComponent();
            _kind = kind;
            _selected = selected;
            _previewData = previewData;

            EditorGroup.Header = kind switch
            {
                "Material" => "Materyal Bilgisi",
                "Process" => "Proses Parametreleri",
                "CoDep" => "Ortak Kaplama (CoDep) Katmanı",
                "Target" => "Target (Hedef Materyal) Bilgisi",
                _ => "Katman Parametreleri"
            };

            Title = kind switch
            {
                "Material" => (selected == null ? "Yeni Materyal Ekle" : "Materyal Düzenle"),
                "Process" => (selected == null ? "Yeni Proses Ekle" : "Proses Düzenle"),
                "Target" => (selected == null || (selected is TargetRow tr && string.IsNullOrWhiteSpace(tr.Name)) ? "Yeni Target Ekle" : "Target Düzenle"),
                "CoDep" => "Co-Dep Katmanı",
                _ => (selected == null ? "Yeni Katman Ekle" : "Katman Düzenle")
            };

            if (kind == "Material")
            {
                var item = selected as MaterialRow;
                Field("Material", item?.Name ?? "");
                Field("Density (g/cm³)", item?.Density ?? "0", numeric: true);
                Field("Z-Ratio", item?.ZRatio ?? "0", numeric: true);
            }
            else if (kind == "Target")
            {
                var item = selected as TargetRow;
                Field("Target", item?.Name ?? "");
                Choice("Kaynak", item?.Source ?? "—", "—", "DC", "RF1", "RF2");
                Field("Kullanılan ömür (Wh)", item?.UsedWh ?? "0.00", numeric: true);
            }
            else if (kind == "Process")
            {
                var item = selected as ProcessRow;
                Field("Process", item?.Name ?? "");

                // Material listesini dinamik olarak PreviewData veya AppDataManager'dan al
                var matList = GetMaterialNames();
                string defaultMat = item?.Material ?? matList.FirstOrDefault() ?? "—";
                Choice("Material", defaultMat, matList);

                Field("Tooling (%)", item?.Tooling.Replace("%", "").Trim() ?? "100", numeric: true);
            }
            else
            {
                var item = selected as LayerRow;

                // Process listesini dinamik olarak PreviewData veya AppDataManager'dan al
                var procList = GetProcessNames();
                string defaultProc = item?.Process ?? procList.FirstOrDefault() ?? "—";
                Choice("Process", defaultProc, procList);

                Field("Final (kÅ)", item?.Thickness ?? "0.50", numeric: true);
                Choice(kind == "CoDep" ? "Source 1" : "Source", item?.Source ?? "DC", "DC", "RF1", "RF2");
                if (kind == "CoDep") Choice("2. kaynak", "RF1", "RF2", "DC");
                Choice("Sensor", item?.Sensor ?? "DC", "DC", "RF1", "RF2");
                Choice("Start mode", item?.StartMode ?? "Manual", "Manual", "Auto");
            }
        }

        private List<string> GetMaterialNames()
        {
            var list = _previewData?.Materials?
                .Select(m => m.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n) && n != "—")
                .Distinct()
                .ToList() ?? new List<string>();

            if (list.Count == 0)
            {
                try
                {
                    var loaded = AppDataManager.LoadMaterialsSync();
                    list = loaded.Select(m => m.Name).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
                }
                catch { }
            }

            return list;
        }

        private List<string> GetProcessNames()
        {
            var list = _previewData?.Processes?
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n) && n != "—")
                .Distinct()
                .ToList() ?? new List<string>();

            if (list.Count == 0)
            {
                try
                {
                    var loaded = AppDataManager.LoadProcessesSync();
                    list = loaded.Select(p => p.Name).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
                }
                catch { }
            }

            return list;
        }

        private void Row(string label, FrameworkElement editor)
        {
            var row = Fields.RowDefinitions.Count;
            Fields.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
            var text = new TextBlock { Text = label, Style = (Style)FindResource("FieldLabel") };
            Grid.SetRow(text, row);
            Grid.SetRow(editor, row);
            Grid.SetColumn(editor, 1);
            editor.VerticalAlignment = VerticalAlignment.Center;
            Fields.Children.Add(text);
            Fields.Children.Add(editor);
            _inputs[label] = editor;
        }

        private void Field(string label, string value, bool numeric = false)
        {
            var box = new TextBox { Text = value };
            if (numeric) box.Style = (Style)FindResource("NumericInput");
            Row(label, box);
        }

        private void Choice(string label, string? selectedValue, IEnumerable<string> choices)
        {
            var list = choices.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
            if (!string.IsNullOrWhiteSpace(selectedValue) && !list.Contains(selectedValue))
                list.Insert(0, selectedValue);

            var cb = new ComboBox { ItemsSource = list };
            cb.SelectedItem = selectedValue ?? list.FirstOrDefault();
            if (cb.SelectedIndex < 0 && list.Any()) cb.SelectedIndex = 0;
            Row(label, cb);
        }

        private void Choice(string label, string? selectedValue, params string[] choices) 
            => Choice(label, selectedValue, (IEnumerable<string>)choices);

        private void Choice(string label, params string[] choices) 
            => Choice(label, choices.FirstOrDefault(), (IEnumerable<string>)choices);

        private string GetValue(string label)
        {
            if (_inputs.TryGetValue(label, out var el))
            {
                if (el is TextBox tb) return tb.Text.Trim();
                if (el is ComboBox cb) return cb.SelectedItem?.ToString() ?? "";
            }
            return "";
        }

        private void SaveItem(object sender, RoutedEventArgs e)
        {
            if (_kind == "Material")
            {
                var name = GetValue("Material");
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Materyal adı boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var density = GetValue("Density (g/cm³)");
                var zratio = GetValue("Z-Ratio");
                var oldItem = _selected as MaterialRow;

                ResultItem = new MaterialRow(oldItem?.Number ?? "—", name.Trim(), density.Trim(), zratio.Trim(), oldItem?.Id ?? Guid.NewGuid().ToString());
            }
            else if (_kind == "Target")
            {
                var name = GetValue("Target");
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Target adı boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var source = GetValue("Kaynak");
                var usedWh = GetValue("Kullanılan ömür (Wh)");
                if (string.IsNullOrWhiteSpace(usedWh)) usedWh = "0.00";
                var oldItem = _selected as TargetRow;
                string id = (oldItem != null && !string.IsNullOrWhiteSpace(oldItem.Id)) ? oldItem.Id : Guid.NewGuid().ToString();

                ResultItem = new TargetRow(name.Trim(), string.IsNullOrWhiteSpace(source) ? "—" : source.Trim(), usedWh.Trim(), id);
            }
            else if (_kind == "Process")
            {
                var name = GetValue("Process");
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Proses adı boş olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var mat = GetValue("Material");
                if (string.IsNullOrWhiteSpace(mat) || mat == "—")
                {
                    MessageBox.Show("Lütfen bir materyal seçin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var tooling = GetValue("Tooling (%)");
                if (string.IsNullOrWhiteSpace(tooling)) tooling = "100";
                tooling = tooling.Replace("%", "").Trim() + " %";
                var oldItem = _selected as ProcessRow;

                ResultItem = new ProcessRow(oldItem?.Number ?? "—", name.Trim(), mat.Trim(), tooling, oldItem?.Id ?? Guid.NewGuid().ToString());
            }
            else
            {
                var proc = GetValue("Process");
                if (string.IsNullOrWhiteSpace(proc) || proc == "—")
                {
                    MessageBox.Show("Lütfen bir proses seçin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var thk = GetValue("Final (kÅ)");
                if (string.IsNullOrWhiteSpace(thk)) thk = "0.50";
                var src = _kind == "CoDep" ? GetValue("Source 1") : GetValue("Source");
                var sensor = GetValue("Sensor");
                var startMode = GetValue("Start mode");
                var oldItem = _selected as LayerRow;

                // Seçilen Prosesin bağlı olduğu Materyali bul
                string matName = "—";
                if (_previewData != null)
                {
                    var matchedProc = _previewData.Processes.FirstOrDefault(p => string.Equals(p.Name, proc, StringComparison.OrdinalIgnoreCase));
                    if (matchedProc != null && !string.IsNullOrWhiteSpace(matchedProc.Material) && matchedProc.Material != "—")
                    {
                        matName = matchedProc.Material;
                    }
                    else
                    {
                        var firstMat = _previewData.Materials.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Name) && m.Name != "—");
                        if (firstMat != null) matName = firstMat.Name;
                    }
                }

                ResultItem = new LayerRow(oldItem?.Number ?? "—", proc.Trim(), matName, thk.Trim(), src.Trim(), sensor.Trim(), startMode.Trim(), oldItem?.Status ?? "—", oldItem?.Id ?? Guid.NewGuid().ToString());
            }

            DialogResult = true;
        }
    }
}
