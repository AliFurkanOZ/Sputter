namespace SputterControlApp
{
    partial class ManualReadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tlpManual = new TableLayoutPanel();
            numManualCount = new NumericUpDown();
            cmbManualWordOrder = new ComboBox();
            lblManualWordOrder = new Label();
            cmbManualEndian = new ComboBox();
            cmbManualType = new ComboBox();
            lblManualAddr = new Label();
            numManualAddr = new NumericUpDown();
            lblManualType = new Label();
            lblManualCount = new Label();
            lblManualEndian = new Label();
            panel1 = new Panel();
            btnManualRead = new Button();
            pnlTop = new Panel();
            grpManualRead = new GroupBox();
            txtManualResult = new TextBox();
            pnlBottom = new Panel();
            lblManualError = new Label();
            tlpManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numManualCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numManualAddr).BeginInit();
            pnlTop.SuspendLayout();
            grpManualRead.SuspendLayout();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // tlpManual
            // 
            tlpManual.BackColor = Color.LightGray;
            tlpManual.ColumnCount = 4;
            tlpManual.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tlpManual.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpManual.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tlpManual.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpManual.Controls.Add(numManualCount, 1, 2);
            tlpManual.Controls.Add(cmbManualWordOrder, 3, 1);
            tlpManual.Controls.Add(lblManualWordOrder, 2, 1);
            tlpManual.Controls.Add(cmbManualEndian, 3, 0);
            tlpManual.Controls.Add(cmbManualType, 1, 1);
            tlpManual.Controls.Add(lblManualAddr, 0, 0);
            tlpManual.Controls.Add(numManualAddr, 1, 0);
            tlpManual.Controls.Add(lblManualType, 0, 1);
            tlpManual.Controls.Add(lblManualCount, 0, 2);
            tlpManual.Controls.Add(lblManualEndian, 2, 0);
            tlpManual.Controls.Add(panel1, 2, 2);
            tlpManual.Controls.Add(btnManualRead, 3, 2);
            tlpManual.Dock = DockStyle.Fill;
            tlpManual.Location = new Point(10, 26);
            tlpManual.Name = "tlpManual";
            tlpManual.RowCount = 3;
            tlpManual.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpManual.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpManual.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpManual.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpManual.Size = new Size(464, 94);
            tlpManual.TabIndex = 1;
            // 
            // numManualCount
            // 
            numManualCount.Dock = DockStyle.Fill;
            numManualCount.Location = new Point(93, 63);
            numManualCount.Maximum = new decimal(new int[] { 125, 0, 0, 0 });
            numManualCount.Name = "numManualCount";
            numManualCount.Size = new Size(126, 23);
            numManualCount.TabIndex = 21;
            // 
            // cmbManualWordOrder
            // 
            cmbManualWordOrder.AutoCompleteCustomSource.AddRange(new string[] { "Word Order: High-Low (ABCD)", "Word Order: Low-High (CDAB)" });
            cmbManualWordOrder.Dock = DockStyle.Fill;
            cmbManualWordOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualWordOrder.FormattingEnabled = true;
            cmbManualWordOrder.Items.AddRange(new object[] { "High-Low (ABCD)", "Low-High (CDAB)" });
            cmbManualWordOrder.Location = new Point(335, 33);
            cmbManualWordOrder.Name = "cmbManualWordOrder";
            cmbManualWordOrder.Size = new Size(126, 23);
            cmbManualWordOrder.TabIndex = 20;
            // 
            // lblManualWordOrder
            // 
            lblManualWordOrder.AutoSize = true;
            lblManualWordOrder.Dock = DockStyle.Fill;
            lblManualWordOrder.Font = new Font("Segoe UI", 9F);
            lblManualWordOrder.ImageAlign = ContentAlignment.TopLeft;
            lblManualWordOrder.Location = new Point(225, 30);
            lblManualWordOrder.Name = "lblManualWordOrder";
            lblManualWordOrder.Size = new Size(104, 30);
            lblManualWordOrder.TabIndex = 19;
            lblManualWordOrder.Text = "Word order";
            lblManualWordOrder.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbManualEndian
            // 
            cmbManualEndian.Dock = DockStyle.Fill;
            cmbManualEndian.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualEndian.FormattingEnabled = true;
            cmbManualEndian.Items.AddRange(new object[] { "Big Endian", "Little Endian" });
            cmbManualEndian.Location = new Point(335, 3);
            cmbManualEndian.Name = "cmbManualEndian";
            cmbManualEndian.Size = new Size(126, 23);
            cmbManualEndian.TabIndex = 4;
            // 
            // cmbManualType
            // 
            cmbManualType.Dock = DockStyle.Fill;
            cmbManualType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbManualType.FormattingEnabled = true;
            cmbManualType.Items.AddRange(new object[] { "Raw (16-bit regs)", "INT16", "UINT16", "INT32", "UINT32", "FLOAT32" });
            cmbManualType.Location = new Point(93, 33);
            cmbManualType.Name = "cmbManualType";
            cmbManualType.Size = new Size(126, 23);
            cmbManualType.TabIndex = 2;
            // 
            // lblManualAddr
            // 
            lblManualAddr.AutoSize = true;
            lblManualAddr.Dock = DockStyle.Fill;
            lblManualAddr.Location = new Point(3, 0);
            lblManualAddr.Name = "lblManualAddr";
            lblManualAddr.Size = new Size(84, 30);
            lblManualAddr.TabIndex = 0;
            lblManualAddr.Text = "Address";
            lblManualAddr.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numManualAddr
            // 
            numManualAddr.Dock = DockStyle.Fill;
            numManualAddr.Location = new Point(93, 3);
            numManualAddr.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numManualAddr.Name = "numManualAddr";
            numManualAddr.Size = new Size(126, 23);
            numManualAddr.TabIndex = 2;
            // 
            // lblManualType
            // 
            lblManualType.AutoSize = true;
            lblManualType.Dock = DockStyle.Fill;
            lblManualType.Location = new Point(3, 30);
            lblManualType.Name = "lblManualType";
            lblManualType.Size = new Size(84, 30);
            lblManualType.TabIndex = 0;
            lblManualType.Text = "Type";
            lblManualType.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblManualCount
            // 
            lblManualCount.AutoSize = true;
            lblManualCount.Dock = DockStyle.Fill;
            lblManualCount.Location = new Point(3, 60);
            lblManualCount.Name = "lblManualCount";
            lblManualCount.Size = new Size(84, 40);
            lblManualCount.TabIndex = 0;
            lblManualCount.Text = "Quantity";
            lblManualCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblManualEndian
            // 
            lblManualEndian.AutoSize = true;
            lblManualEndian.Dock = DockStyle.Fill;
            lblManualEndian.Location = new Point(225, 0);
            lblManualEndian.Name = "lblManualEndian";
            lblManualEndian.Size = new Size(104, 30);
            lblManualEndian.TabIndex = 3;
            lblManualEndian.Text = "Endian";
            lblManualEndian.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.BackColor = Color.WhiteSmoke;
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(225, 63);
            panel1.Name = "panel1";
            panel1.Size = new Size(104, 34);
            panel1.TabIndex = 22;
            // 
            // btnManualRead
            // 
            btnManualRead.Dock = DockStyle.Fill;
            btnManualRead.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnManualRead.Location = new Point(335, 63);
            btnManualRead.Name = "btnManualRead";
            btnManualRead.Size = new Size(126, 34);
            btnManualRead.TabIndex = 23;
            btnManualRead.Text = "Read";
            btnManualRead.UseVisualStyleBackColor = true;
            btnManualRead.Click += btnManualRead_Click;
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.WhiteSmoke;
            pnlTop.Controls.Add(grpManualRead);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(10);
            pnlTop.Size = new Size(504, 150);
            pnlTop.TabIndex = 2;
            // 
            // grpManualRead
            // 
            grpManualRead.Controls.Add(tlpManual);
            grpManualRead.Dock = DockStyle.Fill;
            grpManualRead.Location = new Point(10, 10);
            grpManualRead.Name = "grpManualRead";
            grpManualRead.Padding = new Padding(10);
            grpManualRead.Size = new Size(484, 130);
            grpManualRead.TabIndex = 0;
            grpManualRead.TabStop = false;
            grpManualRead.Text = "Manual Read";
            // 
            // txtManualResult
            // 
            txtManualResult.BackColor = SystemColors.ControlLightLight;
            txtManualResult.Dock = DockStyle.Fill;
            txtManualResult.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            txtManualResult.Location = new Point(0, 150);
            txtManualResult.Multiline = true;
            txtManualResult.Name = "txtManualResult";
            txtManualResult.ReadOnly = true;
            txtManualResult.ScrollBars = ScrollBars.Vertical;
            txtManualResult.Size = new Size(504, 231);
            txtManualResult.TabIndex = 3;
            txtManualResult.WordWrap = false;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(lblManualError);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 353);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(10, 0, 10, 0);
            pnlBottom.Size = new Size(504, 28);
            pnlBottom.TabIndex = 4;
            // 
            // lblManualError
            // 
            lblManualError.AutoEllipsis = true;
            lblManualError.AutoSize = true;
            lblManualError.Dock = DockStyle.Fill;
            lblManualError.ForeColor = Color.Red;
            lblManualError.Location = new Point(10, 0);
            lblManualError.Name = "lblManualError";
            lblManualError.Size = new Size(0, 15);
            lblManualError.TabIndex = 0;
            lblManualError.TextAlign = ContentAlignment.MiddleLeft;
            lblManualError.Visible = false;
            // 
            // ManualReadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 381);
            Controls.Add(pnlBottom);
            Controls.Add(txtManualResult);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "ManualReadForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Modbus Manual Read";
            tlpManual.ResumeLayout(false);
            tlpManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numManualCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numManualAddr).EndInit();
            pnlTop.ResumeLayout(false);
            grpManualRead.ResumeLayout(false);
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TableLayoutPanel tlpManual;
        private NumericUpDown numManualAddr;
        private Label lblManualAddr;
        private Label lblManualType;
        private ComboBox cmbManualType;
        private Label lblManualCount;
        private Panel pnlTop;
        private GroupBox grpManualRead;
        private ComboBox cmbManualEndian;
        private Label lblManualEndian;
        private Label lblManualWordOrder;
        private NumericUpDown numManualCount;
        private ComboBox cmbManualWordOrder;
        private Panel panel1;
        private Button btnManualRead;
        private TextBox txtManualResult;
        private Panel pnlBottom;
        private Label lblManualError;
    }
}