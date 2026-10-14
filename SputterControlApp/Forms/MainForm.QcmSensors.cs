using SputterControlApp.Core.Models;
using SputterControlApp.Core.Registers;
using SputterControlApp.Core.Services;
using System.ComponentModel;
using System.Data;


namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private Stm2Client? _stm;
        private BindingList<MaterialItem> _materials = new();
        private BindingList<ProcessItem> _processes = new();
        private BindingList<LayerItem> _layers = new();
        private bool _autoStopTriggered = false;
        private DateTime _nextLifePollUtc = DateTime.MinValue;
        private DateTime _nextIdleRatePollUtc = DateTime.MinValue;

        // Thickness unit policy:
        // - STM-2 returns raw thickness in Å (Angstrom)
        // - UI shows thickness in kÅ
        private const float ANGSTROM_PER_KA = 1000f;
        private static float AngstromToKA(float angstrom) => angstrom / ANGSTROM_PER_KA;
        private static string FormatLogFloat(float value) =>
            value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);

        private void dgvMaterials_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvMaterials.Rows[e.RowIndex].Cells[colMatNumber.Index].Value = (e.RowIndex + 1).ToString();
        }
        private void dgvProcesses_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
        {
            dgvProcesses.Rows[e.RowIndex].Cells[colProcessNum.Index].Value = (e.RowIndex + 1).ToString();
        }

        private async Task LoadMaterialsToGridAsync()
        {
            // 1) Seed -> (ilk açılışta) user storage’a kopyalanmış materyal listesini getirir
            var mats = await AppDataManager.LoadMaterialsAsync();

            // 2) BindingList'e çevir
            _materials = new BindingList<MaterialItem>(mats);

            // 3) Grid bind
            dgvMaterials.AutoGenerateColumns = false; // çünkü sende colMatNumber gibi designer kolonları var
            dgvMaterials.DataSource = _materials;

            // (İstersen) ilk kolon/özellik görünürlükleri burada yönetilebilir
            // Örn: eğer grid’de Id kolonu varsa gizle vs.
        }

        // ===============================
        // Materials CRUD (Add/Edit/Delete)
        // ===============================
        private static string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return string.Join(" ", s.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                .ToUpperInvariant();
        }

        private bool IsDuplicateMaterialName(string name, string? exceptId = null)
        {
            var key = NormalizeName(name);
            if (string.IsNullOrEmpty(key)) return true; // boş isim = geçersiz

            return _materials.Any(m =>
                (exceptId == null || m.Id != exceptId) &&
                NormalizeName(m.Name) == key
            );
        }

        private MaterialItem? GetSelectedMaterial()
        {
            if (dgvMaterials.CurrentRow?.DataBoundItem is MaterialItem mi)
                return mi;

            // bazen CurrentRow null olabilir; seçili satırlardan da deneriz
            if (dgvMaterials.SelectedRows.Count > 0 &&
                dgvMaterials.SelectedRows[0].DataBoundItem is MaterialItem mi2)
                return mi2;

            return null;
        }
        private async Task SaveMaterialsAsyncSafe()
        {
            try
            {
                await AppDataManager.SaveMaterialsAsync(_materials.ToList());
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                MessageBox.Show($"Materials kaydedilemedi: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -----------------------
        // Minimal Material Dialog
        // -----------------------
        private sealed class MaterialEditDialog : Form
        {
            private readonly TextBox txtName = new TextBox();
            private readonly NumericUpDown numDensity = new NumericUpDown();
            private readonly NumericUpDown numZ = new NumericUpDown();
            private readonly Button btnOk = new Button();
            private readonly Button btnCancel = new Button();

            public string MaterialName => txtName.Text;
            public float Density => (float)numDensity.Value;
            public float ZRatio => (float)numZ.Value;

            public MaterialEditDialog(string title, MaterialItem? initial = null)
            {
                Text = title;
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 420;
                Height = 230;

                var lbl1 = new Label { Left = 16, Top = 18, Width = 120, Text = "Material Name" };
                txtName.Left = 150; txtName.Top = 14; txtName.Width = 230;

                var lbl2 = new Label { Left = 16, Top = 58, Width = 120, Text = "Density" };
                numDensity.Left = 150; numDensity.Top = 54; numDensity.Width = 120;
                numDensity.DecimalPlaces = 3;
                numDensity.Minimum = 0;
                numDensity.Maximum = 1000;

                var lbl3 = new Label { Left = 16, Top = 98, Width = 120, Text = "Z Ratio" };
                numZ.Left = 150; numZ.Top = 94; numZ.Width = 120;
                numZ.DecimalPlaces = 3;
                numZ.Minimum = 0;
                numZ.Maximum = 1000;

                btnOk.Text = "OK";
                btnOk.Left = 210; btnOk.Top = 140; btnOk.Width = 80;
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = "Cancel";
                btnCancel.Left = 300; btnCancel.Top = 140; btnCancel.Width = 80;
                btnCancel.DialogResult = DialogResult.Cancel;

                Controls.AddRange(new Control[] { lbl1, txtName, lbl2, numDensity, lbl3, numZ, btnOk, btnCancel });

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                if (initial != null)
                {
                    txtName.Text = initial.Name ?? "";
                    numDensity.Value = (decimal)Math.Clamp(initial.Density, 0, 1000);
                    numZ.Value = (decimal)Math.Clamp(initial.ZRatio, 0, 1000);
                }
            }
        }

        // -----------------------
        // Button handlers
        // -----------------------
        private async void btnMatAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using var dlg = new MaterialEditDialog("Add Material");
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var name = dlg.MaterialName;

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Material Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (IsDuplicateMaterialName(name))
                {
                    MessageBox.Show("Aynı isimde material zaten var.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var item = new MaterialItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name.Trim(),
                    Density = dlg.Density,
                    ZRatio = dlg.ZRatio,
                    IsSeedMaterial = false
                };

                _materials.Add(item);
                await SaveMaterialsAsyncSafe();
                dgvProcesses.Invalidate();

                // yeni ekleneni seç
                dgvMaterials.ClearSelection();
                var idx = _materials.IndexOf(item);
                if (idx >= 0 && idx < dgvMaterials.Rows.Count)
                {
                    dgvMaterials.Rows[idx].Selected = true;
                    dgvMaterials.CurrentCell = dgvMaterials.Rows[idx].Cells[0];
                }
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private async void btnMatEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedMaterial();
                if (sel == null)
                {
                    MessageBox.Show("Düzenlemek için bir satır seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new MaterialEditDialog("Edit Material", sel);
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var newName = dlg.MaterialName;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Material Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (IsDuplicateMaterialName(newName, exceptId: sel.Id))
                {
                    MessageBox.Show("Aynı isimde material zaten var.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                sel.Name = newName.Trim();
                sel.Density = dlg.Density;
                sel.ZRatio = dlg.ZRatio;

                // BindingList değişimi grid’e yansır ama bazı durumlarda refresh gerekir
                dgvMaterials.Refresh();

                await SaveMaterialsAsyncSafe();
                dgvProcesses.Invalidate();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private async void btnMatDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedMaterial();
                if (sel == null)
                {
                    MessageBox.Show("Silmek için bir satır seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Process kullanımı kontrolü
                if (_processes.Any(p => p.MaterialId == sel.Id))
                {
                    MessageBox.Show(
                        "Bu material process listesinde kullanılıyor. Silinemez.",
                        "Blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }


                var confirm = MessageBox.Show(
                    $"'{sel.Name}' silinsin mi?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                _materials.Remove(sel);
                await SaveMaterialsAsyncSafe();
                dgvProcesses.Invalidate();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }


        // ===============================
        // Process Add/Edit Dialog
        // ===============================
        private sealed class ProcessEditDialog : Form
        {
            private readonly TextBox txtName = new TextBox();
            private readonly ComboBox cmbMaterial = new ComboBox();
            private readonly NumericUpDown numTooling = new NumericUpDown();
            private readonly Button btnOk = new Button();
            private readonly Button btnCancel = new Button();

            public string ProcessName => txtName.Text;
            public string SelectedMaterialId => (cmbMaterial.SelectedValue?.ToString()) ?? "";
            public float Tooling => (float)numTooling.Value;

            public ProcessEditDialog(
                string title,
                IList<MaterialItem> materials,
                ProcessItem? initial = null)
            {
                Text = title;
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 520;
                Height = 260;

                var lbl1 = new Label { Left = 16, Top = 18, Width = 130, Text = "Process Name" };
                txtName.Left = 160; txtName.Top = 14; txtName.Width = 320;

                var lbl2 = new Label { Left = 16, Top = 58, Width = 130, Text = "Material" };
                cmbMaterial.Left = 160; cmbMaterial.Top = 54; cmbMaterial.Width = 320;
                cmbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;

                var lbl3 = new Label { Left = 16, Top = 98, Width = 130, Text = "Tooling" };
                numTooling.Left = 160; numTooling.Top = 94; numTooling.Width = 160;
                numTooling.DecimalPlaces = 3;
                numTooling.Minimum = 0;
                numTooling.Maximum = 1000;
                numTooling.Value = 1.000m;

                btnOk.Text = "OK";
                btnOk.Left = 320; btnOk.Top = 150; btnOk.Width = 75;
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = "Cancel";
                btnCancel.Left = 405; btnCancel.Top = 150; btnCancel.Width = 75;
                btnCancel.DialogResult = DialogResult.Cancel;

                Controls.AddRange(new Control[] { lbl1, txtName, lbl2, cmbMaterial, lbl3, numTooling, btnOk, btnCancel });

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                // ---- bind materials ----
                // ComboBox: Display = Name, Value = Id
                cmbMaterial.DataSource = materials.ToList(); // snapshot
                cmbMaterial.DisplayMember = nameof(MaterialItem.Name);
                cmbMaterial.ValueMember = nameof(MaterialItem.Id);

                // ---- initial values (Edit) ----
                if (initial != null)
                {
                    txtName.Text = initial.Name ?? "";
                    numTooling.Value = (decimal)Math.Clamp(initial.Tooling, 0, 1000);

                    if (!string.IsNullOrWhiteSpace(initial.MaterialId))
                    {
                        cmbMaterial.SelectedValue = initial.MaterialId;
                    }
                }

                // Basit validasyon: Material listesi boşsa OK kapansın
                if (materials.Count == 0)
                {
                    btnOk.Enabled = false;
                    cmbMaterial.Enabled = false;
                }
            }
        }

        private ProcessItem? GetSelectedProcess()
        {
            if (dgvProcesses.CurrentRow?.DataBoundItem is ProcessItem pi)
                return pi;

            if (dgvProcesses.SelectedRows.Count > 0 &&
                dgvProcesses.SelectedRows[0].DataBoundItem is ProcessItem pi2)
                return pi2;

            return null;
        }

        private async Task SaveProcessesAsyncSafe()
        {
            try
            {
                await AppDataManager.SaveProcessesAsync(_processes.ToList());
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                MessageBox.Show($"Processes kaydedilemedi: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnProcAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Material listesi boşsa process ekletmeyelim
                if (_materials.Count == 0)
                {
                    MessageBox.Show("Önce Material eklemelisin.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new ProcessEditDialog("Add Process", _materials.ToList());
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                if (string.IsNullOrWhiteSpace(dlg.ProcessName))
                {
                    MessageBox.Show("Process Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsDuplicateProcessName(dlg.ProcessName))
                {
                    MessageBox.Show("Aynı isimde process zaten var.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(dlg.SelectedMaterialId))
                {
                    MessageBox.Show("Material seçmelisin.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var item = new ProcessItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dlg.ProcessName.Trim(),
                    MaterialId = dlg.SelectedMaterialId,
                    Tooling = dlg.Tooling
                };

                _processes.Add(item);
                await SaveProcessesAsyncSafe();
                dgvProcesses.Refresh();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private async void btnProcEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedProcess();
                if (sel == null)
                {
                    MessageBox.Show("Düzenlemek için bir process seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new ProcessEditDialog("Edit Process", _materials.ToList(), sel);
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                if (string.IsNullOrWhiteSpace(dlg.ProcessName))
                {
                    MessageBox.Show("Process Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsDuplicateProcessName(dlg.ProcessName, exceptId: sel.Id))
                {
                    MessageBox.Show("Aynı isimde process zaten var.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(dlg.SelectedMaterialId))
                {
                    MessageBox.Show("Material seçmelisin.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                sel.Name = dlg.ProcessName.Trim();
                sel.MaterialId = dlg.SelectedMaterialId;
                sel.Tooling = dlg.Tooling;

                dgvProcesses.Refresh();
                await SaveProcessesAsyncSafe();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }
        private int? GetMaterialNumberById(string materialId)
        {
            if (string.IsNullOrWhiteSpace(materialId)) return null;

            for (int i = 0; i < _materials.Count; i++)
            {
                if (_materials[i].Id == materialId)
                    return i + 1; // 1-based
            }

            return null;
        }

        private void dgvProcesses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // WinForms bazen header (-1) veya geçici index ile çağırabilir
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                // Silme/refresh anında index out of range olabiliyor
                if (e.RowIndex >= dgvProcesses.Rows.Count) return;
                if (e.ColumnIndex >= dgvProcesses.Columns.Count) return;

                var col = dgvProcesses.Columns[e.ColumnIndex];
                if (col == null) return;

                if (!string.Equals(col.Name, "colProcMaterialNumber", StringComparison.Ordinal))
                    return;

                var row = dgvProcesses.Rows[e.RowIndex];
                if (row == null || row.IsNewRow) return;

                if (row.DataBoundItem is not ProcessItem p) return;

                var num = GetMaterialNumberById(p.MaterialId);
                e.Value = num?.ToString() ?? "--";
                e.FormattingApplied = true;
            }
            catch
            {
                // CellFormatting içinde exception fırlatmak grid’i kilitleyebilir.
                // Bu yüzden swallow ediyoruz.
            }
        }

        private async void btnProcDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedProcess();
                if (sel == null)
                {
                    MessageBox.Show("Silmek için bir process seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (_layers.Any(l => l.ProcessId == sel.Id))
                {
                    MessageBox.Show("Bu process Layer listesinde kullanılıyor. Silinemez.", "Blocked",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show(
                    $"'{sel.Name}' silinsin mi?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                _processes.Remove(sel);
                await SaveProcessesAsyncSafe();

                // grid refresh (çoğu durumda gerekmez ama güvenli)
                dgvProcesses.Refresh();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private bool IsDuplicateProcessName(string name, string? exceptId = null)
        {
            var key = NormalizeName(name);
            if (string.IsNullOrEmpty(key)) return true; // boş/invalid

            return _processes.Any(p =>
                (exceptId == null || p.Id != exceptId) &&
                NormalizeName(p.Name) == key
            );
        }
        // -----------------------
        // Processes CRUD (Add/Edit/Delete)
        // -----------------------

        private async Task LoadProcessesToGridAsync()
        {
            var list = await AppDataManager.LoadProcessesAsync();

            _processes = new BindingList<ProcessItem>(list);

            dgvProcesses.AutoGenerateColumns = false; // Designer kolonları var
            dgvProcesses.DataSource = _processes;

            // satır numaraları için (Designer’da yoksa)
            dgvProcesses.RowPostPaint -= dgvProcesses_RowPostPaint;
            dgvProcesses.RowPostPaint += dgvProcesses_RowPostPaint;

            dgvProcesses.CellFormatting -= dgvProcesses_CellFormatting;
            dgvProcesses.CellFormatting += dgvProcesses_CellFormatting;
        }







        private async Task LoadLayersToGridAsync()
        {
            var list = await AppDataManager.LoadLayersAsync();
            _layers = new BindingList<LayerItem>(list);

            dgvLayers.AutoGenerateColumns = false;
            dgvLayers.DataSource = _layers;

            // Bound kolonlar
            if (dgvLayers.Columns.Contains("colFinalThickness"))
                dgvLayers.Columns["colFinalThickness"].DataPropertyName = nameof(LayerItem.FinalThickness);

            if (dgvLayers.Columns.Contains("colSource"))
                dgvLayers.Columns["colSource"].DataPropertyName = nameof(LayerItem.Source);

            if (dgvLayers.Columns.Contains("colRF1Sensor"))
                dgvLayers.Columns["colRF1Sensor"].DataPropertyName = nameof(LayerItem.RF1SensorEnabled);

            if (dgvLayers.Columns.Contains("colRF2Sensor"))
                dgvLayers.Columns["colRF2Sensor"].DataPropertyName = nameof(LayerItem.RF2SensorEnabled);

            if (dgvLayers.Columns.Contains("colDCSensor"))
                dgvLayers.Columns["colDCSensor"].DataPropertyName = nameof(LayerItem.DCSensorEnabled);


            // Formatting (unbound kolonlar + sensor ON)
            dgvLayers.CellFormatting -= dgvLayers_CellFormatting;
            dgvLayers.CellFormatting += dgvLayers_CellFormatting;
        }

        private async Task SaveLayersAsyncSafe()
        {
            try
            {
                await AppDataManager.SaveLayersAsync(_layers.ToList());
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                MessageBox.Show($"Layers kaydedilemedi: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? GetProcessNumberById(string processId)
        {
            if (string.IsNullOrWhiteSpace(processId)) return null;

            for (int i = 0; i < _processes.Count; i++)
                if (_processes[i].Id == processId)
                    return i + 1;

            return null;
        }

        private string? GetProcessNameById(string processId)
        {
            if (string.IsNullOrWhiteSpace(processId)) return null;

            for (int i = 0; i < _processes.Count; i++)
                if (_processes[i].Id == processId)
                    return _processes[i].Name;

            return null;
        }

        private void dgvLayers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (e.RowIndex >= dgvLayers.Rows.Count) return;
                if (e.ColumnIndex >= dgvLayers.Columns.Count) return;

                var colName = dgvLayers.Columns[e.ColumnIndex].Name;
                var row = dgvLayers.Rows[e.RowIndex];
                if (row == null || row.IsNewRow) return;

                if (row.DataBoundItem is not LayerItem layer) return;

                // colLayerInfo: 1.1, 1.2, 1.3 ...
                if (colName == "colLayerInfo")
                {
                    e.Value = $"1.{e.RowIndex + 1}";
                    e.FormattingApplied = true;
                    return;
                }

                // ProcessNumber (lookup)
                if (colName == "colProcessNumber")
                {
                    var n = GetProcessNumberById(layer.ProcessId);
                    e.Value = n?.ToString() ?? "--";
                    e.FormattingApplied = true;
                    return;
                }

                // ProcessName (lookup)
                if (colName == "colProcessName")
                {
                    e.Value = GetProcessNameById(layer.ProcessId) ?? "--";
                    e.FormattingApplied = true;
                    return;
                }

                // StartMode şimdilik inaktif
                if (colName == "colStartMode")
                {
                    e.Value = "";
                    e.FormattingApplied = true;
                    return;
                }
            }
            catch
            {
                // UI stabil kalsın
            }
        }

        private async void btnInsertNormalLayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (_processes.Count == 0)
                {
                    MessageBox.Show("Önce Process eklemelisin.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new LayerEditDialog(_processes.ToList());
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                if (string.IsNullOrWhiteSpace(dlg.SelectedProcessId))
                {
                    MessageBox.Show("Process seçmelisin.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var item = new LayerItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProcessId = dlg.SelectedProcessId,
                    FinalThickness = dlg.FinalThickness,
                    Source = dlg.Source,
                    RF1SensorEnabled = dlg.RF1Enabled,
                    RF2SensorEnabled = dlg.RF2Enabled,
                    DCSensorEnabled = dlg.DCEnabled

                };

                _layers.Add(item);
                await SaveLayersAsyncSafe();

                dgvLayers.Invalidate();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }





        private async void btnEditLayer_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedLayer();
                if (sel == null)
                {
                    MessageBox.Show("Düzenlemek için bir layer seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_processes.Count == 0)
                {
                    MessageBox.Show("Process listesi boş. Önce Process eklemelisin.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var dlg = new LayerEditDialog(_processes.ToList(), sel);
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                if (string.IsNullOrWhiteSpace(dlg.SelectedProcessId))
                {
                    MessageBox.Show("Process seçmelisin.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                sel.ProcessId = dlg.SelectedProcessId;
                sel.FinalThickness = dlg.FinalThickness;
                sel.Source = dlg.Source;
                sel.RF1SensorEnabled = dlg.RF1Enabled;
                sel.RF2SensorEnabled = dlg.RF2Enabled;
                sel.DCSensorEnabled = dlg.DCEnabled;


                dgvLayers.Refresh();
                dgvLayers.Invalidate();

                await SaveLayersAsyncSafe();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }
        private async void btnDeleteLayer_Click(object sender, EventArgs e)
        {
            try
            {
                var sel = GetSelectedLayer();
                if (sel == null)
                {
                    MessageBox.Show("Silmek için bir layer seç.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show(
                    "Seçili layer silinsin mi?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                dgvLayers.SuspendLayout();
                _layers.Remove(sel);
                dgvLayers.ResumeLayout();

                await SaveLayersAsyncSafe();
                dgvLayers.Invalidate();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private LayerItem? GetSelectedLayer()
        {
            if (dgvLayers.CurrentRow?.DataBoundItem is LayerItem li)
                return li;

            if (dgvLayers.SelectedRows.Count > 0 &&
                dgvLayers.SelectedRows[0].DataBoundItem is LayerItem li2)
                return li2;

            return null;
        }

        private sealed class LayerEditDialog : Form
        {
            private readonly ComboBox cmbProcess = new ComboBox();
            private readonly NumericUpDown numFinalThickness = new NumericUpDown();
            private readonly ComboBox cmbSource = new ComboBox();
            private readonly Button btnOk = new Button();
            private readonly Button btnCancel = new Button();
            private readonly CheckBox chkRF1 = new CheckBox();
            private readonly CheckBox chkRF2 = new CheckBox();
            private readonly CheckBox chkDC = new CheckBox();

            public bool RF1Enabled => chkRF1.Checked;
            public bool RF2Enabled => chkRF2.Checked;
            public bool DCEnabled => chkDC.Checked;


            public string SelectedProcessId => (cmbProcess.SelectedValue?.ToString()) ?? "";
            public float FinalThickness => (float)numFinalThickness.Value;
            public DepositionSource Source => (DepositionSource)cmbSource.SelectedItem!;

            public LayerEditDialog(IList<ProcessItem> processes, LayerItem? initial = null)
            {
                Text = initial == null ? "Insert Layer" : "Edit Layer";
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 540;
                Height = 250;

                var lbl1 = new Label { Left = 16, Top = 18, Width = 140, Text = "Process" };
                cmbProcess.Left = 170; cmbProcess.Top = 14; cmbProcess.Width = 340;
                cmbProcess.DropDownStyle = ComboBoxStyle.DropDownList;

                var lbl2 = new Label { Left = 16, Top = 58, Width = 140, Text = "Final Thickness (kÅ)" };
                numFinalThickness.Left = 170; numFinalThickness.Top = 54; numFinalThickness.Width = 160;
                numFinalThickness.DecimalPlaces = 3;
                numFinalThickness.Minimum = 0;
                numFinalThickness.Maximum = 100000;
                numFinalThickness.Increment = new decimal(new int[] { 1, 0, 0, 65536 });

                var lbl3 = new Label { Left = 16, Top = 98, Width = 140, Text = "Source" };
                cmbSource.Left = 170; cmbSource.Top = 94; cmbSource.Width = 160;
                cmbSource.DropDownStyle = ComboBoxStyle.DropDownList;

                var lbl4 = new Label { Left = 16, Top = 138, Width = 140, Text = "Sensors" };

                chkRF1.Left = 170; chkRF1.Top = 134; chkRF1.Width = 60; chkRF1.Text = "RF1";
                chkRF2.Left = 240; chkRF2.Top = 134; chkRF2.Width = 60; chkRF2.Text = "RF2";
                chkDC.Left = 310; chkDC.Top = 134; chkDC.Width = 60; chkDC.Text = "DC";

                Controls.AddRange(new Control[] { lbl4, chkRF1, chkRF2, chkDC });

                // Defaults
                chkRF1.Checked = true;
                chkRF2.Checked = false;
                chkDC.Checked = false;

                btnOk.Text = "OK";
                btnOk.Left = 350; btnOk.Top = 160; btnOk.Width = 75;
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = "Cancel";
                btnCancel.Left = 435; btnCancel.Top = 160; btnCancel.Width = 75;
                btnCancel.DialogResult = DialogResult.Cancel;

                Controls.AddRange(new Control[] { lbl1, cmbProcess, lbl2, numFinalThickness, lbl3, cmbSource, btnOk, btnCancel });
                AcceptButton = btnOk;
                CancelButton = btnCancel;

                cmbProcess.DataSource = processes.ToList();
                cmbProcess.DisplayMember = nameof(ProcessItem.Name);
                cmbProcess.ValueMember = nameof(ProcessItem.Id);

                cmbSource.DataSource = Enum.GetValues(typeof(DepositionSource));

                // defaults
                cmbSource.SelectedItem = DepositionSource.RF1;


                // initial (Edit)
                if (initial != null)
                {
                    if (!string.IsNullOrWhiteSpace(initial.ProcessId))
                        cmbProcess.SelectedValue = initial.ProcessId;

                    numFinalThickness.Value = (decimal)Math.Clamp(initial.FinalThickness, 0, 100000);

                    cmbSource.SelectedItem = initial.Source;
                    chkRF1.Checked = initial.RF1SensorEnabled;
                    chkRF2.Checked = initial.RF2SensorEnabled;
                    chkDC.Checked = initial.DCSensorEnabled;

                }

                if (processes.Count == 0)
                {
                    btnOk.Enabled = false;
                    cmbProcess.Enabled = false;
                }
            }
        }

        private void BindGridShortcuts()
        {
            // Double click -> Edit
            dgvMaterials.CellDoubleClick -= dgvMaterials_CellDoubleClick;
            dgvMaterials.CellDoubleClick += dgvMaterials_CellDoubleClick;

            dgvProcesses.CellDoubleClick -= dgvProcesses_CellDoubleClick;
            dgvProcesses.CellDoubleClick += dgvProcesses_CellDoubleClick;

            dgvLayers.CellDoubleClick -= dgvLayers_CellDoubleClick;
            dgvLayers.CellDoubleClick += dgvLayers_CellDoubleClick;

            // Delete key -> Delete
            dgvMaterials.KeyDown -= dgvMaterials_KeyDown;
            dgvMaterials.KeyDown += dgvMaterials_KeyDown;

            dgvProcesses.KeyDown -= dgvProcesses_KeyDown;
            dgvProcesses.KeyDown += dgvProcesses_KeyDown;

            dgvLayers.KeyDown -= dgvLayers_KeyDown;
            dgvLayers.KeyDown += dgvLayers_KeyDown;
        }
        private void dgvMaterials_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // header
            btnMatEdit_Click(sender!, EventArgs.Empty);
        }

        private void dgvMaterials_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                btnMatDelete_Click(sender!, EventArgs.Empty);
            }
        }
        private void dgvProcesses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            btnProcEdit_Click(sender!, EventArgs.Empty);
        }

        private void dgvProcesses_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                btnProcDelete_Click(sender!, EventArgs.Empty);
            }
        }
        private void dgvLayers_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            btnEditLayer_Click(sender!, EventArgs.Empty);
        }

        private void dgvLayers_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                btnDeleteLayer_Click(sender!, EventArgs.Empty);
            }
        }







        // ===============================
        // Layer Detail Panel
        // ===============================

        private int _activeLayerRowIndex = -1;
        private bool _layerRunning = false;

        // 3 sensör için client (istersen MultiStm2Manager’a geçiririz)
        private Stm2Client? _stmRf1;
        private Stm2Client? _stmRf2;
        private Stm2Client? _stmDc;

        // ✅ Sensör seri numaraları (macro gibi tek yerden değişsin)
        private const string STM2_SN_RF1 = "700950";
        private const string STM2_SN_RF2 = "700918";
        private const string STM2_SN_DC = "700969";

        private void UpdateLayerUiState()
        {
            bool hasLayerSelected = (_activeLayerRowIndex >= 0 && _activeLayerRowIndex < _layers.Count);
            bool hasAnyStmConnected = (_stmRf1?.IsOpen == true) || (_stmRf2?.IsOpen == true) || (_stmDc?.IsOpen == true);
            bool isChillerOn = string.Equals(btnChiller.Text, "START", StringComparison.OrdinalIgnoreCase);
            // Toggle buton görünümü
            if (_layerRunning)
            {
                btnStartLayer.Text = "Stop Layer";
                btnStartLayer.Enabled = isChillerOn; // running sırasında stop edebilsin (chiller açıksa)

                // ✅ STOP görünümü
                btnStartLayer.BackColor = Color.IndianRed;
                btnStartLayer.ForeColor = Color.White;
            }
            else
            {
                btnStartLayer.Text = "Start Layer";
                btnStartLayer.Enabled = hasLayerSelected && isChillerOn; // && hasAnyStmConnected;

                // ✅ START görünümü
                btnStartLayer.BackColor = Color.ForestGreen;
                btnStartLayer.ForeColor = Color.White;
            }
            // Next: running değilken ve next varsa
            bool hasNext = hasLayerSelected && (_activeLayerRowIndex + 1 < _layers.Count);
            btnNextLayer.Enabled = !_layerRunning && hasNext;

            // Zero: hep aktif olsun (running sırasında da)
            // Ama seçili layer yoksa kapatalım; sensör yoksa da kapatmak daha güvenli
            if (hasLayerSelected)
            {
                var layer = _layers[_activeLayerRowIndex];
                bool anySelectedSensorConnected =
                    (layer.RF1SensorEnabled && _stmRf1?.IsOpen == true) ||
                    (layer.RF2SensorEnabled && _stmRf2?.IsOpen == true) ||
                    (layer.DCSensorEnabled && _stmDc?.IsOpen == true);

                btnZeroThickness.Enabled = anySelectedSensorConnected;
            }
            else
            {
                btnZeroThickness.Enabled = false;
            }

            // Running sırasında layer selection kilit
            dgvLayers.Enabled = !_layerRunning;
            cmbLayerDetailTitle.Enabled = !_layerRunning;
            btnEditLayer.Enabled = !_layerRunning;
            btnDeleteLayer.Enabled = !_layerRunning;
            // İstersen layer CRUD da kilitlenebilir:
            // btnInsertNormalLayer.Enabled = !_layerRunning;
        }
        // Aktif layer için seçili ve bağlı sensörleri getir
        private IEnumerable<(string tag, Stm2Client client)> GetEnabledStmClientsForLayer(LayerItem layer)
        {
            if (layer.RF1SensorEnabled && _stmRf1?.IsOpen == true) yield return ("RF1", _stmRf1);
            if (layer.RF2SensorEnabled && _stmRf2?.IsOpen == true) yield return ("RF2", _stmRf2);
            if (layer.DCSensorEnabled && _stmDc?.IsOpen == true) yield return ("DC", _stmDc);
        }
        // Ping mesajından seri numarasını çıkart
        private static string? TryExtractSerialNumber(string pingMsg)
        {
            if (string.IsNullOrWhiteSpace(pingMsg)) return null;

            // Ping mesajında 700950 gibi 6 haneli seri numarası geçiyorsa yakala
            var m = System.Text.RegularExpressions.Regex.Match(pingMsg, @"\b\d{6}\b");
            return m.Success ? m.Value : null;
        }

        // Bazı STM-2 cihazları COM açıldıktan hemen sonra SN komutuna geç cevap verebilir.
        // Bu yüzden küçük bir warm-up beklemesi + kademeli timeout ile birkaç deneme yapar.
        private static async Task<string?> TryReadSerialNumberWithRetryAsync(Stm2Client client, CancellationToken ct = default)
        {
            int[] timeouts = { 1200, 1800, 2500 };

            for (int i = 0; i < timeouts.Length; i++)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var ping = await client.getSNAsync(timeoutMs: timeouts[i], ct).ConfigureAwait(false);
                    var sn = TryExtractSerialNumber(ping);

                    if (!string.IsNullOrWhiteSpace(sn))
                        return sn;
                }
                catch (TimeoutException)
                {
                    // Cihaz meşgul olabilir, bir sonraki denemeye geç.
                }
                catch (IOException)
                {
                    // Port geçici olarak cevap üretmediyse yeniden dene.
                }

                // Son deneme değilse kısa bekle; cihazın hazır olmasına fırsat ver.
                if (i < timeouts.Length - 1)
                    await Task.Delay(200, ct).ConfigureAwait(false);
            }

            return null;
        }
        // Tüm COM portları tarayıp STM-2 sensörlerini otomatik bağla
        private async Task AutoConnectStm2BySerialAsync(bool showWarningDialog = true)
        {
            // Zaten bağlıysa tekrar bağlanma
            if ((_stmRf1?.IsOpen == true) || (_stmRf2?.IsOpen == true) || (_stmDc?.IsOpen == true))
                return;

            var found = await Task.Run(async () =>
            {
                var ports = System.IO.Ports.SerialPort.GetPortNames();
                Array.Sort(ports);

                var scanResult = new Dictionary<string, Stm2Client>(); // SN -> client

                foreach (var port in ports)
                {
                    // aynı SN’den 2 tane çıkarsa ilkini al
                    try
                    {
                        var c = new Stm2Client(port);
                        c.Open();

                        // Bazı USB-Serial adaptörlerinde open sonrası cihazın hazır olması gecikebiliyor.
                        await Task.Delay(150).ConfigureAwait(false);

                        var sn = await TryReadSerialNumberWithRetryAsync(c).ConfigureAwait(false);

                        if (!string.IsNullOrWhiteSpace(sn))
                        {
                            if (!scanResult.ContainsKey(sn))
                                scanResult[sn] = c;
                            else
                                c.Dispose(); // duplicate -> kapat
                        }
                        else
                        {
                            // SN yoksa bizim için cihaz değil: kapat
                            c.Dispose();
                        }
                    }
                    catch
                    {
                        // bu portta STM2 yok / erişim yok / timeout -> geç
                    }
                }

                return scanResult;
            }).ConfigureAwait(true);

            found.TryGetValue(STM2_SN_RF1, out _stmRf1);
            found.TryGetValue(STM2_SN_RF2, out _stmRf2);
            found.TryGetValue(STM2_SN_DC, out _stmDc);

            foreach (var kvp in found)
            {
                if (kvp.Key != STM2_SN_RF1 && kvp.Key != STM2_SN_RF2 && kvp.Key != STM2_SN_DC)
                    kvp.Value.Dispose();
            }

            // ✅ Uyarı: eksik sensör varsa ekrana bas
            var missing = new List<string>();
            if (_stmRf1 == null) missing.Add($"RF1({STM2_SN_RF1})");
            if (_stmRf2 == null) missing.Add($"RF2({STM2_SN_RF2})");
            if (_stmDc == null) missing.Add($"DC({STM2_SN_DC})");

            if (missing.Count > 0)
            {
                var msg =
                    "STM-2 sensör bağlantısı kurulamadı:\n" +
                    string.Join("\n", missing) +
                    "\n\nCOM portlar değişmiş olabilir veya cihaz kapalı olabilir.";

                PostStatus("STM-2 missing: " + string.Join(", ", missing));
                PostQcmStatus("QCM missing: " + string.Join(", ", missing));

                if (showWarningDialog)
                    MessageBox.Show(msg, "STM-2 Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                PostStatus($"STM-2 OK: RF1={_stmRf1!.PortName}, RF2={_stmRf2!.PortName}, DC={_stmDc!.PortName}");
                PostQcmStatus($"QCM OK: RF1={_stmRf1!.PortName}, RF2={_stmRf2!.PortName}, DC={_stmDc!.PortName}");
            }
        }
        // Layer selection değiştiğinde detay paneli güncelle
        private void dgvLayers_SelectionChanged(object? sender, EventArgs e)
        {
            if (_layerRunning) return; // start sonrası selection değişmesin

            if (dgvLayers.CurrentRow?.DataBoundItem is not LayerItem layer)
                return;

            //Plot’u seçilen layer’ın final thickness’ına grafik için ayarla
            //MainForm.Plot.cs içerisinde yer alıyor, ama burada çağırmak mantıklı çünkü layer değiştiğinde güncellememiz gerekiyor
            ThicknessPlot_SetTarget(layer.FinalThickness);

            _activeLayerRowIndex = dgvLayers.CurrentRow.Index;
            ApplyActiveLayerDetail(layer, _activeLayerRowIndex);
            UpdateLayerUiState();
        }
        // Layer detay panelini doldur
        private void ApplyActiveLayerDetail(LayerItem layer, int rowIndex)
        {
            // Layer title: "Layer 1.1 - ProcessName"
            string layerName = $"Layer 1.{rowIndex + 1}";

            var proc = _processes.FirstOrDefault(p => p.Id == layer.ProcessId);
            var procName = proc?.Name ?? "--";

            cmbLayerDetailTitle.Text = $"{layerName} - {procName}";

            // Material + process lookups
            var mat = (proc != null) ? _materials.FirstOrDefault(m => m.Id == proc.MaterialId) : null;

            lblLdMaterial.Text = mat?.Name ?? "--";
            lblLdDensity.Text = mat != null ? mat.Density.ToString("0.###") : "--";
            lblLdZRatio.Text = mat != null ? mat.ZRatio.ToString("0.###") : "--";

            lblLdTooling.Text = proc != null ? proc.Tooling.ToString("0.###") : "--";

            lblLdFinalThk.Text = layer.FinalThickness.ToString("0.###");
            lblLdSource.Text = layer.Source.ToString();

            lblLdStartMode.Text = "-"; // şimdilik
        }
        // Seçilen layer için sensörlerin bağlı olup olmadığını kontrol et
        private bool ValidateLayerSensorsConnected(LayerItem layer)
        {
            var wanted = new List<string>();
            if (layer.RF1SensorEnabled) wanted.Add("RF1");
            if (layer.RF2SensorEnabled) wanted.Add("RF2");
            if (layer.DCSensorEnabled) wanted.Add("DC");

            var available = GetEnabledStmClientsForLayer(layer).Select(x => x.tag).ToHashSet();

            // ✅ Primary thickness için source sensörü zorunlu
            string primaryTag = layer.Source.ToString(); // "RF1"/"RF2"/"DC"
            bool primaryOk =
                (layer.Source == DepositionSource.RF1 && layer.RF1SensorEnabled && _stmRf1?.IsOpen == true) ||
                (layer.Source == DepositionSource.RF2 && layer.RF2SensorEnabled && _stmRf2?.IsOpen == true) ||
                (layer.Source == DepositionSource.DC && layer.DCSensorEnabled && _stmDc?.IsOpen == true);

            if (!primaryOk)
            {
                MessageBox.Show(
                    $"Primary sensor (Source={primaryTag}) seçili ve bağlı olmalı.",
                    "STM-2 Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }


            var missing = wanted.Where(w => !available.Contains(w)).ToList();
            if (missing.Count == 0) return true;

            MessageBox.Show(
                "Seçilen sensör(ler) bağlı değil: " + string.Join(", ", missing),
                "STM-2 Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }
        // Start/Stop layer button
        private async void btnStartLayer_Click(object sender, EventArgs e)
        {
            // Toggle: running ise STOP, değilse START
            if (_layerRunning)
            {
                await StopLayerAsync(manual: true);
                return;
            }

            try
            {
                if (_activeLayerRowIndex < 0 || _activeLayerRowIndex >= _layers.Count)
                    return;

                var layer = _layers[_activeLayerRowIndex];

                // Seçilen sensörler bağlı mı? (source sensörü de zorunlu)
                // TEST: sensör bağlılık kontrolünü bypass
                // if (!ValidateLayerSensorsConnected(layer))
                //     return;


                // Lookups
                var proc = _processes.FirstOrDefault(p => p.Id == layer.ProcessId);
                var mat = (proc != null) ? _materials.FirstOrDefault(m => m.Id == proc.MaterialId) : null;
                if (proc == null || mat == null)
                {
                    MessageBox.Show("Process/Material bulunamadı.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // önce kaynağı başlat (StopLayer ile simetrik)
                await ForceStartDepositionSourceAsync(layer.Source);

                // Running state
                _autoStopTriggered = false;
                _layerRunning = true;
                UpdateLayerUiState();
                UiLog($"[QCM RAW] Layer start: row={_activeLayerRowIndex + 1}, source={layer.Source}, targetKA={FormatLogFloat(layer.FinalThickness)}, sensors RF1={layer.RF1SensorEnabled}, RF2={layer.RF2SensorEnabled}, DC={layer.DCSensorEnabled}");

                // STM'lere yaz
                float density = mat.Density;
                float zratio = mat.ZRatio;
                float tooling = proc.Tooling;


                foreach (var (_, client) in GetEnabledStmClientsForLayer(layer))
                {
                    await client.WriteDensityAsync(density);
                    await client.WriteZRatioAsync(zratio);
                    await client.WriteToolingAsync(tooling);
                    client.ZeroThickness_NoWait();
                }

                PostStatus("Layer started (params written) and Zero thickness.");
                PostQcmStatus($"QCM layer started: {layer.Source}");
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                _layerRunning = false;
                UpdateLayerUiState();
                PostQcmStatus($"QCM layer start failed: {ex.Message}");
            }
        }
        // Next layer button
        private void btnNextLayer_Click(object sender, EventArgs e)
        {
            if (_layerRunning) return;

            int next = _activeLayerRowIndex + 1;
            if (next < 0 || next >= dgvLayers.Rows.Count) return;

            dgvLayers.ClearSelection();
            dgvLayers.Rows[next].Selected = true;
            dgvLayers.CurrentCell = dgvLayers.Rows[next].Cells[0];

            _activeLayerRowIndex = next;

            if (dgvLayers.CurrentRow?.DataBoundItem is LayerItem layer)
                ApplyActiveLayerDetail(layer, _activeLayerRowIndex);

            UpdateLayerUiState();
        }
        // Zero Thickness button
        private void btnZeroThickness_Click(object sender, EventArgs e)
        {
            try
            {
                if (_activeLayerRowIndex < 0 || _activeLayerRowIndex >= _layers.Count)
                    return;

                var layer = _layers[_activeLayerRowIndex];

                foreach (var (_, client) in GetEnabledStmClientsForLayer(layer))
                {
                    client.ZeroThickness_NoWait();
                }

                PostStatus("Zero Thickness sent.");
                PostQcmStatus("QCM zero thickness sent");
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                PostQcmStatus($"QCM zero failed: {ex.Message}");
            }
        }
        // Polling method called by timer
        private async Task PollStm2SensorsForActiveLayerAsync()
        {
            if (!_layerRunning) return;
            if (_activeLayerRowIndex < 0 || _activeLayerRowIndex >= _layers.Count) return;

            var layer = _layers[_activeLayerRowIndex];

            // 1) UI update
            // Rate: tum acik sensorlerden okunur
            // Thickness: sadece secili sensorlerden okunur
            var tasks = new List<Task>();

            if (_stmRf1?.IsOpen == true)
                tasks.Add(UpdateSensorUiAsync(_stmRf1, "RF1", includeThickness: layer.RF1SensorEnabled));

            if (_stmRf2?.IsOpen == true)
                tasks.Add(UpdateSensorUiAsync(_stmRf2, "RF2", includeThickness: layer.RF2SensorEnabled));

            if (_stmDc?.IsOpen == true)
                tasks.Add(UpdateSensorUiAsync(_stmDc, "DC", includeThickness: layer.DCSensorEnabled));

            await Task.WhenAll(tasks);

            // 2) Auto-stop: Source sensörü thickness'i FinalThickness ile karşılaştır
            if (_autoStopTriggered) return;

            var primary = GetPrimaryClientBySource(layer);
            if (primary == null || primary.IsOpen != true)
            {
                UiLog($"[QCM RAW] AutoStop skipped: source={layer.Source}, sourceSensorEnabled={IsLayerSourceSensorEnabled(layer)}, primaryOpen={(primary?.IsOpen == true)}, targetKA={FormatLogFloat(layer.FinalThickness)}");
                return;
            }

            string thkStr = await primary.GetThicknessAsync(1200);
            if (!TryParseFirstFloat(thkStr, out float thk))
            {
                UiLog($"[QCM RAW] AutoStop parse failed: source={layer.Source}, rawThk='{thkStr}', targetKA={FormatLogFloat(layer.FinalThickness)}");
                return;
            }

            // STM-2 thickness raw unit = Å, UI/Layer target unit = kÅ
            float thkKa = AngstromToKA(thk);
            float target = layer.FinalThickness;
            bool targetReached = thkKa >= target;
            UiLog($"[QCM RAW] AutoStop source={layer.Source}, rawThk='{thkStr}', parsedA={FormatLogFloat(thk)}, thkKA={FormatLogFloat(thkKa)}, targetKA={FormatLogFloat(target)}, reached={targetReached}");

            //Grafik güncellemesi UI thread zorunluluğu: Plot update UI control olduğu için
            if (InvokeRequired)
                BeginInvoke(new Action(() => ThicknessPlot_AddPoint(DateTime.Now, thkKa)));
            else
                ThicknessPlot_AddPoint(DateTime.Now, thkKa);

            if (targetReached)
            {
                _autoStopTriggered = true;

                if (InvokeRequired)
                    BeginInvoke(new Action(() => ThicknessPlot_SetTargetReached(true)));
                else
                    ThicknessPlot_SetTargetReached(true);

                if (InvokeRequired)
                {
                    BeginInvoke(new Action(async () =>
                    {
                        await StopLayerAsync(manual: false);
                        AdvanceToNextLayerIfExists();
                    }));
                }
                else
                {
                    await StopLayerAsync(manual: false);
                    AdvanceToNextLayerIfExists();
                }

                return;
            }

            //Rate güncellemesi: Source sensöründen oku ve plot’a ekle
            string rateStr = await primary.GetRateAsync(1200);
            if (TryParseFirstFloat(rateStr, out float rate))
            {
                // UI thread zorunluluğu: Plot update UI control olduğu için
                if (InvokeRequired)
                    BeginInvoke(new Action(() => RatePlot_AddPoint(DateTime.Now, rate)));
                else
                    RatePlot_AddPoint(DateTime.Now, rate);

                PostQcmStatus($"QCM active {layer.Source}: rate={rate:0.###}, thk={thkKa:0.###} kA");
            }

        }
        // Bir sensorden her zaman rate, istenirse thickness oku ve UI guncelle
        private async Task UpdateSensorUiAsync(Stm2Client client, string which, bool includeThickness)
        {
            int timeout = 1200;

            string rate = await client.GetRateAsync(timeout);
            string? thk = null;
            if (includeThickness)
                thk = await client.GetThicknessAsync(timeout);

            UiLog(includeThickness
                ? $"[QCM RAW] {which}: rateRaw='{rate}', thkRaw='{thk ?? ""}'"
                : $"[QCM RAW] {which}: rateRaw='{rate}', thkRaw=<not-selected>");

            // UI thread’e dön
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    ApplySensorRateLabel(which, rate);
                    if (includeThickness && thk != null)
                        ApplySensorThicknessLabel(which, thk);
                }));
            }
            else
            {
                ApplySensorRateLabel(which, rate);
                if (includeThickness && thk != null)
                    ApplySensorThicknessLabel(which, thk);
            }

            if (includeThickness && thk != null)
            {
                var thicknessUi = TryParseFirstFloat(thk, out float angstrom)
                    ? $"{AngstromToKA(angstrom):0.###} kA"
                    : thk;
                PostQcmStatus($"QCM {which}: rate={rate}, thk={thicknessUi}");
            }
            else
            {
                PostQcmStatus($"QCM {which}: rate={rate}");
            }
        }

        private async Task PollStm2LifeForAllSensorsAsync()
        {
            var now = DateTime.UtcNow;
            if (now < _nextLifePollUtc) return;
            _nextLifePollUtc = now.AddSeconds(5);

            var tasks = new List<Task>();

            if (_stmRf1?.IsOpen == true) tasks.Add(UpdateSensorLifeUiAsync(_stmRf1, "RF1"));
            if (_stmRf2?.IsOpen == true) tasks.Add(UpdateSensorLifeUiAsync(_stmRf2, "RF2"));
            if (_stmDc?.IsOpen == true) tasks.Add(UpdateSensorLifeUiAsync(_stmDc, "DC"));

            await Task.WhenAll(tasks);
        }

        private async Task PollStm2RatesForAllSensorsWhenIdleAsync()
        {
            if (_layerRunning) return;

            var now = DateTime.UtcNow;
            if (now < _nextIdleRatePollUtc) return;

            var tasks = new List<Task>();

            if (_stmRf1?.IsOpen == true) tasks.Add(UpdateSensorRateOnlyUiAsync(_stmRf1, "RF1"));
            if (_stmRf2?.IsOpen == true) tasks.Add(UpdateSensorRateOnlyUiAsync(_stmRf2, "RF2"));
            if (_stmDc?.IsOpen == true) tasks.Add(UpdateSensorRateOnlyUiAsync(_stmDc, "DC"));

            if (tasks.Count == 0) return;

            _nextIdleRatePollUtc = now.AddSeconds(2);

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                PostQcmStatus($"QCM idle rate poll failed: {ex.Message}");
            }
        }

        private async Task UpdateSensorRateOnlyUiAsync(Stm2Client client, string which)
        {
            int timeout = 1200;
            string rate = await client.GetRateAsync(timeout);

            if (InvokeRequired)
                BeginInvoke(new Action(() => ApplySensorRateLabel(which, rate)));
            else
                ApplySensorRateLabel(which, rate);
        }

        private async Task UpdateSensorLifeUiAsync(Stm2Client client, string which)
        {
            int timeout = 1800;
            string life = await client.GetLifeAsync(timeout);

            if (InvokeRequired)
                BeginInvoke(new Action(() => ApplySensorLifeLabel(which, life)));
            else
                ApplySensorLifeLabel(which, life);

            PostQcmStatus($"QCM {which} life: {life}");
        }
        // Sensör değerlerini ilgili label’lara uygula
        private void ApplySensorLabels(string which, string rate, string thickness)
        {
            switch (which)
            {
                case "RF1":
                    ApplySensorRateLabel(which, rate);
                    ApplySensorThicknessLabel(which, thickness);
                    break;

                case "RF2":
                    ApplySensorRateLabel(which, rate);
                    ApplySensorThicknessLabel(which, thickness);
                    break;

                case "DC":
                    ApplySensorRateLabel(which, rate);
                    ApplySensorThicknessLabel(which, thickness);
                    break;
            }
        }

        private void ApplySensorLifeLabel(string which, string life)
        {
            switch (which)
            {
                case "RF1":
                    lblValue_Rf1CrystalLife.Text = life;
                    lblValue2_Rf1Life.Text = life;
                    break;
                case "RF2":
                    lblValue_Rf2CrystalLife.Text = life;
                    lblValue2_Rf2Life.Text = life;
                    break;
                case "DC":
                    lblValue_DcCrystalLife.Text = life;
                    lblValue2_DcLife.Text = life;
                    break;
            }
        }

        private void ApplySensorRateLabel(string which, string rate)
        {
            switch (which)
            {
                case "RF1":
                    lblValue_Rf1Rate.Text = rate;
                    lblValue2_Rf1Rate.Text = rate;
                    break;

                case "RF2":
                    lblValue_Rf2Rate.Text = rate;
                    lblValue2_Rf2Rate.Text = rate;
                    break;

                case "DC":
                    lblValue_DcRate.Text = rate;
                    lblValue2_DcRate.Text = rate;
                    break;
            }
        }

        private void ApplySensorThicknessLabel(string which, string thickness)
        {
            var thicknessUi = TryParseFirstFloat(thickness, out float angstrom)
                ? $"{AngstromToKA(angstrom):0.###} kÅ"
                : thickness;

            switch (which)
            {
                case "RF1":
                    lblValue_Rf1Thickness.Text = thicknessUi;
                    lblValue2_Rf1Thickness.Text = thicknessUi;
                    break;

                case "RF2":
                    lblValue_Rf2Thickness.Text = thicknessUi;
                    lblValue2_Rf2Thickness.Text = thicknessUi;
                    break;

                case "DC":
                    lblValue_DcThickness.Text = thicknessUi;
                    lblValue2_DcThickness.Text = thicknessUi;
                    break;
            }
        }
        // Source sensörüne göre primary client getir
        private Stm2Client? GetPrimaryClientBySource(LayerItem layer)
        {
            return layer.Source switch
            {
                DepositionSource.RF1 => (layer.RF1SensorEnabled ? _stmRf1 : null),
                DepositionSource.RF2 => (layer.RF2SensorEnabled ? _stmRf2 : null),
                DepositionSource.DC => (layer.DCSensorEnabled ? _stmDc : null),
                _ => null
            };
        }

        private static bool IsLayerSourceSensorEnabled(LayerItem layer)
        {
            return layer.Source switch
            {
                DepositionSource.RF1 => layer.RF1SensorEnabled,
                DepositionSource.RF2 => layer.RF2SensorEnabled,
                DepositionSource.DC => layer.DCSensorEnabled,
                _ => false
            };
        }
        // İlk float sayıyı string’den ayrıştır
        private static bool TryParseFirstFloat(string s, out float value)
        {
            value = 0f;
            if (string.IsNullOrWhiteSpace(s)) return false;

            // İçinde bir yerde sayı varsa yakala (örn: "1.234", "  0.56kA", "-0.12")
            var m = System.Text.RegularExpressions.Regex.Match(s, @"[-+]?\d+(?:[.,]\d+)?");
            if (!m.Success) return false;

            var token = m.Value.Replace(',', '.');
            return float.TryParse(token, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out value);
        }

        private async Task ForceStartDepositionSourceAsync(DepositionSource source)
        {
            if (_modbus == null)
            {
                UiLog("[ForceON] _modbus is null");
                return;
            }

            ushort cmdAddr = source switch
            {
                DepositionSource.RF1 => (ushort)RegisterId.RF1_Control_Command,
                DepositionSource.RF2 => (ushort)RegisterId.RF2_Control_Command,
                DepositionSource.DC => (ushort)RegisterId.DC_Control_Command
            };

            try
            {
                await _modbus.WriteSingleRegisterAsync(cmdAddr, 1);
                UiLog($"[ForceON] {source} START command sent");
            }
            catch (Exception ex)
            {
                UiLog($"[ForceON] ERROR writing START to {source}: {ex.Message}");
            }
        }

        private async Task ForceStopDepositionSourceAsync(DepositionSource source)
        {
            if (_modbus == null)
            {
                UiLog("[ForceOFF] _modbus is null");
                return;
            }

            ushort cmdAddr = source switch
            {
                DepositionSource.RF1 => (ushort)RegisterId.RF1_Control_Command,
                DepositionSource.RF2 => (ushort)RegisterId.RF2_Control_Command,
                DepositionSource.DC => (ushort)RegisterId.DC_Control_Command
            };

            try
            {
                await _modbus.WriteSingleRegisterAsync(cmdAddr, 0);
                UiLog($"[ForceOFF] {source} STOP command sent");
            }
            catch (Exception ex)
            {
                UiLog($"[ForceOFF] ERROR writing STOP to {source}: {ex.Message}");
            }
        }



        // Layer stop işlemi
        private async Task StopLayerAsync(bool manual)
        {
            UiLog($"[StopLayer] ENTER manual={manual} layerRunning={_layerRunning} activeRow={_activeLayerRowIndex}");

            // aktif layer kaynağını belirle
            DepositionSource? src = null;
            if (_activeLayerRowIndex >= 0 && _activeLayerRowIndex < _layers.Count)
            {
                src = _layers[_activeLayerRowIndex].Source;
                UiLog($"[StopLayer] source={src} -> send STOP command");
            }
            else
            {
                UiLog("[StopLayer] No active layer -> skip ForceOFF");
            }

            // önce kaynağı durdur
            if (src != null)
                await ForceStopDepositionSourceAsync(src.Value);

            // sonra yazılım state
            _layerRunning = false;
            UpdateLayerUiState();
            PostQcmStatus(manual ? "QCM layer stopped manually" : "QCM layer stopped automatically");

            UiLog(manual ? "[StopLayer] EXIT (manual)" : "[StopLayer] EXIT (auto)");
        }



        // Bir Next layer'e geç
        private void AdvanceToNextLayerIfExists()
        {
            int next = _activeLayerRowIndex + 1;
            if (next < 0 || next >= _layers.Count) return;

            _activeLayerRowIndex = next;

            // grid selection
            if (next < dgvLayers.Rows.Count)
            {
                dgvLayers.ClearSelection();
                dgvLayers.Rows[next].Selected = true;
                dgvLayers.CurrentCell = dgvLayers.Rows[next].Cells[0];
            }

            // detail update
            var layer = _layers[next];
            ApplyActiveLayerDetail(layer, next);

            UpdateLayerUiState();
        }

        private void DisposeStm2Clients()
        {
            try { _stmRf1?.Dispose(); } catch { }
            try { _stmRf2?.Dispose(); } catch { }
            try { _stmDc?.Dispose(); } catch { }

            _stmRf1 = null;
            _stmRf2 = null;
            _stmDc = null;
        }

    }
}
