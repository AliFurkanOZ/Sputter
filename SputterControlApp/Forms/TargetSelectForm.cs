using SputterControlApp.Core.Models;
using System.ComponentModel;
using System.Drawing;

namespace SputterControlApp
{
    internal sealed class TargetSelectForm : Form
    {
        private readonly List<TargetItem> _allTargets;
        private readonly HashSet<string> _blockedNames;
        private readonly string _currentTargetKey;
        private BindingList<TargetItem> _visibleTargets = new();

        private readonly DataGridView _grid = new DataGridView();
        private readonly Button _btnSelect = new Button();
        private readonly Button _btnNew = new Button();
        private readonly Button _btnEdit = new Button();
        private readonly Button _btnDelete = new Button();
        private readonly Button _btnCancel = new Button();

        public TargetItem? SelectedTarget { get; private set; }

        private TargetSelectForm(
            IList<TargetItem> targets,
            string? currentTargetName,
            IEnumerable<string>? unavailableTargetNames)
        {
            _allTargets = targets.Select(CloneTarget).ToList();
            _blockedNames = new HashSet<string>(unavailableTargetNames ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
            _currentTargetKey = NormalizeTargetName(currentTargetName);

            Text = "Target Select";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(760, 520);

            BuildLayout();
            RefreshVisibleTargets(selectedTargetName: currentTargetName);
            UpdateButtonStates();
        }

        public static async Task<TargetItem?> SelectTargetAsync(
            IWin32Window owner,
            string? currentTargetName,
            decimal? currentUsedWh,
            IEnumerable<string>? unavailableTargetNames)
        {
            var targets = await AppDataManager.LoadTargetsAsync();

            if (!string.IsNullOrWhiteSpace(currentTargetName) &&
                !targets.Any(t => string.Equals(t.Name?.Trim(), currentTargetName.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                targets.Insert(0, new TargetItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = currentTargetName.Trim(),
                    UsedWh = currentUsedWh ?? 0m
                });
            }

            using var form = new TargetSelectForm(targets, currentTargetName, unavailableTargetNames);
            return form.ShowDialog(owner) == DialogResult.OK
                ? form.SelectedTarget
                : null;
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(12)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170f));

            var gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };

            _grid.Dock = DockStyle.Fill;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.AutoGenerateColumns = false;
            _grid.BackgroundColor = Color.White;
            _grid.MultiSelect = false;
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _grid.DoubleClick += (_, __) => SelectCurrentTargetAndClose();
            _grid.SelectionChanged += (_, __) => UpdateButtonStates();

            var colName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(TargetItem.Name),
                HeaderText = "Target Material",
                FillWeight = 58f
            };

            var colUsedWh = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(TargetItem.UsedWh),
                HeaderText = "Target Used (Wh)",
                FillWeight = 42f,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            };

            _grid.Columns.Add(colName);
            _grid.Columns.Add(colUsedWh);
            gridHost.Controls.Add(_grid);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10, 38, 0, 0),
                WrapContents = false
            };

            ConfigureButton(_btnSelect, "SELECT", SelectButton_Click);
            ConfigureButton(_btnNew, "NEW", NewButton_Click);
            ConfigureButton(_btnEdit, "EDIT", EditButton_Click);
            ConfigureButton(_btnDelete, "DELETE", DeleteButton_Click);
            ConfigureButton(_btnCancel, "CANCEL", (_, __) => Close());
            _btnCancel.DialogResult = DialogResult.Cancel;

            buttons.Controls.Add(_btnSelect);
            buttons.Controls.Add(_btnNew);
            buttons.Controls.Add(_btnEdit);
            buttons.Controls.Add(_btnDelete);
            buttons.Controls.Add(_btnCancel);

            root.Controls.Add(gridHost, 0, 0);
            root.Controls.Add(buttons, 1, 0);

            Controls.Add(root);

            AcceptButton = _btnSelect;
            CancelButton = _btnCancel;
        }

        private void ConfigureButton(Button button, string text, EventHandler onClick)
        {
            button.Text = text;
            button.Width = 130;
            button.Height = 54;
            button.Margin = new Padding(0, 0, 0, 16);
            button.Click += onClick;
        }

        private void RefreshVisibleTargets(string? selectedTargetName = null, string? selectedTargetId = null)
        {
            var visible = _allTargets
                .Where(CanDisplayTarget)
                .OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(CloneTarget)
                .ToList();

            _visibleTargets = new BindingList<TargetItem>(visible);
            _grid.DataSource = _visibleTargets;

            if (_grid.Rows.Count == 0)
            {
                UpdateButtonStates();
                return;
            }

            var rowToSelect = _grid.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(r =>
                    r.DataBoundItem is TargetItem target &&
                    ((!string.IsNullOrWhiteSpace(selectedTargetId) &&
                      string.Equals(target.Id, selectedTargetId, StringComparison.Ordinal)) ||
                     (!string.IsNullOrWhiteSpace(selectedTargetName) &&
                      string.Equals(target.Name?.Trim(), selectedTargetName.Trim(), StringComparison.CurrentCultureIgnoreCase))))
                ?? _grid.Rows[0];

            rowToSelect.Selected = true;
            _grid.CurrentCell = rowToSelect.Cells[0];
            UpdateButtonStates();
        }

        private bool CanDisplayTarget(TargetItem target)
        {
            var targetKey = NormalizeTargetName(target.Name);
            if (string.Equals(targetKey, _currentTargetKey, StringComparison.Ordinal))
                return true;

            return !_blockedNames.Contains(targetKey);
        }

        private TargetItem? GetSelectedTarget()
        {
            if (_grid.CurrentRow?.DataBoundItem is TargetItem current)
                return current;

            if (_grid.SelectedRows.Count > 0 && _grid.SelectedRows[0].DataBoundItem is TargetItem selected)
                return selected;

            return null;
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = GetSelectedTarget() != null;
            _btnSelect.Enabled = hasSelection;
            _btnEdit.Enabled = hasSelection;
            _btnDelete.Enabled = hasSelection;
        }

        private static TargetItem CloneTarget(TargetItem target)
        {
            return new TargetItem
            {
                Id = target.Id,
                Name = target.Name,
                UsedWh = target.UsedWh
            };
        }

        private static string NormalizeTargetName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return string.Join(" ", value.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                .ToUpperInvariant();
        }

        private bool IsDuplicateName(string name, string? exceptId = null)
        {
            var normalized = NormalizeTargetName(name);
            if (string.IsNullOrWhiteSpace(normalized))
                return true;

            return _allTargets.Any(target =>
                !string.Equals(target.Id, exceptId, StringComparison.Ordinal) &&
                NormalizeTargetName(target.Name) == normalized);
        }

        private async Task SaveTargetsAsyncSafe()
        {
            try
            {
                await AppDataManager.SaveTargetsAsync(_allTargets.Select(CloneTarget).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Target listesi kaydedilemedi: {ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SelectCurrentTargetAndClose()
        {
            var target = GetSelectedTarget();
            if (target == null)
                return;

            SelectedTarget = CloneTarget(target);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectButton_Click(object? sender, EventArgs e)
            => SelectCurrentTargetAndClose();

        private async void NewButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new TargetEditDialog("New Target");
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            var name = dialog.TargetName.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Target Material bos olamaz.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsDuplicateName(name))
            {
                MessageBox.Show("Ayni isimde target zaten var.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = new TargetItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                UsedWh = dialog.UsedWh
            };

            _allTargets.Add(item);
            await SaveTargetsAsyncSafe();
            RefreshVisibleTargets(selectedTargetId: item.Id);
        }

        private async void EditButton_Click(object? sender, EventArgs e)
        {
            var selected = GetSelectedTarget();
            if (selected == null)
                return;

            using var dialog = new TargetEditDialog("Edit Target", selected);
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            var name = dialog.TargetName.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Target Material bos olamaz.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsDuplicateName(name, selected.Id))
            {
                MessageBox.Show("Ayni isimde target zaten var.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var targetInStore = _allTargets.FirstOrDefault(t => string.Equals(t.Id, selected.Id, StringComparison.Ordinal));
            if (targetInStore == null)
                return;

            targetInStore.Name = name;
            targetInStore.UsedWh = dialog.UsedWh;

            await SaveTargetsAsyncSafe();
            RefreshVisibleTargets(selectedTargetId: selected.Id);
        }

        private async void DeleteButton_Click(object? sender, EventArgs e)
        {
            var selected = GetSelectedTarget();
            if (selected == null)
                return;

            var confirm = MessageBox.Show(
                $"'{selected.Name}' target silinsin mi?",
                "Delete Target",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            var index = _visibleTargets.IndexOf(selected);
            var targetInStore = _allTargets.FirstOrDefault(t => string.Equals(t.Id, selected.Id, StringComparison.Ordinal));
            if (targetInStore == null)
                return;

            _allTargets.Remove(targetInStore);
            await SaveTargetsAsyncSafe();

            var remainingVisible = _allTargets
                .Where(CanDisplayTarget)
                .OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            if (remainingVisible.Count == 0)
            {
                RefreshVisibleTargets();
                return;
            }

            var nextIndex = Math.Clamp(index, 0, remainingVisible.Count - 1);
            RefreshVisibleTargets(selectedTargetId: remainingVisible[nextIndex].Id);
        }

        private sealed class TargetEditDialog : Form
        {
            private readonly TextBox _txtName = new TextBox();
            private readonly NumericUpDown _numUsedWh = new NumericUpDown();

            public string TargetName => _txtName.Text;
            public decimal UsedWh => _numUsedWh.Value;

            public TargetEditDialog(string title, TargetItem? existing = null)
            {
                Text = title;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                ClientSize = new Size(420, 170);

                _numUsedWh.DecimalPlaces = 2;
                _numUsedWh.Minimum = 0;
                _numUsedWh.Maximum = 1000000;
                _numUsedWh.ThousandsSeparator = true;
                _numUsedWh.Increment = 0.01m;

                var grid = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 3,
                    Padding = new Padding(12)
                };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));

                var lblName = new Label { Text = "Target Material", AutoSize = true, Anchor = AnchorStyles.Left };
                var lblUsed = new Label { Text = "Target Used (Wh)", AutoSize = true, Anchor = AnchorStyles.Left };

                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

                var buttons = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(12),
                    Height = 50
                };

                grid.Controls.Add(lblName, 0, 0);
                grid.Controls.Add(_txtName, 1, 0);
                grid.Controls.Add(lblUsed, 0, 1);
                grid.Controls.Add(_numUsedWh, 1, 1);

                buttons.Controls.Add(btnOk);
                buttons.Controls.Add(btnCancel);

                Controls.Add(grid);
                Controls.Add(buttons);

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                if (existing != null)
                {
                    _txtName.Text = existing.Name;
                    _numUsedWh.Value = Math.Clamp(existing.UsedWh, _numUsedWh.Minimum, _numUsedWh.Maximum);
                }
            }
        }
    }
}
