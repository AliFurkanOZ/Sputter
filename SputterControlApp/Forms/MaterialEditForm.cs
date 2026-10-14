using System;
using System.Drawing;
using System.Windows.Forms;

namespace SputterControlApp.Forms
{
    public sealed partial class MaterialEditForm : Form
    {
        private readonly TextBox _txtName = new() { Width = 220 };
        private readonly NumericUpDown _numDensity = new()
        {
            DecimalPlaces = 3,
            Minimum = 0,
            Maximum = 1000,
            Width = 120,
            Increment = 0.001M
        };
        private readonly NumericUpDown _numZ = new()
        {
            DecimalPlaces = 3,
            Minimum = 0,
            Maximum = 1000,
            Width = 120,
            Increment = 0.001M
        };

        public MaterialItem Result { get; private set; } = new();

        public MaterialEditForm(string title, MaterialItem? existing = null)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 180);

            var lblName = new Label { Text = "Material Name", AutoSize = true };
            var lblDen = new Label { Text = "Density (g/cm³)", AutoSize = true };
            var lblZ = new Label { Text = "Z-Ratio", AutoSize = true };

            var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
            var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(12),
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            grid.Controls.Add(lblName, 0, 0);
            grid.Controls.Add(_txtName, 1, 0);

            grid.Controls.Add(lblDen, 0, 1);
            grid.Controls.Add(_numDensity, 1, 1);

            grid.Controls.Add(lblZ, 0, 2);
            grid.Controls.Add(_numZ, 1, 2);

            var pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12),
                Height = 48
            };
            pnlButtons.Controls.Add(btnOk);
            pnlButtons.Controls.Add(btnCancel);

            Controls.Add(grid);
            Controls.Add(pnlButtons);

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            if (existing != null)
            {
                _txtName.Text = existing.Name;
                _numDensity.Value = (decimal)existing.Density;
                _numZ.Value = (decimal)existing.ZRatio;
            }

            btnOk.Click += (_, __) =>
            {
                var name = (_txtName.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Material Name boş olamaz.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }

                Result = new MaterialItem
                {
                    Name = name,
                    Density = (float)_numDensity.Value,
                    ZRatio = (float)_numZ.Value
                };
            };
        }
    }
}
