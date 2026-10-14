using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SputterControlApp.Wpf.Models;
using SputterControlApp.Wpf.Views;

namespace SputterControlApp.Wpf;

public partial class MainWindow : Window
{
    public WorkspaceState State { get; } = new();
    public PreviewData Preview { get; } = new();
    private static readonly string[] PageOrder = ["Process", "Graph", "QCM", "Log", "Debug", "Sputter"];
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }
    public void SelectPage(string page)
    {
        foreach (TabItem tab in Pages.Items)
            if ((string)tab.Tag == page) { Pages.SelectedItem = tab; break; }
    }
    private void HandleFunctionKeys(object sender, KeyEventArgs e)
    {
        var index = e.Key - Key.F1;
        if (index < 0 || index >= PageOrder.Length) return;
        SelectPage(PageOrder[index]);
        e.Handled = true;
    }
    private void OpenConnection(object sender, RoutedEventArgs e) => new ConnectionWindow { Owner = this }.ShowDialog();
    private void OpenManualRead(object sender, RoutedEventArgs e) => new ManualReadWindow { Owner = this }.ShowDialog();
    private void OpenSchematic(object sender, RoutedEventArgs e) => new Window { Title = "Sputter şeması", Style = (Style)FindResource("AppWindow"), Content = new Border { Padding = new Thickness(12), Child = new SputterDiagram { DataContext = this } }, Width = 1600, Height = 900, MinWidth = 800, MinHeight = 520, Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog();
    private void ResetPreview(object sender, RoutedEventArgs e)
    {
        var replacement = new MainWindow { WindowState = WindowState };
        if (WindowState == WindowState.Normal) { replacement.Left = Left; replacement.Top = Top; replacement.Width = Width; replacement.Height = Height; }
        Application.Current.MainWindow = replacement;
        replacement.Show();
        Close();
    }
    private void OpenEditor(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var kind = (string)button.Tag;
        object? selected = button.Content?.ToString() == "Edit" ? kind switch
        {
            "Material" => MaterialsGrid.SelectedItem,
            "Process" => ProcessesGrid.SelectedItem,
            _ => LayersGrid.SelectedItem
        } : null;
        new EditorWindow(kind, selected) { Owner = this }.ShowDialog();
    }
    private void FilterLog(object sender, RoutedEventArgs e)
    {
        if (LogGrid == null || LogSearch == null || LogLevel == null || LogCount == null) return;
        var query = LogSearch.Text.Trim();
        var level = (LogLevel.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
        var rows = Preview.Events.Where(row => (level == "All" || row.Level == level) && $"{row.Message} {row.Source} {row.Code}".Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
        LogGrid.ItemsSource = rows;
        LogCount.Text = $"{rows.Length} kayıt";
    }
}
