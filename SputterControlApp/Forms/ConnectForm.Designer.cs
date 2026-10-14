namespace SputterControlApp
{
    partial class ConnectForm
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
            grpConnection = new GroupBox();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            txtRegistrationNo = new TextBox();
            label1 = new Label();
            txtPort = new TextBox();
            lblPort = new Label();
            txtIp = new TextBox();
            lblIp = new Label();
            btnExit = new Button();
            btnConnect = new Button();
            lblStatus = new Label();
            grpConnection.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // grpConnection
            // 
            grpConnection.BackColor = Color.WhiteSmoke;
            grpConnection.BackgroundImageLayout = ImageLayout.None;
            grpConnection.Controls.Add(panel1);
            grpConnection.Controls.Add(tableLayoutPanel1);
            grpConnection.Dock = DockStyle.Top;
            grpConnection.Location = new Point(0, 0);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(462, 226);
            grpConnection.TabIndex = 12;
            grpConnection.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.connectForm;
            panel1.BackgroundImageLayout = ImageLayout.None;
            panel1.Location = new Point(6, 11);
            panel1.Name = "panel1";
            panel1.Size = new Size(452, 65);
            panel1.TabIndex = 15;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = SystemColors.HighlightText;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(txtRegistrationNo, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtPort, 1, 2);
            tableLayoutPanel1.Controls.Add(lblPort, 0, 2);
            tableLayoutPanel1.Controls.Add(txtIp, 1, 1);
            tableLayoutPanel1.Controls.Add(lblIp, 0, 1);
            tableLayoutPanel1.Controls.Add(btnExit, 0, 3);
            tableLayoutPanel1.Controls.Add(btnConnect, 1, 3);
            tableLayoutPanel1.Location = new Point(6, 79);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.Size = new Size(452, 134);
            tableLayoutPanel1.TabIndex = 14;
            // 
            // txtRegistrationNo
            // 
            txtRegistrationNo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtRegistrationNo.Location = new Point(229, 3);
            txtRegistrationNo.Name = "txtRegistrationNo";
            txtRegistrationNo.Size = new Size(220, 23);
            txtRegistrationNo.TabIndex = 16;
            txtRegistrationNo.Text = "1234";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(3, 2);
            label1.Name = "label1";
            label1.Size = new Size(220, 21);
            label1.TabIndex = 15;
            label1.Text = "Registration Number:";
            // 
            // txtPort
            // 
            txtPort.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPort.Location = new Point(229, 55);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(220, 23);
            txtPort.TabIndex = 3;
            txtPort.Text = "502";
            // 
            // lblPort
            // 
            lblPort.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 12F);
            lblPort.Location = new Point(3, 54);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(220, 21);
            lblPort.TabIndex = 1;
            lblPort.Text = "Port:";
            // 
            // txtIp
            // 
            txtIp.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIp.Location = new Point(229, 29);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(220, 23);
            txtIp.TabIndex = 9;
            txtIp.Text = "192.168.1.5";
            // 
            // lblIp
            // 
            lblIp.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblIp.AutoSize = true;
            lblIp.Font = new Font("Segoe UI", 12F);
            lblIp.Location = new Point(3, 28);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(220, 21);
            lblIp.TabIndex = 0;
            lblIp.Text = "IP Address:";
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.Gainsboro;
            btnExit.DialogResult = DialogResult.Cancel;
            btnExit.Dock = DockStyle.Fill;
            btnExit.Font = new Font("Segoe UI", 12F);
            btnExit.Location = new Point(3, 81);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(220, 50);
            btnExit.TabIndex = 14;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.SeaGreen;
            btnConnect.Dock = DockStyle.Fill;
            btnConnect.Font = new Font("Segoe UI", 12F);
            btnConnect.ForeColor = SystemColors.ControlLightLight;
            btnConnect.Location = new Point(229, 81);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(220, 50);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.ForeColor = SystemColors.Highlight;
            lblStatus.Location = new Point(0, 222);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(462, 24);
            lblStatus.TabIndex = 13;
            lblStatus.Text = "Disconnected";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ConnectForm
            // 
            AcceptButton = btnConnect;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            BackgroundImageLayout = ImageLayout.Center;
            CancelButton = btnExit;
            ClientSize = new Size(462, 246);
            Controls.Add(lblStatus);
            Controls.Add(grpConnection);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "ConnectForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sputter HMI";
            grpConnection.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpConnection;
        private TextBox txtIp;
        private TextBox txtPort;
        private Button btnConnect;
        private Label lblIp;
        private Label lblPort;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnExit;
        private Label lblStatus;
        private Label label1;
        private TextBox txtRegistrationNo;
        private Panel panel1;
    }
}