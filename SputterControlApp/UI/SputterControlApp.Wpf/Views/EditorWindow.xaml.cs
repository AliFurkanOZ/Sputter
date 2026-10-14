using System.Windows;
using System.Windows.Controls;
using SputterControlApp.Wpf.Models;
namespace SputterControlApp.Wpf.Views;
public partial class EditorWindow : Window
{
    public EditorWindow(string kind, object? selected = null)
    {
        InitializeComponent();
        EditorGroup.Header = kind switch { "Material" => "Materyal", "Process" => "Proses", "CoDep" => "Ortak kaplama (CoDep) katmanı", _ => "Katman" };
        if (kind == "Material")
        {
            var item = selected as MaterialRow;
            Field("Material", item?.Name ?? ""); Field("Density (g/cm³)", item?.Density ?? "0", numeric: true); Field("Z-Ratio", item?.ZRatio ?? "0", numeric: true);
        }
        else if (kind == "Process")
        {
            var item = selected as ProcessRow;
            Field("Process", item?.Name ?? ""); Choice("Material", item?.Material ?? "Silver", "Silver", "Aluminum", "Titanium oxide"); Field("Tooling (%)", item?.Tooling.Replace(" %", "") ?? "100", numeric: true);
        }
        else
        {
            var item = selected as LayerRow;
            Choice("Process", item?.Process ?? "Ag deposition", "Ag deposition", "Al coating", "Oxide coating"); Field("Final (kÅ)", item?.Thickness ?? "0.50", numeric: true); Choice(kind == "CoDep" ? "Source 1" : "Source", item?.Source ?? "DC", "DC", "RF1", "RF2");
            if (kind == "CoDep") Choice("2. kaynak", "RF1", "RF2", "DC");
            Choice("Sensor", item?.Sensor ?? "DC", "DC", "RF1", "RF2"); Choice("Start mode", item?.StartMode ?? "Manual", "Manual", "Auto");
        }
    }
    // One labelled row per field in a two-column grid.
    private void Row(string label, FrameworkElement editor)
    {
        var row = Fields.RowDefinitions.Count;
        Fields.RowDefinitions.Add(new RowDefinition { Height = new GridLength(42) });
        var text = new TextBlock { Text = label, Style = (Style)FindResource("FieldLabel") };
        Grid.SetRow(text, row);
        Grid.SetRow(editor, row); Grid.SetColumn(editor, 1);
        editor.VerticalAlignment = VerticalAlignment.Center;
        Fields.Children.Add(text); Fields.Children.Add(editor);
    }
    private void Field(string label, string value, bool numeric = false)
    {
        var box = new TextBox { Text = value };
        if (numeric) box.Style = (Style)FindResource("NumericInput");
        Row(label, box);
    }
    private void Choice(string label, params string[] choices) => Row(label, new ComboBox { ItemsSource = choices.Distinct().ToArray(), SelectedIndex = 0 });
}
