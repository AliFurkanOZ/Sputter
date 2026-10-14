using System.Windows;
using System.Windows.Controls;
using SputterControlApp.Wpf.Models;

namespace SputterControlApp.Wpf.Controls;
public partial class SourceCard : UserControl
{
    public SourceCard() => InitializeComponent();
    private void SelectTarget(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SourceState source) return;
        var dialog = new Views.TargetWindow { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true && dialog.SelectedTarget is { } target)
        {
            source.Target = target.Name;
            source.UsedWh = target.UsedWh;
        }
    }
}
