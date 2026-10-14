using System.Windows;
using SputterControlApp.Wpf.Models;
namespace SputterControlApp.Wpf.Views;
public partial class TargetWindow : Window
{
    public TargetRow? SelectedTarget { get; private set; }
    public TargetWindow() { InitializeComponent(); DataContext = new PreviewData(); }
    private void SelectTarget(object sender, RoutedEventArgs e)
    {
        if (Targets.SelectedItem is not TargetRow target) return;
        SelectedTarget = target;
        DialogResult = true;
    }
}
