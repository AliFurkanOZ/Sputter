using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SputterNewApp.Models;

namespace SputterNewApp.Controls
{
    public partial class SourceCard : UserControl
    {
        public SourceCard() => InitializeComponent();

        private MainWindow? MainWin => Window.GetWindow(this) as MainWindow;

        private async void StartSource(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SourceState source || MainWin?.Coordinator == null) return;
            await MainWin.Coordinator.StartSourceAsync(source.Name);
        }

        private async void StopSource(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SourceState source || MainWin?.Coordinator == null) return;
            await MainWin.Coordinator.StopSourceAsync(source.Name);
        }

        private async void ToggleSource(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SourceState source || MainWin?.Coordinator == null) return;
            if (source.IsActive == false)
            {
                await MainWin.Coordinator.StartSourceAsync(source.Name);
            }
            else if (source.IsActive == true)
            {
                await MainWin.Coordinator.StopSourceAsync(source.Name);
            }
        }

        private void SelectTarget(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SourceState source) return;
            var preview = MainWin?.Preview ?? new PreviewData();
            var coordinator = MainWin?.Coordinator;
            var dialog = new Views.TargetWindow(preview, coordinator, source.Name, source.Target)
            {
                Owner = Window.GetWindow(this)
            };
            if (dialog.ShowDialog() == true && dialog.SelectedTarget is { } target)
            {
                source.Target = target.Name;
                source.UsedWh = target.UsedWh;
            }
        }

        private async void ToggleArgonValve(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not SourceState source || MainWin?.Coordinator == null) return;
            int mfcId = source.Name switch
            {
                "RF2" => 1,
                "RF1" => 3,
                _ => 4 // DC
            };
            await MainWin.Coordinator.ToggleValveAsync(mfcId);
        }

        private async void ToggleOxygenValve(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not SourceState source || MainWin?.Coordinator == null) return;
            int mfcId = source.Name switch
            {
                "RF2" => 2,
                _ => 5 // DC
            };
            await MainWin.Coordinator.ToggleValveAsync(mfcId);
        }

        private async void OnArgonSetpointKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (sender is not TextBox tb || DataContext is not SourceState source || MainWin?.Coordinator == null) return;

            if (float.TryParse(tb.Text.Trim().Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var sp))
            {
                int mfcId = source.Name switch { "RF2" => 1, "RF1" => 3, _ => 4 };
                await MainWin.Coordinator.SetMfcSetpointAsync(mfcId, sp);
            }
        }

        private async void OnOxygenSetpointKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (sender is not TextBox tb || DataContext is not SourceState source || MainWin?.Coordinator == null) return;

            if (float.TryParse(tb.Text.Trim().Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var sp))
            {
                int mfcId = source.Name switch { "RF2" => 2, _ => 5 };
                await MainWin.Coordinator.SetMfcSetpointAsync(mfcId, sp);
            }
        }
    }
}
