using System.Windows;
namespace SputterControlApp.Wpf.Views;
public partial class ConnectionWindow : Window
{
    public ConnectionWindow() => InitializeComponent();
    private void CloseWindow(object sender, RoutedEventArgs e) => Close();
}
