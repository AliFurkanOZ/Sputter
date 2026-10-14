using System.Windows;
namespace SputterControlApp.Wpf.Views;
public partial class ManualReadWindow : Window
{
    public ManualReadWindow() => InitializeComponent();
    private void CloseWindow(object sender, RoutedEventArgs e) => Close();
}
