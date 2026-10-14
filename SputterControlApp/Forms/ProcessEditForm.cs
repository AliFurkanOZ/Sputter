using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static SputterControlApp.Core.Models.Models;

namespace SputterControlApp.Forms
{
    public sealed partial class ProcessEditForm : Form
    {
        private sealed class MaterialPick
        {
            public string Id { get; init; } = "";
            public string Number { get; init; } = "";
            public string Name { get; init; } = "";
            public override string ToString() => $"{Number} - {Name}";
        }

        private readonly TextBox _txtName = new() { Width = 220 };

        private readonly ComboBox _cmbMaterial = new()
        {
            Width = 220,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        private readonly NumericUpDown _numTooling = new()
        {
            DecimalPlaces = 0,
            Minimum = 0,
            Maximum = 300,
            Width = 120
        };

        // ✅ Shown'da seçimi yapabilmek için field olarak tutuyoruz
        private readonly List<MaterialPick> _picks;
        private readonly ProcessItem? _existing;
        private readonly Button _btnOk;

        public ProcessItem Result { get; private set; } = new();

        public ProcessEditForm(
            string title,
            IList<MaterialItem> materials,
            ProcessItem? existing = null)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(480, 210);

            _existing = existing;

            // ✅ Picks oluştur
            _picks = materials
                .Select((m, i) => new MaterialPick
                {
                    Id = m.Id ?? "",
                    Number = (i + 1).ToString(),
                    Name = m.Name ?? ""
                })
                .Where(p => !string.IsNullOrWhiteSpace(p.Id) && !string.IsNullOrWhiteSpace(p.Name))
                .ToList();

            // ✅ Labels
            var lblName = new Label { Text = "Process Name", AutoSize = true };
            var lblMat = new Label { Text = "Material Number", AutoSize = true };
            var lblTool = new Label { Text = "Tooling (%)", AutoSize = true };

            // ✅ Buttons
            _btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

            // ✅ Layout
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(12)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            grid.Controls.Add(lblName, 0, 0);
            grid.Controls.Add(_txtName, 1, 0);

            grid.Controls.Add(lblMat, 0, 1);
            grid.Controls.Add(_cmbMaterial, 1, 1);

            grid.Controls.Add(lblTool, 0, 2);
            grid.Controls.Add(_numTooling, 1, 2);

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12),
                Height = 52
            };
            pnlButtons.Controls.Add(_btnOk);
            pnlButtons.Controls.Add(btnCancel);

            Controls.Add(grid);
            Controls.Add(pnlButtons);

            AcceptButton = _btnOk;
            CancelButton = btnCancel;

            // ✅ Combo bind
            _cmbMaterial.DataSource = _picks;

            // ✅ Edit mode alanlarını doldur (combo seçimini Shown'da yapacağız)
            if (_existing != null)
            {
                _txtName.Text = _existing.Name ?? "";

                var t = _existing.Tooling;
                if (t < 0) t = 0;
                if (t > 300) t = 300;
                _numTooling.Value = (decimal)t;
            }

            // ✅ Material yoksa OK disable + uyarı
            if (_picks.Count == 0)
            {
                _btnOk.Enabled = false;
                MessageBox.Show(
                    "Material listesi boş. Önce Material eklemelisin.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // ✅ Handle garanti: combo SelectedIndex'i Shown'da ayarla
            this.Shown += ProcessEditForm_Shown;

            // ✅ OK click
            _btnOk.Click += (_, __) =>
            {
                var name = (_txtName.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Process Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                if (_cmbMaterial.SelectedItem is not MaterialPick pick ||
                    string.IsNullOrWhiteSpace(pick.Id) ||
                    string.IsNullOrWhiteSpace(pick.Number))
                {
                    MessageBox.Show("Material seçmelisin.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                Result = new ProcessItem
                {
                    Name = name,
                    MaterialId = pick.Id,          // ✅ asıl link
                    MaterialNumber = pick.Number,  // ✅ display/legacy
                    Tooling = (float)_numTooling.Value
                };
            };
        }

        private void ProcessEditForm_Shown(object? sender, EventArgs e)
        {
            this.Shown -= ProcessEditForm_Shown;

            if (_cmbMaterial.Items.Count <= 0) return;

            // Edit: MaterialId ile seç, fallback MaterialNumber
            if (_existing != null)
            {
                int idx = -1;

                if (!string.IsNullOrWhiteSpace(_existing.MaterialId))
                    idx = _picks.FindIndex(p => p.Id == _existing.MaterialId);

                if (idx < 0 && !string.IsNullOrWhiteSpace(_existing.MaterialNumber))
                    idx = _picks.FindIndex(p => p.Number == _existing.MaterialNumber);

                _cmbMaterial.SelectedIndex = (idx >= 0) ? idx : 0;
            }
            else
            {
                // Add: ilk eleman
                _cmbMaterial.SelectedIndex = 0;
            }
        }
    }
}
