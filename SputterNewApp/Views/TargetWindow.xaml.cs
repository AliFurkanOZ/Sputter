using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SputterNewApp.Core.Models;
using SputterNewApp.Models;
using SputterNewApp.Services;

namespace SputterNewApp.Views
{
    public partial class TargetWindow : Window
    {
        private readonly PreviewData _previewData;
        private readonly SputterServiceCoordinator? _coordinator;
        private readonly string? _currentSource;
        private readonly ObservableCollection<TargetRow> _viewTargets = new();
        private bool _isInitialized = false;

        public TargetRow? SelectedTarget { get; private set; }

        public TargetWindow() : this(new PreviewData(), null, null, null)
        {
        }

        public TargetWindow(PreviewData previewData) : this(previewData, null, null, null)
        {
        }

        public TargetWindow(PreviewData previewData, SputterServiceCoordinator? coordinator, string? currentSource = null, string? currentTargetName = null)
        {
            _previewData = previewData ?? new PreviewData();
            _coordinator = coordinator;
            _currentSource = string.IsNullOrWhiteSpace(currentSource) ? null : currentSource;

            InitializeComponent();

            TargetsGrid.ItemsSource = _viewTargets;

            if (!string.IsNullOrWhiteSpace(_currentSource))
            {
                lblActiveSource.Text = _currentSource;
                Title = $"Target (Hedef Materyal) Yönetimi ve Seçimi — {_currentSource}";
            }
            else
            {
                lblActiveSource.Text = "Tümü";
            }

            if (cbSourceFilter != null && cbSourceFilter.SelectedIndex < 0)
            {
                cbSourceFilter.SelectedIndex = 0;
            }

            // Eğer Targets listesi henüz yüklenmemişse AppDataManager'dan yükle
            if (_previewData.Targets.Count == 0)
            {
                try
                {
                    var loaded = AppDataManager.LoadTargetsSync();
                    foreach (var t in loaded)
                    {
                        _previewData.Targets.Add(new TargetRow(t.Name, string.IsNullOrWhiteSpace(t.Source) ? "—" : t.Source, t.UsedWh.ToString("0.00"), t.Id));
                    }
                }
                catch
                {
                }
            }

            _isInitialized = true;

            ReloadTargets(currentTargetName);
        }

        private void ReloadTargets(string? targetToSelect = null)
        {
            if (!_isInitialized) return;
            ApplyFilter(targetToSelect);
            UpdateButtonsState();
        }

        private void ApplyFilter(string? targetNameToSelect = null)
        {
            if (!_isInitialized || _previewData?.Targets == null) return;

            var search = txtSearch?.Text?.Trim() ?? "";
            var selectedFilterItem = (cbSourceFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tümü";

            var sourceItems = _previewData.Targets.Where(t => t != null).ToList();

            var filtered = sourceItems.Where(t =>
            {
                string tName = t.Name ?? "";
                string tSource = t.Source ?? "";

                // Arama metni filtresi (Target adı veya atanmış kaynak)
                if (!string.IsNullOrWhiteSpace(search))
                {
                    bool matchName = tName.Contains(search, StringComparison.OrdinalIgnoreCase);
                    bool matchSource = tSource.Contains(search, StringComparison.OrdinalIgnoreCase);
                    if (!matchName && !matchSource) return false;
                }

                // Kaynak dropdown filtresi
                if (selectedFilterItem == "DC" && !string.Equals(tSource, "DC", StringComparison.OrdinalIgnoreCase)) return false;
                if (selectedFilterItem == "RF1" && !string.Equals(tSource, "RF1", StringComparison.OrdinalIgnoreCase)) return false;
                if (selectedFilterItem == "RF2" && !string.Equals(tSource, "RF2", StringComparison.OrdinalIgnoreCase)) return false;
                if (selectedFilterItem.StartsWith("—") && !string.IsNullOrWhiteSpace(tSource) && tSource != "—") return false;

                return true;
            }).ToList();

            _viewTargets.Clear();
            foreach (var item in filtered)
            {
                _viewTargets.Add(item);
            }

            // Seçim belirleme
            TargetRow? itemToSelect = null;
            if (!string.IsNullOrWhiteSpace(targetNameToSelect))
            {
                itemToSelect = _viewTargets.FirstOrDefault(t => string.Equals(t.Name, targetNameToSelect, StringComparison.OrdinalIgnoreCase));
            }

            if (itemToSelect == null && SelectedTarget != null)
            {
                itemToSelect = _viewTargets.FirstOrDefault(t => (!string.IsNullOrWhiteSpace(t.Id) && t.Id == SelectedTarget.Id)
                                                             || string.Equals(t.Name, SelectedTarget.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (itemToSelect == null && _viewTargets.Any())
            {
                itemToSelect = _viewTargets.FirstOrDefault();
            }

            TargetsGrid.SelectedItem = itemToSelect;
            if (itemToSelect != null)
            {
                TargetsGrid.ScrollIntoView(itemToSelect);
            }

            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            if (!_isInitialized || _previewData?.Targets == null) return;

            int total = _previewData.Targets.Count;
            int showing = _viewTargets.Count;

            if (lblStatusCount != null)
            {
                lblStatusCount.Text = showing == total
                    ? $"Toplam {total} hedef materyal kayıtlı"
                    : $"{showing} hedef materyal gösteriliyor (Toplam {total} kayıt)";
            }
        }

        private void UpdateButtonsState()
        {
            if (!_isInitialized) return;

            var sel = TargetsGrid?.SelectedItem as TargetRow;
            bool hasSelection = sel != null;

            if (btnEdit != null) btnEdit.IsEnabled = hasSelection;
            if (btnDelete != null) btnDelete.IsEnabled = hasSelection;
            if (btnResetWh != null) btnResetWh.IsEnabled = hasSelection;
            if (btnSelect != null) btnSelect.IsEnabled = hasSelection;

            if (lblSelectionSummary != null)
            {
                lblSelectionSummary.Text = sel != null
                    ? $"Seçilen: {sel.Name}  |  Kaynak: {sel.Source}  |  Kullanılan Ömür: {sel.UsedWh} Wh"
                    : "Seçilen: —";
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            SelectedTarget = TargetsGrid.SelectedItem as TargetRow;
            UpdateButtonsState();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            ApplyFilter();
        }

        private void OnSourceFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            ApplyFilter();
        }

        private async void OnAddTargetClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var defaultSource = _currentSource ?? "—";
                var dummyRow = new TargetRow("", defaultSource, "0.00");

                var editor = new EditorWindow("Target", dummyRow) { Owner = this };
                if (editor.ShowDialog() == true && editor.ResultItem is TargetRow newTarget)
                {
                    // İsim benzersizlik kontrolü
                    if (_previewData.Targets.Any(t => string.Equals(t.Name, newTarget.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var res = MessageBox.Show($"'{newTarget.Name}' isimli bir hedef materyal zaten mevcut. Bilgilerini güncellemek istiyor musunuz?", "Mevcut Kayıt", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (res != MessageBoxResult.Yes) return;
                    }

                    if (_coordinator != null)
                    {
                        await _coordinator.SaveTargetAsync(newTarget);
                    }
                    else
                    {
                        // Fallback (Coordinator olmaması durumu)
                        var existing = _previewData.Targets.FirstOrDefault(t => string.Equals(t.Name, newTarget.Name, StringComparison.OrdinalIgnoreCase));
                        if (existing != null) _previewData.Targets.Remove(existing);
                        _previewData.Targets.Add(newTarget);

                        var storageTargets = _previewData.Targets.Select(t => new TargetItem
                        {
                            Id = string.IsNullOrWhiteSpace(t.Id) ? Guid.NewGuid().ToString() : t.Id,
                            Name = t.Name,
                            Source = t.Source,
                            UsedWh = decimal.TryParse(t.UsedWh.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var wh) ? wh : 0m
                        }).ToList();
                        await AppDataManager.SaveTargetsAsync(storageTargets);
                    }

                    ReloadTargets(newTarget.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Target eklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnEditTargetClicked(object sender, RoutedEventArgs e)
        {
            if (TargetsGrid.SelectedItem is not TargetRow selected)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz hedef materyali seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var editor = new EditorWindow("Target", selected) { Owner = this };
                if (editor.ShowDialog() == true && editor.ResultItem is TargetRow updatedTarget)
                {
                    if (_coordinator != null)
                    {
                        await _coordinator.SaveTargetAsync(updatedTarget);
                    }
                    else
                    {
                        int index = _previewData.Targets.IndexOf(selected);
                        if (index >= 0)
                        {
                            _previewData.Targets[index] = updatedTarget;
                        }

                        var storageTargets = _previewData.Targets.Select(t => new TargetItem
                        {
                            Id = string.IsNullOrWhiteSpace(t.Id) ? Guid.NewGuid().ToString() : t.Id,
                            Name = t.Name,
                            Source = t.Source,
                            UsedWh = decimal.TryParse(t.UsedWh.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var wh) ? wh : 0m
                        }).ToList();
                        await AppDataManager.SaveTargetsAsync(storageTargets);
                    }

                    ReloadTargets(updatedTarget.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Target düzenlenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnDeleteTargetClicked(object sender, RoutedEventArgs e)
        {
            if (TargetsGrid.SelectedItem is not TargetRow selected)
            {
                MessageBox.Show("Lütfen silmek istediğiniz hedef materyali seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"'{selected.Name}' ({selected.Source}) hedef materyalini silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz.",
                "Target Silme Onayı",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                if (_coordinator != null)
                {
                    await _coordinator.DeleteTargetAsync(selected.Id, selected.Name);
                }
                else
                {
                    _previewData.Targets.Remove(selected);
                    var storageTargets = _previewData.Targets.Select(t => new TargetItem
                    {
                        Id = string.IsNullOrWhiteSpace(t.Id) ? Guid.NewGuid().ToString() : t.Id,
                        Name = t.Name,
                        Source = t.Source,
                        UsedWh = decimal.TryParse(t.UsedWh.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var wh) ? wh : 0m
                    }).ToList();
                    await AppDataManager.SaveTargetsAsync(storageTargets);
                }

                ReloadTargets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Target silinirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnResetLifeClicked(object sender, RoutedEventArgs e)
        {
            if (TargetsGrid.SelectedItem is not TargetRow selected)
            {
                MessageBox.Show("Lütfen ömrü sıfırlanacak hedef materyali seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"'{selected.Name}' hedef materyali için kullanılan {selected.UsedWh} Wh ömrü 0.00 Wh olarak sıfırlansın mı?\n\n(Yeni target diski takıldığında bu işlem yapılır)",
                "Target Ömrü Sıfırlama",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var resetRow = selected with { UsedWh = "0.00" };

                if (_coordinator != null)
                {
                    await _coordinator.SaveTargetAsync(resetRow);
                }
                else
                {
                    int index = _previewData.Targets.IndexOf(selected);
                    if (index >= 0)
                    {
                        _previewData.Targets[index] = resetRow;
                    }
                    var storageTargets = _previewData.Targets.Select(t => new TargetItem
                    {
                        Id = string.IsNullOrWhiteSpace(t.Id) ? Guid.NewGuid().ToString() : t.Id,
                        Name = t.Name,
                        Source = t.Source,
                        UsedWh = decimal.TryParse(t.UsedWh.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var wh) ? wh : 0m
                    }).ToList();
                    await AppDataManager.SaveTargetsAsync(storageTargets);
                }

                ReloadTargets(resetRow.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Target ömrü sıfırlanırken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectTarget(object sender, RoutedEventArgs e)
        {
            if (TargetsGrid.SelectedItem is not TargetRow target)
            {
                MessageBox.Show("Lütfen bir hedef materyal seçin.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SelectedTarget = target;
            DialogResult = true;
            Close();
        }

        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject dep)
            {
                var row = FindVisualParent<DataGridRow>(dep);
                if (row != null && row.Item is TargetRow)
                {
                    SelectTarget(sender, e);
                }
            }
        }

        private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent) return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TargetsGrid.SelectedItem is TargetRow)
            {
                SelectTarget(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Delete && TargetsGrid.SelectedItem is TargetRow && (txtSearch == null || !txtSearch.IsFocused))
            {
                OnDeleteTargetClicked(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.F2 && TargetsGrid.SelectedItem is TargetRow)
            {
                OnEditTargetClicked(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                e.Handled = true;
            }
        }
    }
}
