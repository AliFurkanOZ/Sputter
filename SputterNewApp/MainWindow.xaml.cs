using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using SputterNewApp.Models;
using SputterNewApp.Services;
using SputterNewApp.Views;

namespace SputterNewApp;

public partial class MainWindow : Window
{
    public WorkspaceState State { get; } = new();
    public PreviewData Preview { get; } = new();
    public SputterServiceCoordinator Coordinator { get; }

    private static readonly string[] PageOrder = ["Process", "Graph", "QCM", "Log", "Debug", "Sputter"];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        Coordinator = new SputterServiceCoordinator(State, Preview);

        Coordinator.LayerRunningStateChanged += (isRunning, layer) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (isRunning)
                {
                    btnStartLayer.Content = "■ Stop Layer";
                    btnStartLayer.Style = (Style)FindResource("DangerToolButton");
                    btnNextLayer.IsEnabled = false;
                    LayersGrid.IsEnabled = false;
                }
                else
                {
                    btnStartLayer.Content = "▶ Start Layer";
                    btnStartLayer.Style = (Style)FindResource("PrimaryButton");
                    btnNextLayer.IsEnabled = true;
                    LayersGrid.IsEnabled = true;
                }
            });
        };

        Coordinator.LayerAutoCompleted += (layer) =>
        {
            Dispatcher.Invoke(() =>
            {
                OnNextLayerClicked(this, new RoutedEventArgs());
            });
        };

        Loaded += async (_, _) =>
        {
            await Coordinator.WarmupAndInitializeAsync();
            UpdateDiagnostics();
        };

        Closing += (_, _) => Coordinator.Dispose();
    }

    public void SelectPage(string page)
    {
        foreach (TabItem tab in Pages.Items)
        {
            if ((string)tab.Tag == page)
            {
                Pages.SelectedItem = tab;
                break;
            }
        }
    }

    private void HandleFunctionKeys(object sender, KeyEventArgs e)
    {
        var index = e.Key - Key.F1;
        if (index < 0 || index >= PageOrder.Length) return;
        SelectPage(PageOrder[index]);
        e.Handled = true;
    }

    private void OpenConnection(object sender, RoutedEventArgs e)
    {
        new ConnectionWindow(Coordinator) { Owner = this }.ShowDialog();
        UpdateDiagnostics();
    }

    private void OpenManualRead(object sender, RoutedEventArgs e)
    {
        new ManualReadWindow(Coordinator) { Owner = this }.ShowDialog();
    }

    private void OpenSchematic(object sender, RoutedEventArgs e)
    {
        new Window
        {
            Title = "Sputter Şeması & Dijital İkiz (P&ID)",
            Style = (Style)FindResource("AppWindow"),
            Background = (Brush)FindResource("ScreenBrush"),
            Content = new SputterDiagram { DataContext = this },
            Width = 1600,
            Height = 900,
            MinWidth = 800,
            MinHeight = 520,
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        }.ShowDialog();
    }

    private void ResetPreview(object sender, RoutedEventArgs e)
    {
        var replacement = new MainWindow { WindowState = WindowState };
        if (WindowState == WindowState.Normal)
        {
            replacement.Left = Left;
            replacement.Top = Top;
            replacement.Width = Width;
            replacement.Height = Height;
        }
        Application.Current.MainWindow = replacement;
        replacement.Show();
        Close();
    }

    private async void OnRefreshClicked(object sender, RoutedEventArgs e)
    {
        await Coordinator.ReadSnapshotAsync();
        UpdateDiagnostics();
    }

    private void OnDisconnectClicked(object sender, RoutedEventArgs e)
    {
        Coordinator.Disconnect();
        UpdateDiagnostics();
    }

    private async void OnDeviceToggleClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ushort addr)
        {
            await Coordinator.ToggleDeviceAsync(addr);
        }
    }

    private async void OnRotationKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        await Coordinator.SetRotationMotorAsync(State.RotationSet);
    }

    private async void OnGateKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        await Coordinator.SetGateValvePositionAsync(State.GateSet);
    }

    private async void OnHeaterKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        await Coordinator.SetOrdelTemperatureAsync(State.HeaterSet);
    }

    private async void OnHeaterSetClicked(object sender, RoutedEventArgs e)
    {
        await Coordinator.SetOrdelTemperatureAsync(State.HeaterSet);
    }

    private async void OpenEditor(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var kind = (string)button.Tag;
        bool isEdit = (button.Content is StackPanel sp && sp.Children.OfType<TextBlock>().Any(tb => tb.Text.Contains("Düzenle")))
                      || button.Content?.ToString() == "Edit"
                      || (button.Tag is string t && t.StartsWith("Edit"));

        // SputterControlApp algoritma kuralı 1: Process eklemeden önce Material olmalı
        if (kind == "Process" && !isEdit && Preview.Materials.Count == 0)
        {
            MessageBox.Show("Önce Material eklemelisin. Proses oluşturabilmek için sistemde en az bir materyal kayıtlı olmalıdır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // SputterControlApp algoritma kuralı 2: Layer eklemeden önce Process olmalı
        if ((kind == "Layer" || kind == "CoDep") && !isEdit && Preview.Processes.Count == 0)
        {
            MessageBox.Show("Önce Process eklemelisin. Katman oluşturabilmek için sistemde en az bir proses kayıtlı olmalıdır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        object? selected = isEdit ? kind switch
        {
            "Material" => MaterialsGrid.SelectedItem,
            "Process" => ProcessesGrid.SelectedItem,
            _ => LayersGrid.SelectedItem
        } : null;

        if (isEdit && selected == null)
        {
            string itemType = kind switch
            {
                "Material" => "materyal",
                "Process" => "proses",
                _ => "katman"
            };
            MessageBox.Show($"Lütfen düzenlemek istediğiniz {itemType} kaydını seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new EditorWindow(kind, selected, Preview) { Owner = this };
        if (editor.ShowDialog() == true && editor.ResultItem != null)
        {
            if (editor.ResultItem is MaterialRow mat)
                await Coordinator.SaveMaterialAsync(mat);
            else if (editor.ResultItem is ProcessRow proc)
                await Coordinator.SaveProcessAsync(proc);
            else if (editor.ResultItem is LayerRow layer)
                await Coordinator.SaveLayerAsync(layer);
            else if (editor.ResultItem is TargetRow target)
                await Coordinator.SaveTargetAsync(target);
        }
    }

    private async void OnDeleteMaterialClicked(object sender, RoutedEventArgs e)
    {
        if (MaterialsGrid.SelectedItem is MaterialRow row)
        {
            var res = MessageBox.Show($"'{row.Name}' materyalini silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
                await Coordinator.DeleteMaterialAsync(row.Id);
        }
    }

    private async void OnDeleteProcessClicked(object sender, RoutedEventArgs e)
    {
        if (ProcessesGrid.SelectedItem is ProcessRow row)
        {
            var res = MessageBox.Show($"'{row.Name}' prosesini silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
                await Coordinator.DeleteProcessAsync(row.Id);
        }
    }

    private async void OnDeleteLayerClicked(object sender, RoutedEventArgs e)
    {
        if (LayersGrid.SelectedItem is LayerRow row)
        {
            var res = MessageBox.Show($"'{row.Process}' katmanını silmek istediğinize emin misiniz?", "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
                await Coordinator.DeleteLayerAsync(row.Id);
        }
    }

    private async void OnStartLayerClicked(object sender, RoutedEventArgs e)
    {
        if (Coordinator.IsLayerRunning)
        {
            await Coordinator.StopLayerExecutionAsync(manual: true);
            return;
        }

        if (LayersGrid.SelectedItem is not LayerRow layer)
        {
            MessageBox.Show("Lütfen önce başlatılacak katmanı seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!Coordinator.Interlocks.IsChillerOn)
        {
            MessageBox.Show("Chiller kapalı! Katman başlatabilmek için önce Chiller cihazını başlatmalısınız.", "Güvenlik Kilidi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await Coordinator.StartLayerExecutionAsync(layer);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Katman başlatılırken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnNextLayerClicked(object sender, RoutedEventArgs e)
    {
        if (Coordinator.IsLayerRunning) return;
        if (LayersGrid.Items.Count == 0) return;
        int nextIndex = (LayersGrid.SelectedIndex + 1) % LayersGrid.Items.Count;
        LayersGrid.SelectedIndex = nextIndex;
    }

    private async void OnZeroThicknessClicked(object sender, RoutedEventArgs e)
    {
        if (LayersGrid.SelectedItem is LayerRow layer)
        {
            await Coordinator.ResetSensorThicknessAsync(layer.Sensor);
        }
        else
        {
            // Varsayılan olarak tüm sensörleri sıfırla
            await Coordinator.ResetSensorThicknessAsync("RF1");
            await Coordinator.ResetSensorThicknessAsync("RF2");
            await Coordinator.ResetSensorThicknessAsync("DC");
        }
    }

    private async void OnRescanQcmClicked(object sender, RoutedEventArgs e)
    {
        btnRescanQcm.IsEnabled = false;
        try
        {
            await Coordinator.AutoConnectStm2BySerialAsync();
            UpdateDiagnostics();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[QCM Rescan] Hata: {ex.Message}");
        }
        finally
        {
            btnRescanQcm.IsEnabled = true;
        }
    }

    private void FilterLog(object sender, RoutedEventArgs e)
    {
        if (LogGrid == null || LogSearch == null || LogLevel == null || LogCount == null) return;
        var query = LogSearch.Text.Trim();
        var level = (LogLevel.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
        var rows = Preview.Events.Where(row =>
            (level == "All" || row.Level.Equals(level, StringComparison.OrdinalIgnoreCase)) &&
            $"{row.Message} {row.Source} {row.Code}".Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToArray();

        LogGrid.ItemsSource = rows;
        LogCount.Text = $"{rows.Length} kayıt";
    }

    private void OnClearLogClicked(object sender, RoutedEventArgs e)
    {
        Preview.Events.Clear();
        if (LogCount != null) LogCount.Text = "0 kayıt";
    }

    private void OnExportLogClicked(object sender, RoutedEventArgs e)
    {
        if (Preview.Events.Count == 0)
        {
            MessageBox.Show("Dışa aktarılacak log kaydı bulunamadı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var sfd = new SaveFileDialog
        {
            Filter = "CSV Dosyası (*.csv)|*.csv|Metin Dosyası (*.txt)|*.txt",
            FileName = $"Sputter_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (sfd.ShowDialog() == true)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Zaman;Tür;Kod;Mesaj;Kaynak;KayıtNo");
                foreach (var ev in Preview.Events)
                {
                    sb.AppendLine($"\"{ev.Time}\";\"{ev.Level}\";\"{ev.Code}\";\"{ev.Message}\";\"{ev.Source}\";\"{ev.Registration}\"");
                }
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Loglar başarıyla kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void OnExportGraphDataClicked(object sender, RoutedEventArgs e)
    {
        var sfd = new SaveFileDialog
        {
            Filter = "CSV Dosyası (*.csv)|*.csv",
            FileName = $"Sputter_GraphData_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (sfd.ShowDialog() == true)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Zaman;Chamber (Torr);Capacitance (Torr);Heater (°C);DC_Rate (Å/s);RF1_Rate (Å/s);RF2_Rate (Å/s)");
                sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{State.Pressure};{State.Capacitance};{State.HeaterRead};{Preview.GetSensorBySource("DC")?.Rate};{Preview.GetSensorBySource("RF1")?.Rate};{Preview.GetSensorBySource("RF2")?.Rate}");
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Grafik verileri kaydedildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void UpdateDiagnostics()
    {
        if (txtDiagnostics == null) return;
        var plc = Coordinator.Modbus.IsConnected ? $"Bağlı ({Coordinator.Modbus.CurrentSettings?.IpAddress}:{Coordinator.Modbus.CurrentSettings?.Port})" : "Bağlı değil";
        txtDiagnostics.Text = $"=== SİSTEM TANILAMA ===\nPLC: {plc}\n{State.QcmStatusText}\n{State.PollingStatusText}\nAcil Stop: {(State.IsEmergencyStopActive ? "AKTİF (ALARM)" : "Normal")}\nKayıt Sayısı: {Preview.Events.Count}";
    }
}
