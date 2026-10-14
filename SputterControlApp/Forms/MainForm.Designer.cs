namespace SputterControlApp
{
    partial class MainForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuMain = new MenuStrip();
            connectionToolStripMenuItem = new ToolStripMenuItem();
            miDisconnect = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            miManualRead = new ToolStripMenuItem();
            pnlTopBar = new Panel();
            lblConnState = new Label();
            btnRefresh = new Button();
            btnDisconnect = new Button();
            lblPollState = new Label();
            statusStrip1 = new StatusStrip();
            tsStatus = new ToolStripStatusLabel();
            tsStatusQcm = new ToolStripStatusLabel();
            tabControl1 = new TabControl();
            tpProcess = new TabPage();
            gbQcmSensorInfo2 = new GroupBox();
            label110 = new Label();
            label111 = new Label();
            label112 = new Label();
            label113 = new Label();
            label114 = new Label();
            label115 = new Label();
            label107 = new Label();
            label108 = new Label();
            label118 = new Label();
            label119 = new Label();
            label120 = new Label();
            label121 = new Label();
            label81 = new Label();
            label85 = new Label();
            label97 = new Label();
            label98 = new Label();
            label99 = new Label();
            label100 = new Label();
            tableLayoutPanel5 = new TableLayoutPanel();
            lblValue2_DcRate = new Label();
            lblValue2_DcLife = new Label();
            lblValue2_DcThickness = new Label();
            label95 = new Label();
            lblValue2_Rf2Life = new Label();
            label93 = new Label();
            lblValue2_Rf1Life = new Label();
            lblValue2_Rf1Thickness = new Label();
            label106 = new Label();
            lblValue2_Rf1Rate = new Label();
            lblValue2_Rf2Rate = new Label();
            lblValue2_Rf2Thickness = new Label();
            grpRf2 = new GroupBox();
            groupBox3 = new GroupBox();
            tableLayoutPanel8 = new TableLayoutPanel();
            tableLayoutPanel12 = new TableLayoutPanel();
            lblID2_Title_O2 = new Label();
            lblID2_ActualFlowValue = new Label();
            numID2_SetPointWrite = new NumericUpDown();
            btnID2_ValveToggle = new Button();
            label71 = new Label();
            label72 = new Label();
            label73 = new Label();
            tableLayoutPanel13 = new TableLayoutPanel();
            lblID1_Title_Ar = new Label();
            lblID1_ActualFlowValue = new Label();
            numID1_SetPointWrite = new NumericUpDown();
            btnID1_ValveToggle = new Button();
            panel11 = new Panel();
            btnSelectRf2Target = new Button();
            label38 = new Label();
            txtRf2_Target = new TextBox();
            txtRf2_TargetLife = new TextBox();
            label39 = new Label();
            label40 = new Label();
            panel12 = new Panel();
            lblRf2_RefP = new Label();
            lblRf2_FwP = new Label();
            label43 = new Label();
            pbRf2_RefP = new ProgressBar();
            label44 = new Label();
            pbRf2_FwP = new ProgressBar();
            panel13 = new Panel();
            btnControlRf2 = new Button();
            panel14 = new Panel();
            numRf2_OffRamp = new NumericUpDown();
            numRf2_Step2_Dwell = new NumericUpDown();
            numRf2_Step2_Ramp = new NumericUpDown();
            label56 = new Label();
            numRf2_Step2_SetW = new NumericUpDown();
            label45 = new Label();
            label46 = new Label();
            label47 = new Label();
            label48 = new Label();
            label49 = new Label();
            label50 = new Label();
            label51 = new Label();
            label52 = new Label();
            label53 = new Label();
            txtRf2_Step1_SetW = new TextBox();
            txtRf2_Step1_Ramp = new TextBox();
            txtRf2_Step1_Dwell = new TextBox();
            panel15 = new Panel();
            txtRf2_Step2_rTime = new TextBox();
            txtRf2_Step1_rTime = new TextBox();
            label54 = new Label();
            label55 = new Label();
            grpRf1 = new GroupBox();
            groupBox2 = new GroupBox();
            tableLayoutPanel7 = new TableLayoutPanel();
            label42 = new Label();
            label60 = new Label();
            label61 = new Label();
            tableLayoutPanel9 = new TableLayoutPanel();
            lblID3_Title_Ar = new Label();
            lblID3_ActualFlowValue = new Label();
            numID3_SetPointWrite = new NumericUpDown();
            btnID3_ValveToggle = new Button();
            panel6 = new Panel();
            btnSelectRf1Target = new Button();
            label20 = new Label();
            txtRf1_Target = new TextBox();
            txtRf1_TargetLife = new TextBox();
            label21 = new Label();
            label22 = new Label();
            panel7 = new Panel();
            lblRf1_RefP = new Label();
            lblRf1_FwP = new Label();
            label25 = new Label();
            pbRf1_RefP = new ProgressBar();
            lbl = new Label();
            pbRf1_FwP = new ProgressBar();
            panel8 = new Panel();
            btnControlRf1 = new Button();
            panel9 = new Panel();
            label14 = new Label();
            numRf1_Step2_SetW = new NumericUpDown();
            numRf1_OffRamp = new NumericUpDown();
            numRf1_Step2_Dwell = new NumericUpDown();
            numRf1_Step2_Ramp = new NumericUpDown();
            lblRf1_OffRamp = new Label();
            label28 = new Label();
            label29 = new Label();
            label30 = new Label();
            label31 = new Label();
            lblRf2_Dwell = new Label();
            lblRf2_Ramp = new Label();
            label34 = new Label();
            lblRf2_Set = new Label();
            txtRf1_Step1_SetW = new TextBox();
            txtRf1_Step1_Ramp = new TextBox();
            txtRf1_Step1_Dwell = new TextBox();
            panel10 = new Panel();
            txtRf1_Step2_rTime = new TextBox();
            txtRf1_Step1_rTime = new TextBox();
            label36 = new Label();
            label37 = new Label();
            grpDC = new GroupBox();
            groupBox1 = new GroupBox();
            tableLayoutPanel6 = new TableLayoutPanel();
            tableLayoutPanel10 = new TableLayoutPanel();
            lblID5_Title_O2 = new Label();
            lblID5_ActualFlowValue = new Label();
            numID5_SetPointWrite = new NumericUpDown();
            btnID5_ValveToggle = new Button();
            label65 = new Label();
            label66 = new Label();
            label67 = new Label();
            tableLayoutPanel11 = new TableLayoutPanel();
            lblID4_Title_Ar = new Label();
            lblID4_ActualFlowValue = new Label();
            numID4_SetPointWrite = new NumericUpDown();
            btnID4_ValveToggle = new Button();
            panel5 = new Panel();
            btnSelectDcTarget = new Button();
            label17 = new Label();
            txtDC_Target = new TextBox();
            txtDC_TargetLife = new TextBox();
            label16 = new Label();
            label15 = new Label();
            panel3 = new Panel();
            txtDC_ReadPower = new TextBox();
            label59 = new Label();
            label58 = new Label();
            label57 = new Label();
            txtDC_ReadVoltage = new TextBox();
            txtDC_ReadCurrent = new TextBox();
            panel2 = new Panel();
            btnControlDC = new Button();
            panel1 = new Panel();
            label27 = new Label();
            label26 = new Label();
            txtDC_SetCurrent = new TextBox();
            numDC_OffRamp = new NumericUpDown();
            numDc_Step2_Dwell = new NumericUpDown();
            numDc_Step2_Ramp = new NumericUpDown();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            txtDC_SetVoltage = new TextBox();
            numDc_Step2_SetW = new NumericUpDown();
            lblDc_OffRamp = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            lblRf1_Dwell = new Label();
            lblRf1_Ramp = new Label();
            label5 = new Label();
            lblRf1_Set = new Label();
            txtDc_Step1_SetW = new TextBox();
            txtDc_Step1_Ramp = new TextBox();
            txtDc_Step1_Dwell = new TextBox();
            panel4 = new Panel();
            txtDC_Step2_rTime = new TextBox();
            txtDC_Step1_rTime = new TextBox();
            label19 = new Label();
            label18 = new Label();
            grpStartControls = new GroupBox();
            tlpAnalogFeedback = new TableLayoutPanel();
            label80 = new Label();
            label79 = new Label();
            label35 = new Label();
            lblTurboFrequency = new Label();
            label32 = new Label();
            lblRotationMotor = new Label();
            label10 = new Label();
            numGateValvePosition = new NumericUpDown();
            label2 = new Label();
            btnRotationMotor = new Button();
            numMotorRpm = new NumericUpDown();
            tableLayoutPanel3 = new TableLayoutPanel();
            label82 = new Label();
            numRezistanceControl = new NumericUpDown();
            rezistanceReadValue = new TextBox();
            btnSetRezistanceVal = new Button();
            tlpStartControls = new TableLayoutPanel();
            btnRezistans = new Button();
            lblRezistans = new Label();
            btnMatchBoxPower = new Button();
            lblGUC4 = new Label();
            btnDCShutter = new Button();
            lblLeftFrontShutter = new Label();
            btnDCPower = new Button();
            lblGUC3 = new Label();
            btnRF2Power = new Button();
            lblGUC2 = new Label();
            btnRF2Shutter = new Button();
            lblRightRearShutter = new Label();
            btnRF1Shutter = new Button();
            lblRightFrontShutter = new Label();
            btnRF1Power = new Button();
            lblGUC1 = new Label();
            btnChamber = new Button();
            lblChamberValve = new Label();
            btnTurboVacuum = new Button();
            lblTurbo_EVP_Vacuum = new Label();
            btnChiller = new Button();
            lblChiller = new Label();
            btnScrollVacuum = new Button();
            lblScrewVacuum = new Label();
            panel16 = new Panel();
            label84 = new Label();
            lblTorDegerAux = new Label();
            label83 = new Label();
            label1 = new Label();
            cmbTorUnit = new ComboBox();
            lblTorDeger = new Label();
            lblTor = new Label();
            panelTorMini = new Panel();
            tpGraph = new TabPage();
            btnExport = new Button();
            label77 = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            lblThicknessFrozen = new Label();
            btnThicknessFreeze = new Button();
            btnThicknessAll = new Button();
            btnThicknessLive = new Button();
            lblRateFrozen = new Label();
            btnRateFreeze = new Button();
            btnRateAll = new Button();
            btnRateLive = new Button();
            chkTorLog = new CheckBox();
            lblTorFrozen = new Label();
            btnTorFreeze = new Button();
            btnTorAll = new Button();
            btnTorLive = new Button();
            label76 = new Label();
            panel19 = new Panel();
            label68 = new Label();
            label64 = new Label();
            label63 = new Label();
            panelRate = new Panel();
            panelThickness = new Panel();
            panelTorque = new Panel();
            tpQCM = new TabPage();
            label78 = new Label();
            tlpCrystalLife1 = new TableLayoutPanel();
            lblValue_DcCrystalLife = new Label();
            lblValue_Rf2CrystalLife = new Label();
            label87 = new Label();
            label88 = new Label();
            label92 = new Label();
            lblValue_Rf1CrystalLife = new Label();
            gbQcmSensorInfo = new GroupBox();
            label75 = new Label();
            label70 = new Label();
            label74 = new Label();
            label69 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            lblValue_DcThickness = new Label();
            lblValue_DcRate = new Label();
            lblValue_Rf2Thickness = new Label();
            lblValue_Rf2Rate = new Label();
            lblValue_Rf1Thickness = new Label();
            label89 = new Label();
            label90 = new Label();
            label91 = new Label();
            lblValue_Rf1Rate = new Label();
            groupBox4 = new GroupBox();
            btnZeroThickness = new Button();
            btnNextLayer = new Button();
            btnStartLayer = new Button();
            grpActiveLayerDetail = new GroupBox();
            cmbLayerDetailTitle = new ComboBox();
            lblLd = new Label();
            tlpLayerDetail = new TableLayoutPanel();
            lblLdStartMode = new Label();
            lblLdSource = new Label();
            lblLdFinalThk = new Label();
            lblLdTooling = new Label();
            lblLdZRatio = new Label();
            lblLdDensity = new Label();
            label3 = new Label();
            label4 = new Label();
            label23 = new Label();
            label24 = new Label();
            label33 = new Label();
            label41 = new Label();
            label62 = new Label();
            lblLdMaterial = new Label();
            grpLayerList = new GroupBox();
            dgvLayers = new DataGridView();
            colLayerInfo = new DataGridViewTextBoxColumn();
            colProcessNumber = new DataGridViewTextBoxColumn();
            colProcessName = new DataGridViewTextBoxColumn();
            colFinalThickness = new DataGridViewTextBoxColumn();
            colSource = new DataGridViewTextBoxColumn();
            colRF1Sensor = new DataGridViewTextBoxColumn();
            colRF2Sensor = new DataGridViewTextBoxColumn();
            colDCSensor = new DataGridViewTextBoxColumn();
            colStartMode = new DataGridViewTextBoxColumn();
            pnlLayerButtons = new Panel();
            btnDeleteLayer = new Button();
            btnEditLayer = new Button();
            btnInsertCoDepLayer = new Button();
            btnInsertNormalLayer = new Button();
            grpProcessList = new GroupBox();
            dgvProcesses = new DataGridView();
            colProcessNum = new DataGridViewTextBoxColumn();
            colProcName = new DataGridViewTextBoxColumn();
            colProcMaterialNumber = new DataGridViewTextBoxColumn();
            colProcTooling = new DataGridViewTextBoxColumn();
            pnlProcessButtons = new Panel();
            btnProcDelete = new Button();
            btnProcEdit = new Button();
            btnProcAdd = new Button();
            grpMaterialList = new GroupBox();
            dgvMaterials = new DataGridView();
            colMatNumber = new DataGridViewTextBoxColumn();
            colMatName = new DataGridViewTextBoxColumn();
            colMatDensity = new DataGridViewTextBoxColumn();
            colMatZ = new DataGridViewTextBoxColumn();
            pnlMaterialButtons = new Panel();
            btnMatDelete = new Button();
            btnMatEdit = new Button();
            btnMatAdd = new Button();
            tpLogPage = new TabPage();
            panelEventsBottom = new Panel();
            lblEvtStats = new Label();
            panelEventsTop = new Panel();
            button4 = new Button();
            dtpEvtTo = new DateTimePicker();
            dtpEvtFrom = new DateTimePicker();
            cmbEvtSource = new ComboBox();
            cmbEvtLevel = new ComboBox();
            cmbEvtTimeRange = new ComboBox();
            chkEvtAutoScroll = new CheckBox();
            txtEvtSearch = new TextBox();
            btnEvtApplyRange = new Button();
            btnEvtExport = new Button();
            dgvEvents = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            colCode = new DataGridViewTextBoxColumn();
            colMessage = new DataGridViewTextBoxColumn();
            colSourceLog = new DataGridViewTextBoxColumn();
            colRegistrationNo = new DataGridViewTextBoxColumn();
            tabPage1 = new TabPage();
            lstLog = new ListBox();
            menuMain.SuspendLayout();
            pnlTopBar.SuspendLayout();
            statusStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tpProcess.SuspendLayout();
            gbQcmSensorInfo2.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            grpRf2.SuspendLayout();
            groupBox3.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numID2_SetPointWrite).BeginInit();
            tableLayoutPanel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numID1_SetPointWrite).BeginInit();
            panel11.SuspendLayout();
            panel12.SuspendLayout();
            panel13.SuspendLayout();
            panel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRf2_OffRamp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_Dwell).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_Ramp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_SetW).BeginInit();
            panel15.SuspendLayout();
            grpRf1.SuspendLayout();
            groupBox2.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numID3_SetPointWrite).BeginInit();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            panel8.SuspendLayout();
            panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_SetW).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_OffRamp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_Dwell).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_Ramp).BeginInit();
            panel10.SuspendLayout();
            grpDC.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numID5_SetPointWrite).BeginInit();
            tableLayoutPanel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numID4_SetPointWrite).BeginInit();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDC_OffRamp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_Dwell).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_Ramp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_SetW).BeginInit();
            panel4.SuspendLayout();
            grpStartControls.SuspendLayout();
            tlpAnalogFeedback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numGateValvePosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMotorRpm).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRezistanceControl).BeginInit();
            tlpStartControls.SuspendLayout();
            panel16.SuspendLayout();
            tpGraph.SuspendLayout();
            tpQCM.SuspendLayout();
            tlpCrystalLife1.SuspendLayout();
            gbQcmSensorInfo.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            groupBox4.SuspendLayout();
            grpActiveLayerDetail.SuspendLayout();
            tlpLayerDetail.SuspendLayout();
            grpLayerList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLayers).BeginInit();
            pnlLayerButtons.SuspendLayout();
            grpProcessList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProcesses).BeginInit();
            pnlProcessButtons.SuspendLayout();
            grpMaterialList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMaterials).BeginInit();
            pnlMaterialButtons.SuspendLayout();
            tpLogPage.SuspendLayout();
            panelEventsBottom.SuspendLayout();
            panelEventsTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEvents).BeginInit();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // menuMain
            // 
            menuMain.BackColor = Color.White;
            menuMain.Dock = DockStyle.None;
            menuMain.ImageScalingSize = new Size(20, 20);
            menuMain.Items.AddRange(new ToolStripItem[] { connectionToolStripMenuItem, toolsToolStripMenuItem });
            menuMain.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(136, 24);
            menuMain.TabIndex = 0;
            menuMain.Text = "menuStrip1";
            // 
            // connectionToolStripMenuItem
            // 
            connectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { miDisconnect });
            connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            connectionToolStripMenuItem.Size = new Size(81, 20);
            connectionToolStripMenuItem.Text = "Connection";
            // 
            // miDisconnect
            // 
            miDisconnect.Name = "miDisconnect";
            miDisconnect.Size = new Size(133, 22);
            miDisconnect.Text = "Disconnect";
            miDisconnect.Click += miDisconnect_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { miManualRead });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(47, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // miManualRead
            // 
            miManualRead.Name = "miManualRead";
            miManualRead.ShortcutKeys = Keys.Control | Keys.M;
            miManualRead.Size = new Size(188, 22);
            miManualRead.Text = "Manuel Read";
            miManualRead.Click += miManualRead_Click;
            // 
            // pnlTopBar
            // 
            pnlTopBar.BackColor = Color.White;
            pnlTopBar.Controls.Add(lblConnState);
            pnlTopBar.Controls.Add(btnRefresh);
            pnlTopBar.Controls.Add(btnDisconnect);
            pnlTopBar.Controls.Add(lblPollState);
            pnlTopBar.Location = new Point(1044, 0);
            pnlTopBar.Name = "pnlTopBar";
            pnlTopBar.Size = new Size(592, 24);
            pnlTopBar.TabIndex = 1;
            // 
            // lblConnState
            // 
            lblConnState.AutoSize = true;
            lblConnState.Location = new Point(2, 5);
            lblConnState.Name = "lblConnState";
            lblConnState.Size = new Size(65, 15);
            lblConnState.TabIndex = 0;
            lblConnState.Text = "Connected";
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Right;
            btnRefresh.BackColor = SystemColors.GradientActiveCaption;
            btnRefresh.Location = new Point(225, 2);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(141, 22);
            btnRefresh.TabIndex = 18;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Anchor = AnchorStyles.Right;
            btnDisconnect.BackColor = SystemColors.GradientActiveCaption;
            btnDisconnect.Location = new Point(419, 2);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(141, 22);
            btnDisconnect.TabIndex = 1;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = false;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // lblPollState
            // 
            lblPollState.AutoSize = true;
            lblPollState.Location = new Point(100, 5);
            lblPollState.Name = "lblPollState";
            lblPollState.Size = new Size(83, 15);
            lblPollState.TabIndex = 0;
            lblPollState.Text = "Polling: Active";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsStatus, tsStatusQcm });
            statusStrip1.Location = new Point(0, 961);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1754, 24);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            tsStatus.Name = "tsStatus";
            tsStatus.Size = new Size(39, 19);
            tsStatus.Text = "Ready";
            // 
            // tsStatusQcm
            // 
            tsStatusQcm.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            tsStatusQcm.BorderStyle = Border3DStyle.Raised;
            tsStatusQcm.Name = "tsStatusQcm";
            tsStatusQcm.RightToLeft = RightToLeft.No;
            tsStatusQcm.Size = new Size(37, 19);
            tsStatusQcm.Text = "Qcm";
            // 
            // tabControl1
            // 
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.Controls.Add(tpProcess);
            tabControl1.Controls.Add(tpGraph);
            tabControl1.Controls.Add(tpQCM);
            tabControl1.Controls.Add(tpLogPage);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(0, 25);
            tabControl1.Name = "tabControl1";
            tabControl1.RightToLeft = RightToLeft.No;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1892, 933);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 19;
            // 
            // tpProcess
            // 
            tpProcess.Controls.Add(gbQcmSensorInfo2);
            tpProcess.Controls.Add(grpRf2);
            tpProcess.Controls.Add(grpRf1);
            tpProcess.Controls.Add(grpDC);
            tpProcess.Controls.Add(grpStartControls);
            tpProcess.Location = new Point(4, 27);
            tpProcess.Name = "tpProcess";
            tpProcess.Padding = new Padding(3);
            tpProcess.Size = new Size(1884, 902);
            tpProcess.TabIndex = 0;
            tpProcess.Text = "Process";
            tpProcess.UseVisualStyleBackColor = true;
            // 
            // gbQcmSensorInfo2
            // 
            gbQcmSensorInfo2.BackColor = Color.LightSteelBlue;
            gbQcmSensorInfo2.Controls.Add(label110);
            gbQcmSensorInfo2.Controls.Add(label111);
            gbQcmSensorInfo2.Controls.Add(label112);
            gbQcmSensorInfo2.Controls.Add(label113);
            gbQcmSensorInfo2.Controls.Add(label114);
            gbQcmSensorInfo2.Controls.Add(label115);
            gbQcmSensorInfo2.Controls.Add(label107);
            gbQcmSensorInfo2.Controls.Add(label108);
            gbQcmSensorInfo2.Controls.Add(label118);
            gbQcmSensorInfo2.Controls.Add(label119);
            gbQcmSensorInfo2.Controls.Add(label120);
            gbQcmSensorInfo2.Controls.Add(label121);
            gbQcmSensorInfo2.Controls.Add(label81);
            gbQcmSensorInfo2.Controls.Add(label85);
            gbQcmSensorInfo2.Controls.Add(label97);
            gbQcmSensorInfo2.Controls.Add(label98);
            gbQcmSensorInfo2.Controls.Add(label99);
            gbQcmSensorInfo2.Controls.Add(label100);
            gbQcmSensorInfo2.Controls.Add(tableLayoutPanel5);
            gbQcmSensorInfo2.Font = new Font("Segoe UI", 12F);
            gbQcmSensorInfo2.Location = new Point(26, 778);
            gbQcmSensorInfo2.Name = "gbQcmSensorInfo2";
            gbQcmSensorInfo2.Size = new Size(824, 118);
            gbQcmSensorInfo2.TabIndex = 20;
            gbQcmSensorInfo2.TabStop = false;
            gbQcmSensorInfo2.Text = "Qcm Sensor Info";
            // 
            // label110
            // 
            label110.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label110.AutoSize = true;
            label110.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label110.Location = new Point(760, 42);
            label110.Name = "label110";
            label110.Size = new Size(31, 17);
            label110.TabIndex = 15;
            label110.Text = " (%)";
            label110.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label111
            // 
            label111.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label111.AutoSize = true;
            label111.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label111.Location = new Point(760, 25);
            label111.Name = "label111";
            label111.Size = new Size(28, 17);
            label111.TabIndex = 16;
            label111.Text = "Life";
            label111.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label112
            // 
            label112.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label112.AutoSize = true;
            label112.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label112.Location = new Point(684, 42);
            label112.Name = "label112";
            label112.Size = new Size(34, 17);
            label112.TabIndex = 13;
            label112.Text = " (kÅ)";
            label112.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label113
            // 
            label113.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label113.AutoSize = true;
            label113.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label113.Location = new Point(664, 25);
            label113.Name = "label113";
            label113.Size = new Size(63, 17);
            label113.TabIndex = 14;
            label113.Text = "Thickness";
            label113.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label114
            // 
            label114.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label114.AutoSize = true;
            label114.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label114.Location = new Point(609, 42);
            label114.Name = "label114";
            label114.Size = new Size(35, 17);
            label114.TabIndex = 11;
            label114.Text = "(Å/s)";
            label114.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label115
            // 
            label115.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label115.AutoSize = true;
            label115.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label115.Location = new Point(609, 25);
            label115.Name = "label115";
            label115.Size = new Size(34, 17);
            label115.TabIndex = 12;
            label115.Text = "Rate";
            label115.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label107
            // 
            label107.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label107.AutoSize = true;
            label107.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label107.Location = new Point(491, 42);
            label107.Name = "label107";
            label107.Size = new Size(31, 17);
            label107.TabIndex = 9;
            label107.Text = " (%)";
            label107.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label108
            // 
            label108.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label108.AutoSize = true;
            label108.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label108.Location = new Point(494, 25);
            label108.Name = "label108";
            label108.Size = new Size(28, 17);
            label108.TabIndex = 10;
            label108.Text = "Life";
            label108.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label118
            // 
            label118.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label118.AutoSize = true;
            label118.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label118.Location = new Point(420, 42);
            label118.Name = "label118";
            label118.Size = new Size(34, 17);
            label118.TabIndex = 7;
            label118.Text = " (kÅ)";
            label118.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label119
            // 
            label119.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label119.AutoSize = true;
            label119.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label119.Location = new Point(402, 25);
            label119.Name = "label119";
            label119.Size = new Size(63, 17);
            label119.TabIndex = 8;
            label119.Text = "Thickness";
            label119.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label120
            // 
            label120.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label120.AutoSize = true;
            label120.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label120.Location = new Point(348, 42);
            label120.Name = "label120";
            label120.Size = new Size(35, 17);
            label120.TabIndex = 5;
            label120.Text = "(Å/s)";
            label120.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label121
            // 
            label121.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label121.AutoSize = true;
            label121.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label121.Location = new Point(349, 25);
            label121.Name = "label121";
            label121.Size = new Size(34, 17);
            label121.TabIndex = 6;
            label121.Text = "Rate";
            label121.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label81
            // 
            label81.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label81.AutoSize = true;
            label81.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label81.Location = new Point(223, 42);
            label81.Name = "label81";
            label81.Size = new Size(31, 17);
            label81.TabIndex = 3;
            label81.Text = " (%)";
            label81.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label85
            // 
            label85.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label85.AutoSize = true;
            label85.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label85.Location = new Point(224, 25);
            label85.Name = "label85";
            label85.Size = new Size(28, 17);
            label85.TabIndex = 4;
            label85.Text = "Life";
            label85.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label97
            // 
            label97.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label97.AutoSize = true;
            label97.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label97.Location = new Point(153, 42);
            label97.Name = "label97";
            label97.Size = new Size(34, 17);
            label97.TabIndex = 2;
            label97.Text = " (kÅ)";
            label97.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label98
            // 
            label98.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label98.AutoSize = true;
            label98.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label98.Location = new Point(139, 25);
            label98.Name = "label98";
            label98.Size = new Size(63, 17);
            label98.TabIndex = 2;
            label98.Text = "Thickness";
            label98.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label99
            // 
            label99.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label99.AutoSize = true;
            label99.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label99.Location = new Point(85, 42);
            label99.Name = "label99";
            label99.Size = new Size(35, 17);
            label99.TabIndex = 1;
            label99.Text = "(Å/s)";
            label99.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label100
            // 
            label100.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label100.AutoSize = true;
            label100.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label100.Location = new Point(85, 25);
            label100.Name = "label100";
            label100.Size = new Size(34, 17);
            label100.TabIndex = 1;
            label100.Text = "Rate";
            label100.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.BackColor = SystemColors.GradientInactiveCaption;
            tableLayoutPanel5.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel5.ColumnCount = 12;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableLayoutPanel5.Controls.Add(lblValue2_DcRate, 9, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_DcLife, 11, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_DcThickness, 10, 0);
            tableLayoutPanel5.Controls.Add(label95, 8, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf2Life, 7, 0);
            tableLayoutPanel5.Controls.Add(label93, 4, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf1Life, 3, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf1Thickness, 2, 0);
            tableLayoutPanel5.Controls.Add(label106, 0, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf1Rate, 1, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf2Rate, 5, 0);
            tableLayoutPanel5.Controls.Add(lblValue2_Rf2Thickness, 6, 0);
            tableLayoutPanel5.Location = new Point(12, 62);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel5.Size = new Size(800, 45);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // lblValue2_DcRate
            // 
            lblValue2_DcRate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_DcRate.BackColor = Color.White;
            lblValue2_DcRate.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_DcRate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_DcRate.Location = new Point(583, 7);
            lblValue2_DcRate.Name = "lblValue2_DcRate";
            lblValue2_DcRate.Size = new Size(64, 30);
            lblValue2_DcRate.TabIndex = 19;
            lblValue2_DcRate.Text = "-";
            lblValue2_DcRate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue2_DcLife
            // 
            lblValue2_DcLife.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_DcLife.BackColor = Color.White;
            lblValue2_DcLife.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_DcLife.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_DcLife.Location = new Point(725, 7);
            lblValue2_DcLife.Name = "lblValue2_DcLife";
            lblValue2_DcLife.Size = new Size(71, 30);
            lblValue2_DcLife.TabIndex = 18;
            lblValue2_DcLife.Text = "-";
            lblValue2_DcLife.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue2_DcThickness
            // 
            lblValue2_DcThickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_DcThickness.BackColor = Color.White;
            lblValue2_DcThickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_DcThickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_DcThickness.Location = new Point(654, 7);
            lblValue2_DcThickness.Name = "lblValue2_DcThickness";
            lblValue2_DcThickness.Size = new Size(64, 30);
            lblValue2_DcThickness.TabIndex = 17;
            lblValue2_DcThickness.Text = "-";
            lblValue2_DcThickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label95
            // 
            label95.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label95.AutoSize = true;
            label95.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label95.Location = new Point(532, 14);
            label95.Name = "label95";
            label95.Size = new Size(44, 17);
            label95.TabIndex = 15;
            label95.Text = "DC";
            label95.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblValue2_Rf2Life
            // 
            lblValue2_Rf2Life.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf2Life.BackColor = Color.White;
            lblValue2_Rf2Life.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf2Life.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf2Life.Location = new Point(461, 7);
            lblValue2_Rf2Life.Name = "lblValue2_Rf2Life";
            lblValue2_Rf2Life.Size = new Size(64, 30);
            lblValue2_Rf2Life.TabIndex = 14;
            lblValue2_Rf2Life.Text = "-";
            lblValue2_Rf2Life.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label93
            // 
            label93.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label93.AutoSize = true;
            label93.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label93.Location = new Point(268, 14);
            label93.Name = "label93";
            label93.Size = new Size(44, 17);
            label93.TabIndex = 13;
            label93.Text = "RF2";
            label93.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblValue2_Rf1Life
            // 
            lblValue2_Rf1Life.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf1Life.BackColor = Color.White;
            lblValue2_Rf1Life.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf1Life.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf1Life.Location = new Point(197, 7);
            lblValue2_Rf1Life.Name = "lblValue2_Rf1Life";
            lblValue2_Rf1Life.Size = new Size(64, 30);
            lblValue2_Rf1Life.TabIndex = 12;
            lblValue2_Rf1Life.Text = "-";
            lblValue2_Rf1Life.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue2_Rf1Thickness
            // 
            lblValue2_Rf1Thickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf1Thickness.BackColor = Color.White;
            lblValue2_Rf1Thickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf1Thickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf1Thickness.Location = new Point(126, 7);
            lblValue2_Rf1Thickness.Name = "lblValue2_Rf1Thickness";
            lblValue2_Rf1Thickness.Size = new Size(64, 30);
            lblValue2_Rf1Thickness.TabIndex = 1;
            lblValue2_Rf1Thickness.Text = "-";
            lblValue2_Rf1Thickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label106
            // 
            label106.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label106.AutoSize = true;
            label106.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label106.Location = new Point(4, 14);
            label106.Name = "label106";
            label106.Size = new Size(44, 17);
            label106.TabIndex = 0;
            label106.Text = "RF1";
            label106.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblValue2_Rf1Rate
            // 
            lblValue2_Rf1Rate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf1Rate.BackColor = Color.White;
            lblValue2_Rf1Rate.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf1Rate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf1Rate.Location = new Point(55, 7);
            lblValue2_Rf1Rate.Name = "lblValue2_Rf1Rate";
            lblValue2_Rf1Rate.Size = new Size(64, 30);
            lblValue2_Rf1Rate.TabIndex = 0;
            lblValue2_Rf1Rate.Text = "-";
            lblValue2_Rf1Rate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue2_Rf2Rate
            // 
            lblValue2_Rf2Rate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf2Rate.BackColor = Color.White;
            lblValue2_Rf2Rate.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf2Rate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf2Rate.Location = new Point(319, 7);
            lblValue2_Rf2Rate.Name = "lblValue2_Rf2Rate";
            lblValue2_Rf2Rate.Size = new Size(64, 30);
            lblValue2_Rf2Rate.TabIndex = 10;
            lblValue2_Rf2Rate.Text = "-";
            lblValue2_Rf2Rate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue2_Rf2Thickness
            // 
            lblValue2_Rf2Thickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue2_Rf2Thickness.BackColor = Color.White;
            lblValue2_Rf2Thickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue2_Rf2Thickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue2_Rf2Thickness.Location = new Point(390, 7);
            lblValue2_Rf2Thickness.Name = "lblValue2_Rf2Thickness";
            lblValue2_Rf2Thickness.Size = new Size(64, 30);
            lblValue2_Rf2Thickness.TabIndex = 11;
            lblValue2_Rf2Thickness.Text = "-";
            lblValue2_Rf2Thickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpRf2
            // 
            grpRf2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpRf2.BackColor = Color.LightSteelBlue;
            grpRf2.Controls.Add(groupBox3);
            grpRf2.Controls.Add(panel11);
            grpRf2.Controls.Add(panel12);
            grpRf2.Controls.Add(panel13);
            grpRf2.Controls.Add(panel14);
            grpRf2.Controls.Add(panel15);
            grpRf2.Location = new Point(1304, 6);
            grpRf2.Name = "grpRf2";
            grpRf2.Size = new Size(408, 756);
            grpRf2.TabIndex = 17;
            grpRf2.TabStop = false;
            grpRf2.Text = "RF2 Control";
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.GradientInactiveCaption;
            groupBox3.Controls.Add(tableLayoutPanel8);
            groupBox3.Location = new Point(12, 611);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(386, 123);
            groupBox3.TabIndex = 20;
            groupBox3.TabStop = false;
            groupBox3.Text = "MFC Control RF2";
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel8.ColumnCount = 3;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel8.Controls.Add(tableLayoutPanel12, 0, 2);
            tableLayoutPanel8.Controls.Add(numID2_SetPointWrite, 1, 2);
            tableLayoutPanel8.Controls.Add(btnID2_ValveToggle, 2, 2);
            tableLayoutPanel8.Controls.Add(label71, 2, 0);
            tableLayoutPanel8.Controls.Add(label72, 1, 0);
            tableLayoutPanel8.Controls.Add(label73, 0, 0);
            tableLayoutPanel8.Controls.Add(tableLayoutPanel13, 0, 1);
            tableLayoutPanel8.Controls.Add(numID1_SetPointWrite, 1, 1);
            tableLayoutPanel8.Controls.Add(btnID1_ValveToggle, 2, 1);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(3, 19);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 3;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel8.Size = new Size(380, 101);
            tableLayoutPanel8.TabIndex = 0;
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.BackColor = SystemColors.Window;
            tableLayoutPanel12.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.0575523F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.9424438F));
            tableLayoutPanel12.Controls.Add(lblID2_Title_O2, 0, 0);
            tableLayoutPanel12.Controls.Add(lblID2_ActualFlowValue, 1, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(5, 71);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 1;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Size = new Size(135, 25);
            tableLayoutPanel12.TabIndex = 17;
            // 
            // lblID2_Title_O2
            // 
            lblID2_Title_O2.AutoSize = true;
            lblID2_Title_O2.Dock = DockStyle.Fill;
            lblID2_Title_O2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic);
            lblID2_Title_O2.Location = new Point(4, 1);
            lblID2_Title_O2.Name = "lblID2_Title_O2";
            lblID2_Title_O2.Size = new Size(31, 23);
            lblID2_Title_O2.TabIndex = 0;
            lblID2_Title_O2.Text = "O2";
            lblID2_Title_O2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblID2_ActualFlowValue
            // 
            lblID2_ActualFlowValue.AutoSize = true;
            lblID2_ActualFlowValue.Dock = DockStyle.Fill;
            lblID2_ActualFlowValue.Font = new Font("Segoe UI", 9.75F);
            lblID2_ActualFlowValue.ForeColor = SystemColors.WindowText;
            lblID2_ActualFlowValue.Location = new Point(42, 1);
            lblID2_ActualFlowValue.Name = "lblID2_ActualFlowValue";
            lblID2_ActualFlowValue.Size = new Size(89, 23);
            lblID2_ActualFlowValue.TabIndex = 1;
            lblID2_ActualFlowValue.Text = "0.00";
            lblID2_ActualFlowValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numID2_SetPointWrite
            // 
            numID2_SetPointWrite.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numID2_SetPointWrite.DecimalPlaces = 2;
            numID2_SetPointWrite.Font = new Font("Segoe UI", 10F);
            numID2_SetPointWrite.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numID2_SetPointWrite.Location = new Point(148, 71);
            numID2_SetPointWrite.Name = "numID2_SetPointWrite";
            numID2_SetPointWrite.Size = new Size(105, 25);
            numID2_SetPointWrite.TabIndex = 7;
            numID2_SetPointWrite.TextAlign = HorizontalAlignment.Center;
            // 
            // btnID2_ValveToggle
            // 
            btnID2_ValveToggle.AccessibleName = "RF2 Valve O2";
            btnID2_ValveToggle.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnID2_ValveToggle.BackColor = Color.IndianRed;
            btnID2_ValveToggle.FlatStyle = FlatStyle.Flat;
            btnID2_ValveToggle.Font = new Font("Segoe UI", 10F);
            btnID2_ValveToggle.ForeColor = Color.White;
            btnID2_ValveToggle.Location = new Point(261, 71);
            btnID2_ValveToggle.Name = "btnID2_ValveToggle";
            btnID2_ValveToggle.Size = new Size(114, 25);
            btnID2_ValveToggle.TabIndex = 6;
            btnID2_ValveToggle.Text = "CLOSE";
            btnID2_ValveToggle.UseVisualStyleBackColor = false;
            // 
            // label71
            // 
            label71.AutoSize = true;
            label71.Dock = DockStyle.Fill;
            label71.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label71.Location = new Point(261, 2);
            label71.Name = "label71";
            label71.Size = new Size(114, 31);
            label71.TabIndex = 2;
            label71.Text = "VALVE";
            label71.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label72
            // 
            label72.AutoSize = true;
            label72.Dock = DockStyle.Fill;
            label72.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label72.Location = new Point(148, 2);
            label72.Name = "label72";
            label72.Size = new Size(105, 31);
            label72.TabIndex = 1;
            label72.Text = "SETPOINT";
            label72.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label73
            // 
            label73.AutoSize = true;
            label73.Dock = DockStyle.Fill;
            label73.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label73.Location = new Point(5, 2);
            label73.Name = "label73";
            label73.Size = new Size(135, 31);
            label73.TabIndex = 0;
            label73.Text = "ACTUAL FLOW";
            label73.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel13
            // 
            tableLayoutPanel13.BackColor = SystemColors.Window;
            tableLayoutPanel13.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel13.ColumnCount = 2;
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.0575523F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.9424438F));
            tableLayoutPanel13.Controls.Add(lblID1_Title_Ar, 0, 0);
            tableLayoutPanel13.Controls.Add(lblID1_ActualFlowValue, 1, 0);
            tableLayoutPanel13.Dock = DockStyle.Fill;
            tableLayoutPanel13.Location = new Point(5, 38);
            tableLayoutPanel13.Name = "tableLayoutPanel13";
            tableLayoutPanel13.RowCount = 1;
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel13.Size = new Size(135, 25);
            tableLayoutPanel13.TabIndex = 3;
            // 
            // lblID1_Title_Ar
            // 
            lblID1_Title_Ar.AutoSize = true;
            lblID1_Title_Ar.Dock = DockStyle.Fill;
            lblID1_Title_Ar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic);
            lblID1_Title_Ar.Location = new Point(4, 1);
            lblID1_Title_Ar.Name = "lblID1_Title_Ar";
            lblID1_Title_Ar.Size = new Size(31, 23);
            lblID1_Title_Ar.TabIndex = 0;
            lblID1_Title_Ar.Text = "Ar";
            lblID1_Title_Ar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblID1_ActualFlowValue
            // 
            lblID1_ActualFlowValue.AutoSize = true;
            lblID1_ActualFlowValue.Dock = DockStyle.Fill;
            lblID1_ActualFlowValue.Font = new Font("Segoe UI", 9.75F);
            lblID1_ActualFlowValue.ForeColor = SystemColors.WindowText;
            lblID1_ActualFlowValue.Location = new Point(42, 1);
            lblID1_ActualFlowValue.Name = "lblID1_ActualFlowValue";
            lblID1_ActualFlowValue.Size = new Size(89, 23);
            lblID1_ActualFlowValue.TabIndex = 1;
            lblID1_ActualFlowValue.Text = "0.00";
            lblID1_ActualFlowValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numID1_SetPointWrite
            // 
            numID1_SetPointWrite.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numID1_SetPointWrite.DecimalPlaces = 2;
            numID1_SetPointWrite.Font = new Font("Segoe UI", 10F);
            numID1_SetPointWrite.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numID1_SetPointWrite.Location = new Point(148, 38);
            numID1_SetPointWrite.Name = "numID1_SetPointWrite";
            numID1_SetPointWrite.Size = new Size(105, 25);
            numID1_SetPointWrite.TabIndex = 4;
            numID1_SetPointWrite.TextAlign = HorizontalAlignment.Center;
            // 
            // btnID1_ValveToggle
            // 
            btnID1_ValveToggle.AccessibleName = "RF2 Valve Ar";
            btnID1_ValveToggle.BackColor = Color.IndianRed;
            btnID1_ValveToggle.Dock = DockStyle.Fill;
            btnID1_ValveToggle.FlatStyle = FlatStyle.Flat;
            btnID1_ValveToggle.Font = new Font("Segoe UI", 10F);
            btnID1_ValveToggle.ForeColor = Color.White;
            btnID1_ValveToggle.Location = new Point(261, 38);
            btnID1_ValveToggle.Name = "btnID1_ValveToggle";
            btnID1_ValveToggle.Size = new Size(114, 25);
            btnID1_ValveToggle.TabIndex = 3;
            btnID1_ValveToggle.Text = "CLOSE";
            btnID1_ValveToggle.UseVisualStyleBackColor = false;
            // 
            // panel11
            // 
            panel11.BackColor = SystemColors.GradientInactiveCaption;
            panel11.BorderStyle = BorderStyle.Fixed3D;
            panel11.Controls.Add(btnSelectRf2Target);
            panel11.Controls.Add(label38);
            panel11.Controls.Add(txtRf2_Target);
            panel11.Controls.Add(txtRf2_TargetLife);
            panel11.Controls.Add(label39);
            panel11.Controls.Add(label40);
            panel11.Location = new Point(10, 410);
            panel11.Name = "panel11";
            panel11.Size = new Size(386, 68);
            panel11.TabIndex = 16;
            // 
            // btnSelectRf2Target
            // 
            btnSelectRf2Target.FlatStyle = FlatStyle.Popup;
            btnSelectRf2Target.Image = Properties.Resources.select;
            btnSelectRf2Target.ImageAlign = ContentAlignment.MiddleLeft;
            btnSelectRf2Target.Location = new Point(37, 3);
            btnSelectRf2Target.Name = "btnSelectRf2Target";
            btnSelectRf2Target.Size = new Size(107, 23);
            btnSelectRf2Target.TabIndex = 21;
            btnSelectRf2Target.Text = "Select Target";
            btnSelectRf2Target.TextAlign = ContentAlignment.MiddleRight;
            btnSelectRf2Target.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            label38.BackColor = Color.Transparent;
            label38.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label38.Location = new Point(313, 32);
            label38.Name = "label38";
            label38.Padding = new Padding(6, 0, 0, 0);
            label38.Size = new Size(41, 23);
            label38.TabIndex = 19;
            label38.Text = "Wh";
            label38.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtRf2_Target
            // 
            txtRf2_Target.Location = new Point(34, 32);
            txtRf2_Target.Name = "txtRf2_Target";
            txtRf2_Target.ReadOnly = true;
            txtRf2_Target.Size = new Size(113, 23);
            txtRf2_Target.TabIndex = 18;
            txtRf2_Target.Text = "Na";
            txtRf2_Target.TextAlign = HorizontalAlignment.Center;
            // 
            // txtRf2_TargetLife
            // 
            txtRf2_TargetLife.Location = new Point(221, 32);
            txtRf2_TargetLife.Name = "txtRf2_TargetLife";
            txtRf2_TargetLife.ReadOnly = true;
            txtRf2_TargetLife.Size = new Size(96, 23);
            txtRf2_TargetLife.TabIndex = 18;
            txtRf2_TargetLife.Text = "1717.20";
            txtRf2_TargetLife.TextAlign = HorizontalAlignment.Center;
            // 
            // label39
            // 
            label39.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label39.AutoSize = true;
            label39.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label39.Location = new Point(221, 9);
            label39.Name = "label39";
            label39.Padding = new Padding(6, 0, 0, 0);
            label39.Size = new Size(97, 15);
            label39.TabIndex = 17;
            label39.Text = "Used Target Life";
            label39.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label40
            // 
            label40.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label40.AutoSize = true;
            label40.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label40.Location = new Point(56, 9);
            label40.Name = "label40";
            label40.Padding = new Padding(6, 0, 0, 0);
            label40.Size = new Size(68, 15);
            label40.TabIndex = 16;
            label40.Text = "RF2 Target";
            label40.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel12
            // 
            panel12.BackColor = SystemColors.GradientInactiveCaption;
            panel12.BorderStyle = BorderStyle.Fixed3D;
            panel12.Controls.Add(lblRf2_RefP);
            panel12.Controls.Add(lblRf2_FwP);
            panel12.Controls.Add(label43);
            panel12.Controls.Add(pbRf2_RefP);
            panel12.Controls.Add(label44);
            panel12.Controls.Add(pbRf2_FwP);
            panel12.Location = new Point(10, 268);
            panel12.Name = "panel12";
            panel12.Size = new Size(386, 114);
            panel12.TabIndex = 8;
            // 
            // lblRf2_RefP
            // 
            lblRf2_RefP.AutoSize = true;
            lblRf2_RefP.Font = new Font("Segoe UI", 10F);
            lblRf2_RefP.ForeColor = SystemColors.WindowText;
            lblRf2_RefP.Location = new Point(156, 60);
            lblRf2_RefP.Name = "lblRf2_RefP";
            lblRf2_RefP.Size = new Size(17, 19);
            lblRf2_RefP.TabIndex = 12;
            lblRf2_RefP.Text = "0";
            lblRf2_RefP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRf2_FwP
            // 
            lblRf2_FwP.AutoSize = true;
            lblRf2_FwP.Font = new Font("Segoe UI", 10F);
            lblRf2_FwP.ForeColor = SystemColors.WindowText;
            lblRf2_FwP.Location = new Point(152, 4);
            lblRf2_FwP.Name = "lblRf2_FwP";
            lblRf2_FwP.Size = new Size(17, 19);
            lblRf2_FwP.TabIndex = 11;
            lblRf2_FwP.Text = "0";
            lblRf2_FwP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label43
            // 
            label43.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label43.AutoSize = true;
            label43.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label43.Location = new Point(15, 60);
            label43.Name = "label43";
            label43.Padding = new Padding(6, 0, 0, 0);
            label43.Size = new Size(147, 19);
            label43.TabIndex = 10;
            label43.Text = "Reflected Power (W) : ";
            label43.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbRf2_RefP
            // 
            pbRf2_RefP.Location = new Point(15, 80);
            pbRf2_RefP.Maximum = 300;
            pbRf2_RefP.Name = "pbRf2_RefP";
            pbRf2_RefP.Size = new Size(341, 23);
            pbRf2_RefP.Step = 1;
            pbRf2_RefP.TabIndex = 9;
            pbRf2_RefP.Value = 25;
            // 
            // label44
            // 
            label44.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label44.AutoSize = true;
            label44.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label44.Location = new Point(15, 5);
            label44.Name = "label44";
            label44.Padding = new Padding(6, 0, 0, 0);
            label44.Size = new Size(146, 19);
            label44.TabIndex = 9;
            label44.Text = "Forward Power (W) :  ";
            label44.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbRf2_FwP
            // 
            pbRf2_FwP.Location = new Point(15, 25);
            pbRf2_FwP.Maximum = 300;
            pbRf2_FwP.Name = "pbRf2_FwP";
            pbRf2_FwP.Size = new Size(341, 23);
            pbRf2_FwP.Step = 1;
            pbRf2_FwP.TabIndex = 7;
            pbRf2_FwP.Value = 25;
            // 
            // panel13
            // 
            panel13.BackColor = SystemColors.GradientActiveCaption;
            panel13.BorderStyle = BorderStyle.Fixed3D;
            panel13.Controls.Add(btnControlRf2);
            panel13.Location = new Point(10, 16);
            panel13.Name = "panel13";
            panel13.Size = new Size(386, 40);
            panel13.TabIndex = 6;
            // 
            // btnControlRf2
            // 
            btnControlRf2.AccessibleName = "RF2 Supply Command";
            btnControlRf2.Anchor = AnchorStyles.None;
            btnControlRf2.BackColor = Color.IndianRed;
            btnControlRf2.FlatStyle = FlatStyle.Flat;
            btnControlRf2.Font = new Font("Segoe UI", 10F);
            btnControlRf2.ForeColor = Color.White;
            btnControlRf2.Location = new Point(4, 4);
            btnControlRf2.Name = "btnControlRf2";
            btnControlRf2.Size = new Size(374, 29);
            btnControlRf2.TabIndex = 5;
            btnControlRf2.Text = "RF2 OFF";
            btnControlRf2.UseVisualStyleBackColor = false;
            // 
            // panel14
            // 
            panel14.BackColor = SystemColors.GradientInactiveCaption;
            panel14.BorderStyle = BorderStyle.Fixed3D;
            panel14.Controls.Add(numRf2_OffRamp);
            panel14.Controls.Add(numRf2_Step2_Dwell);
            panel14.Controls.Add(numRf2_Step2_Ramp);
            panel14.Controls.Add(label56);
            panel14.Controls.Add(numRf2_Step2_SetW);
            panel14.Controls.Add(label45);
            panel14.Controls.Add(label46);
            panel14.Controls.Add(label47);
            panel14.Controls.Add(label48);
            panel14.Controls.Add(label49);
            panel14.Controls.Add(label50);
            panel14.Controls.Add(label51);
            panel14.Controls.Add(label52);
            panel14.Controls.Add(label53);
            panel14.Controls.Add(txtRf2_Step1_SetW);
            panel14.Controls.Add(txtRf2_Step1_Ramp);
            panel14.Controls.Add(txtRf2_Step1_Dwell);
            panel14.Location = new Point(10, 62);
            panel14.Name = "panel14";
            panel14.Size = new Size(386, 174);
            panel14.TabIndex = 5;
            // 
            // numRf2_OffRamp
            // 
            numRf2_OffRamp.Cursor = Cursors.Hand;
            numRf2_OffRamp.Font = new Font("Segoe UI", 10F);
            numRf2_OffRamp.ForeColor = SystemColors.WindowText;
            numRf2_OffRamp.Location = new Point(152, 137);
            numRf2_OffRamp.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numRf2_OffRamp.Name = "numRf2_OffRamp";
            numRf2_OffRamp.Size = new Size(100, 25);
            numRf2_OffRamp.TabIndex = 27;
            numRf2_OffRamp.TextAlign = HorizontalAlignment.Center;
            numRf2_OffRamp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numRf2_Step2_Dwell
            // 
            numRf2_Step2_Dwell.Cursor = Cursors.Hand;
            numRf2_Step2_Dwell.Font = new Font("Segoe UI", 10F);
            numRf2_Step2_Dwell.ForeColor = SystemColors.WindowText;
            numRf2_Step2_Dwell.Location = new Point(215, 97);
            numRf2_Step2_Dwell.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numRf2_Step2_Dwell.Name = "numRf2_Step2_Dwell";
            numRf2_Step2_Dwell.Size = new Size(100, 25);
            numRf2_Step2_Dwell.TabIndex = 26;
            numRf2_Step2_Dwell.TextAlign = HorizontalAlignment.Center;
            numRf2_Step2_Dwell.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numRf2_Step2_Ramp
            // 
            numRf2_Step2_Ramp.Cursor = Cursors.Hand;
            numRf2_Step2_Ramp.Font = new Font("Segoe UI", 10F);
            numRf2_Step2_Ramp.ForeColor = SystemColors.WindowText;
            numRf2_Step2_Ramp.Location = new Point(215, 62);
            numRf2_Step2_Ramp.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numRf2_Step2_Ramp.Name = "numRf2_Step2_Ramp";
            numRf2_Step2_Ramp.Size = new Size(100, 25);
            numRf2_Step2_Ramp.TabIndex = 25;
            numRf2_Step2_Ramp.TextAlign = HorizontalAlignment.Center;
            numRf2_Step2_Ramp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label56.Location = new Point(257, 142);
            label56.Name = "label56";
            label56.Padding = new Padding(6, 0, 0, 0);
            label56.Size = new Size(38, 15);
            label56.TabIndex = 21;
            label56.Text = "(1-5)";
            label56.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numRf2_Step2_SetW
            // 
            numRf2_Step2_SetW.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numRf2_Step2_SetW.Cursor = Cursors.Hand;
            numRf2_Step2_SetW.Font = new Font("Segoe UI", 10F);
            numRf2_Step2_SetW.ForeColor = SystemColors.WindowText;
            numRf2_Step2_SetW.Location = new Point(215, 29);
            numRf2_Step2_SetW.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numRf2_Step2_SetW.Name = "numRf2_Step2_SetW";
            numRf2_Step2_SetW.Size = new Size(100, 25);
            numRf2_Step2_SetW.TabIndex = 19;
            numRf2_Step2_SetW.TextAlign = HorizontalAlignment.Center;
            numRf2_Step2_SetW.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label45.Location = new Point(45, 141);
            label45.Name = "label45";
            label45.Padding = new Padding(6, 0, 0, 0);
            label45.Size = new Size(105, 17);
            label45.TabIndex = 18;
            label45.Text = "Off Ramp (W/s)";
            label45.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label46.Location = new Point(317, 101);
            label46.Name = "label46";
            label46.Padding = new Padding(6, 0, 0, 0);
            label46.Size = new Size(50, 15);
            label46.TabIndex = 16;
            label46.Text = "(0-999)";
            label46.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label47.Location = new Point(317, 68);
            label47.Name = "label47";
            label47.Padding = new Padding(6, 0, 0, 0);
            label47.Size = new Size(38, 15);
            label47.TabIndex = 15;
            label47.Text = "(0-5)";
            label47.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label48.Location = new Point(317, 34);
            label48.Name = "label48";
            label48.Padding = new Padding(6, 0, 0, 0);
            label48.Size = new Size(50, 15);
            label48.TabIndex = 14;
            label48.Text = "(0-600)";
            label48.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label49.Location = new Point(239, 7);
            label49.Name = "label49";
            label49.Padding = new Padding(6, 0, 0, 0);
            label49.Size = new Size(52, 17);
            label49.TabIndex = 13;
            label49.Text = "Step 2";
            label49.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Font = new Font("Segoe UI", 9F);
            label50.Location = new Point(9, 101);
            label50.Name = "label50";
            label50.Padding = new Padding(6, 0, 0, 0);
            label50.Size = new Size(74, 15);
            label50.TabIndex = 9;
            label50.Text = "Dwell (min)";
            label50.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label51
            // 
            label51.AutoSize = true;
            label51.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label51.Location = new Point(5, 67);
            label51.Name = "label51";
            label51.Padding = new Padding(6, 0, 0, 0);
            label51.Size = new Size(83, 17);
            label51.TabIndex = 8;
            label51.Text = "Ramp (W/s)";
            label51.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label52.Location = new Point(119, 7);
            label52.Name = "label52";
            label52.Padding = new Padding(6, 0, 0, 0);
            label52.Size = new Size(50, 17);
            label52.TabIndex = 7;
            label52.Text = "Step 1";
            label52.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label53.Location = new Point(32, 33);
            label53.Name = "label53";
            label53.Padding = new Padding(6, 0, 0, 0);
            label53.Size = new Size(56, 17);
            label53.TabIndex = 6;
            label53.Text = "Set (W)";
            label53.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtRf2_Step1_SetW
            // 
            txtRf2_Step1_SetW.Location = new Point(94, 30);
            txtRf2_Step1_SetW.Name = "txtRf2_Step1_SetW";
            txtRf2_Step1_SetW.Size = new Size(100, 23);
            txtRf2_Step1_SetW.TabIndex = 0;
            txtRf2_Step1_SetW.Text = "10";
            // 
            // txtRf2_Step1_Ramp
            // 
            txtRf2_Step1_Ramp.Location = new Point(94, 64);
            txtRf2_Step1_Ramp.Name = "txtRf2_Step1_Ramp";
            txtRf2_Step1_Ramp.Size = new Size(100, 23);
            txtRf2_Step1_Ramp.TabIndex = 1;
            txtRf2_Step1_Ramp.Text = "1";
            // 
            // txtRf2_Step1_Dwell
            // 
            txtRf2_Step1_Dwell.Location = new Point(94, 98);
            txtRf2_Step1_Dwell.Name = "txtRf2_Step1_Dwell";
            txtRf2_Step1_Dwell.Size = new Size(100, 23);
            txtRf2_Step1_Dwell.TabIndex = 3;
            txtRf2_Step1_Dwell.Text = "1";
            // 
            // panel15
            // 
            panel15.BackColor = SystemColors.GradientInactiveCaption;
            panel15.BorderStyle = BorderStyle.Fixed3D;
            panel15.Controls.Add(txtRf2_Step2_rTime);
            panel15.Controls.Add(txtRf2_Step1_rTime);
            panel15.Controls.Add(label54);
            panel15.Controls.Add(label55);
            panel15.Location = new Point(10, 506);
            panel15.Name = "panel15";
            panel15.Size = new Size(386, 72);
            panel15.TabIndex = 10;
            // 
            // txtRf2_Step2_rTime
            // 
            txtRf2_Step2_rTime.Font = new Font("Segoe UI", 9.75F);
            txtRf2_Step2_rTime.Location = new Point(213, 35);
            txtRf2_Step2_rTime.Name = "txtRf2_Step2_rTime";
            txtRf2_Step2_rTime.ReadOnly = true;
            txtRf2_Step2_rTime.Size = new Size(113, 25);
            txtRf2_Step2_rTime.TabIndex = 20;
            txtRf2_Step2_rTime.Text = "00:00:00";
            txtRf2_Step2_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // txtRf2_Step1_rTime
            // 
            txtRf2_Step1_rTime.Font = new Font("Segoe UI", 9.75F);
            txtRf2_Step1_rTime.Location = new Point(34, 35);
            txtRf2_Step1_rTime.Name = "txtRf2_Step1_rTime";
            txtRf2_Step1_rTime.ReadOnly = true;
            txtRf2_Step1_rTime.Size = new Size(113, 25);
            txtRf2_Step1_rTime.TabIndex = 19;
            txtRf2_Step1_rTime.Text = "00:00:00";
            txtRf2_Step1_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // label54
            // 
            label54.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label54.AutoSize = true;
            label54.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label54.Location = new Point(204, 13);
            label54.Name = "label54";
            label54.Padding = new Padding(6, 0, 0, 0);
            label54.Size = new Size(130, 15);
            label54.TabIndex = 18;
            label54.Text = "Step-2 Remainig Time";
            label54.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label55
            // 
            label55.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label55.AutoSize = true;
            label55.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label55.Location = new Point(25, 13);
            label55.Name = "label55";
            label55.Padding = new Padding(6, 0, 0, 0);
            label55.Size = new Size(130, 15);
            label55.TabIndex = 17;
            label55.Text = "Step-1 Remainig Time";
            label55.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpRf1
            // 
            grpRf1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpRf1.BackColor = Color.LightSteelBlue;
            grpRf1.Controls.Add(groupBox2);
            grpRf1.Controls.Add(panel6);
            grpRf1.Controls.Add(panel7);
            grpRf1.Controls.Add(panel8);
            grpRf1.Controls.Add(panel9);
            grpRf1.Controls.Add(panel10);
            grpRf1.Location = new Point(873, 6);
            grpRf1.Name = "grpRf1";
            grpRf1.Size = new Size(408, 756);
            grpRf1.TabIndex = 18;
            grpRf1.TabStop = false;
            grpRf1.Text = "RF1 Control";
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.GradientInactiveCaption;
            groupBox2.Controls.Add(tableLayoutPanel7);
            groupBox2.Location = new Point(6, 627);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(386, 90);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "MFC Control RF1";
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel7.ColumnCount = 3;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel7.Controls.Add(label42, 2, 0);
            tableLayoutPanel7.Controls.Add(label60, 1, 0);
            tableLayoutPanel7.Controls.Add(label61, 0, 0);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel9, 0, 1);
            tableLayoutPanel7.Controls.Add(numID3_SetPointWrite, 1, 1);
            tableLayoutPanel7.Controls.Add(btnID3_ValveToggle, 2, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(3, 19);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new Size(380, 68);
            tableLayoutPanel7.TabIndex = 0;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Dock = DockStyle.Fill;
            label42.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label42.Location = new Point(261, 2);
            label42.Name = "label42";
            label42.Size = new Size(114, 31);
            label42.TabIndex = 2;
            label42.Text = "VALVE";
            label42.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label60
            // 
            label60.AutoSize = true;
            label60.Dock = DockStyle.Fill;
            label60.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label60.Location = new Point(148, 2);
            label60.Name = "label60";
            label60.Size = new Size(105, 31);
            label60.TabIndex = 1;
            label60.Text = "SETPOINT";
            label60.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label61
            // 
            label61.AutoSize = true;
            label61.Dock = DockStyle.Fill;
            label61.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label61.Location = new Point(5, 2);
            label61.Name = "label61";
            label61.Size = new Size(135, 31);
            label61.TabIndex = 0;
            label61.Text = "ACTUAL FLOW";
            label61.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.BackColor = SystemColors.Window;
            tableLayoutPanel9.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.0575523F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.9424438F));
            tableLayoutPanel9.Controls.Add(lblID3_Title_Ar, 0, 0);
            tableLayoutPanel9.Controls.Add(lblID3_ActualFlowValue, 1, 0);
            tableLayoutPanel9.Location = new Point(5, 38);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel9.Size = new Size(135, 23);
            tableLayoutPanel9.TabIndex = 3;
            // 
            // lblID3_Title_Ar
            // 
            lblID3_Title_Ar.AutoSize = true;
            lblID3_Title_Ar.Dock = DockStyle.Fill;
            lblID3_Title_Ar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic);
            lblID3_Title_Ar.Location = new Point(4, 1);
            lblID3_Title_Ar.Name = "lblID3_Title_Ar";
            lblID3_Title_Ar.Size = new Size(31, 21);
            lblID3_Title_Ar.TabIndex = 0;
            lblID3_Title_Ar.Text = "Ar";
            lblID3_Title_Ar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblID3_ActualFlowValue
            // 
            lblID3_ActualFlowValue.AutoSize = true;
            lblID3_ActualFlowValue.Dock = DockStyle.Fill;
            lblID3_ActualFlowValue.Font = new Font("Segoe UI", 9.75F);
            lblID3_ActualFlowValue.ForeColor = SystemColors.WindowText;
            lblID3_ActualFlowValue.Location = new Point(42, 1);
            lblID3_ActualFlowValue.Name = "lblID3_ActualFlowValue";
            lblID3_ActualFlowValue.Size = new Size(89, 21);
            lblID3_ActualFlowValue.TabIndex = 1;
            lblID3_ActualFlowValue.Text = "0.00";
            lblID3_ActualFlowValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numID3_SetPointWrite
            // 
            numID3_SetPointWrite.DecimalPlaces = 2;
            numID3_SetPointWrite.Dock = DockStyle.Fill;
            numID3_SetPointWrite.Font = new Font("Segoe UI", 10F);
            numID3_SetPointWrite.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numID3_SetPointWrite.Location = new Point(148, 38);
            numID3_SetPointWrite.Name = "numID3_SetPointWrite";
            numID3_SetPointWrite.Size = new Size(105, 25);
            numID3_SetPointWrite.TabIndex = 4;
            numID3_SetPointWrite.TextAlign = HorizontalAlignment.Center;
            // 
            // btnID3_ValveToggle
            // 
            btnID3_ValveToggle.AccessibleName = "RF1 Valve Ar";
            btnID3_ValveToggle.BackColor = Color.IndianRed;
            btnID3_ValveToggle.Dock = DockStyle.Fill;
            btnID3_ValveToggle.FlatStyle = FlatStyle.Flat;
            btnID3_ValveToggle.Font = new Font("Segoe UI", 10F);
            btnID3_ValveToggle.ForeColor = Color.White;
            btnID3_ValveToggle.Location = new Point(261, 38);
            btnID3_ValveToggle.Name = "btnID3_ValveToggle";
            btnID3_ValveToggle.Size = new Size(114, 25);
            btnID3_ValveToggle.TabIndex = 3;
            btnID3_ValveToggle.Text = "CLOSE";
            btnID3_ValveToggle.UseVisualStyleBackColor = false;
            // 
            // panel6
            // 
            panel6.BackColor = SystemColors.GradientInactiveCaption;
            panel6.BorderStyle = BorderStyle.Fixed3D;
            panel6.Controls.Add(btnSelectRf1Target);
            panel6.Controls.Add(label20);
            panel6.Controls.Add(txtRf1_Target);
            panel6.Controls.Add(txtRf1_TargetLife);
            panel6.Controls.Add(label21);
            panel6.Controls.Add(label22);
            panel6.Location = new Point(10, 410);
            panel6.Name = "panel6";
            panel6.Size = new Size(386, 68);
            panel6.TabIndex = 16;
            // 
            // btnSelectRf1Target
            // 
            btnSelectRf1Target.FlatStyle = FlatStyle.Popup;
            btnSelectRf1Target.Image = Properties.Resources.select;
            btnSelectRf1Target.ImageAlign = ContentAlignment.MiddleLeft;
            btnSelectRf1Target.Location = new Point(37, 5);
            btnSelectRf1Target.Name = "btnSelectRf1Target";
            btnSelectRf1Target.Size = new Size(107, 23);
            btnSelectRf1Target.TabIndex = 21;
            btnSelectRf1Target.Text = "Select Target";
            btnSelectRf1Target.TextAlign = ContentAlignment.MiddleRight;
            btnSelectRf1Target.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            label20.BackColor = Color.Transparent;
            label20.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label20.Location = new Point(313, 32);
            label20.Name = "label20";
            label20.Padding = new Padding(6, 0, 0, 0);
            label20.Size = new Size(41, 23);
            label20.TabIndex = 19;
            label20.Text = "Wh";
            label20.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtRf1_Target
            // 
            txtRf1_Target.Location = new Point(34, 32);
            txtRf1_Target.Name = "txtRf1_Target";
            txtRf1_Target.ReadOnly = true;
            txtRf1_Target.Size = new Size(113, 23);
            txtRf1_Target.TabIndex = 18;
            txtRf1_Target.Text = "Na";
            txtRf1_Target.TextAlign = HorizontalAlignment.Center;
            // 
            // txtRf1_TargetLife
            // 
            txtRf1_TargetLife.Location = new Point(221, 32);
            txtRf1_TargetLife.Name = "txtRf1_TargetLife";
            txtRf1_TargetLife.ReadOnly = true;
            txtRf1_TargetLife.Size = new Size(96, 23);
            txtRf1_TargetLife.TabIndex = 18;
            txtRf1_TargetLife.Text = "1576.74";
            txtRf1_TargetLife.TextAlign = HorizontalAlignment.Center;
            // 
            // label21
            // 
            label21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label21.Location = new Point(221, 9);
            label21.Name = "label21";
            label21.Padding = new Padding(6, 0, 0, 0);
            label21.Size = new Size(97, 15);
            label21.TabIndex = 17;
            label21.Text = "Used Target Life";
            label21.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label22
            // 
            label22.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label22.Location = new Point(56, 9);
            label22.Name = "label22";
            label22.Padding = new Padding(6, 0, 0, 0);
            label22.Size = new Size(68, 15);
            label22.TabIndex = 16;
            label22.Text = "RF1 Target";
            label22.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel7
            // 
            panel7.BackColor = SystemColors.GradientInactiveCaption;
            panel7.BorderStyle = BorderStyle.Fixed3D;
            panel7.Controls.Add(lblRf1_RefP);
            panel7.Controls.Add(lblRf1_FwP);
            panel7.Controls.Add(label25);
            panel7.Controls.Add(pbRf1_RefP);
            panel7.Controls.Add(lbl);
            panel7.Controls.Add(pbRf1_FwP);
            panel7.Location = new Point(10, 268);
            panel7.Name = "panel7";
            panel7.Size = new Size(386, 114);
            panel7.TabIndex = 8;
            // 
            // lblRf1_RefP
            // 
            lblRf1_RefP.AutoSize = true;
            lblRf1_RefP.Font = new Font("Segoe UI", 10F);
            lblRf1_RefP.ForeColor = SystemColors.WindowText;
            lblRf1_RefP.Location = new Point(156, 60);
            lblRf1_RefP.Name = "lblRf1_RefP";
            lblRf1_RefP.Size = new Size(17, 19);
            lblRf1_RefP.TabIndex = 12;
            lblRf1_RefP.Text = "0";
            lblRf1_RefP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRf1_FwP
            // 
            lblRf1_FwP.AutoSize = true;
            lblRf1_FwP.Font = new Font("Segoe UI", 10F);
            lblRf1_FwP.ForeColor = SystemColors.WindowText;
            lblRf1_FwP.Location = new Point(152, 4);
            lblRf1_FwP.Name = "lblRf1_FwP";
            lblRf1_FwP.Size = new Size(17, 19);
            lblRf1_FwP.TabIndex = 11;
            lblRf1_FwP.Text = "0";
            lblRf1_FwP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            label25.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label25.Location = new Point(15, 60);
            label25.Name = "label25";
            label25.Padding = new Padding(6, 0, 0, 0);
            label25.Size = new Size(147, 19);
            label25.TabIndex = 10;
            label25.Text = "Reflected Power (W) : ";
            label25.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbRf1_RefP
            // 
            pbRf1_RefP.Location = new Point(15, 80);
            pbRf1_RefP.Maximum = 300;
            pbRf1_RefP.Name = "pbRf1_RefP";
            pbRf1_RefP.Size = new Size(341, 23);
            pbRf1_RefP.Step = 1;
            pbRf1_RefP.TabIndex = 9;
            pbRf1_RefP.Value = 25;
            // 
            // lbl
            // 
            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lbl.Location = new Point(15, 5);
            lbl.Name = "lbl";
            lbl.Padding = new Padding(6, 0, 0, 0);
            lbl.Size = new Size(146, 19);
            lbl.TabIndex = 9;
            lbl.Text = "Forward Power (W) :  ";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pbRf1_FwP
            // 
            pbRf1_FwP.Location = new Point(15, 25);
            pbRf1_FwP.Maximum = 300;
            pbRf1_FwP.Name = "pbRf1_FwP";
            pbRf1_FwP.Size = new Size(341, 23);
            pbRf1_FwP.Step = 1;
            pbRf1_FwP.TabIndex = 7;
            pbRf1_FwP.Value = 100;
            // 
            // panel8
            // 
            panel8.BackColor = SystemColors.GradientActiveCaption;
            panel8.BorderStyle = BorderStyle.Fixed3D;
            panel8.Controls.Add(btnControlRf1);
            panel8.Location = new Point(10, 16);
            panel8.Name = "panel8";
            panel8.Size = new Size(386, 40);
            panel8.TabIndex = 6;
            // 
            // btnControlRf1
            // 
            btnControlRf1.AccessibleName = "RF1 Supply Command";
            btnControlRf1.Anchor = AnchorStyles.None;
            btnControlRf1.BackColor = Color.IndianRed;
            btnControlRf1.FlatStyle = FlatStyle.Flat;
            btnControlRf1.Font = new Font("Segoe UI", 10F);
            btnControlRf1.ForeColor = Color.White;
            btnControlRf1.Location = new Point(4, 4);
            btnControlRf1.Name = "btnControlRf1";
            btnControlRf1.Size = new Size(374, 29);
            btnControlRf1.TabIndex = 5;
            btnControlRf1.Text = "RF1 OFF";
            btnControlRf1.UseVisualStyleBackColor = false;
            // 
            // panel9
            // 
            panel9.BackColor = SystemColors.GradientInactiveCaption;
            panel9.BorderStyle = BorderStyle.Fixed3D;
            panel9.Controls.Add(label14);
            panel9.Controls.Add(numRf1_Step2_SetW);
            panel9.Controls.Add(numRf1_OffRamp);
            panel9.Controls.Add(numRf1_Step2_Dwell);
            panel9.Controls.Add(numRf1_Step2_Ramp);
            panel9.Controls.Add(lblRf1_OffRamp);
            panel9.Controls.Add(label28);
            panel9.Controls.Add(label29);
            panel9.Controls.Add(label30);
            panel9.Controls.Add(label31);
            panel9.Controls.Add(lblRf2_Dwell);
            panel9.Controls.Add(lblRf2_Ramp);
            panel9.Controls.Add(label34);
            panel9.Controls.Add(lblRf2_Set);
            panel9.Controls.Add(txtRf1_Step1_SetW);
            panel9.Controls.Add(txtRf1_Step1_Ramp);
            panel9.Controls.Add(txtRf1_Step1_Dwell);
            panel9.Location = new Point(10, 62);
            panel9.Name = "panel9";
            panel9.Size = new Size(386, 174);
            panel9.TabIndex = 5;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label14.Location = new Point(257, 142);
            label14.Name = "label14";
            label14.Padding = new Padding(6, 0, 0, 0);
            label14.Size = new Size(38, 15);
            label14.TabIndex = 20;
            label14.Text = "(1-5)";
            label14.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numRf1_Step2_SetW
            // 
            numRf1_Step2_SetW.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numRf1_Step2_SetW.Cursor = Cursors.Hand;
            numRf1_Step2_SetW.Font = new Font("Segoe UI", 10F);
            numRf1_Step2_SetW.ForeColor = SystemColors.WindowText;
            numRf1_Step2_SetW.Location = new Point(215, 29);
            numRf1_Step2_SetW.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numRf1_Step2_SetW.Name = "numRf1_Step2_SetW";
            numRf1_Step2_SetW.Size = new Size(100, 25);
            numRf1_Step2_SetW.TabIndex = 19;
            numRf1_Step2_SetW.TextAlign = HorizontalAlignment.Center;
            numRf1_Step2_SetW.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // numRf1_OffRamp
            // 
            numRf1_OffRamp.Cursor = Cursors.Hand;
            numRf1_OffRamp.Font = new Font("Segoe UI", 10F);
            numRf1_OffRamp.ForeColor = SystemColors.WindowText;
            numRf1_OffRamp.Location = new Point(155, 137);
            numRf1_OffRamp.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numRf1_OffRamp.Name = "numRf1_OffRamp";
            numRf1_OffRamp.Size = new Size(100, 25);
            numRf1_OffRamp.TabIndex = 24;
            numRf1_OffRamp.TextAlign = HorizontalAlignment.Center;
            numRf1_OffRamp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numRf1_Step2_Dwell
            // 
            numRf1_Step2_Dwell.Cursor = Cursors.Hand;
            numRf1_Step2_Dwell.Font = new Font("Segoe UI", 10F);
            numRf1_Step2_Dwell.ForeColor = SystemColors.WindowText;
            numRf1_Step2_Dwell.Location = new Point(215, 97);
            numRf1_Step2_Dwell.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numRf1_Step2_Dwell.Name = "numRf1_Step2_Dwell";
            numRf1_Step2_Dwell.Size = new Size(100, 25);
            numRf1_Step2_Dwell.TabIndex = 24;
            numRf1_Step2_Dwell.TextAlign = HorizontalAlignment.Center;
            numRf1_Step2_Dwell.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numRf1_Step2_Ramp
            // 
            numRf1_Step2_Ramp.Cursor = Cursors.Hand;
            numRf1_Step2_Ramp.Font = new Font("Segoe UI", 10F);
            numRf1_Step2_Ramp.ForeColor = SystemColors.WindowText;
            numRf1_Step2_Ramp.Location = new Point(215, 63);
            numRf1_Step2_Ramp.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numRf1_Step2_Ramp.Name = "numRf1_Step2_Ramp";
            numRf1_Step2_Ramp.Size = new Size(100, 25);
            numRf1_Step2_Ramp.TabIndex = 24;
            numRf1_Step2_Ramp.TextAlign = HorizontalAlignment.Center;
            numRf1_Step2_Ramp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblRf1_OffRamp
            // 
            lblRf1_OffRamp.AutoSize = true;
            lblRf1_OffRamp.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblRf1_OffRamp.Location = new Point(45, 141);
            lblRf1_OffRamp.Name = "lblRf1_OffRamp";
            lblRf1_OffRamp.Padding = new Padding(6, 0, 0, 0);
            lblRf1_OffRamp.Size = new Size(105, 17);
            lblRf1_OffRamp.TabIndex = 18;
            lblRf1_OffRamp.Text = "Off Ramp (W/s)";
            lblRf1_OffRamp.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label28.Location = new Point(317, 101);
            label28.Name = "label28";
            label28.Padding = new Padding(6, 0, 0, 0);
            label28.Size = new Size(50, 15);
            label28.TabIndex = 16;
            label28.Text = "(0-999)";
            label28.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label29.Location = new Point(317, 68);
            label29.Name = "label29";
            label29.Padding = new Padding(6, 0, 0, 0);
            label29.Size = new Size(38, 15);
            label29.TabIndex = 15;
            label29.Text = "(0-5)";
            label29.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label30.Location = new Point(317, 34);
            label30.Name = "label30";
            label30.Padding = new Padding(6, 0, 0, 0);
            label30.Size = new Size(50, 15);
            label30.TabIndex = 14;
            label30.Text = "(0-600)";
            label30.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label31.Location = new Point(239, 7);
            label31.Name = "label31";
            label31.Padding = new Padding(6, 0, 0, 0);
            label31.Size = new Size(52, 17);
            label31.TabIndex = 13;
            label31.Text = "Step 2";
            label31.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRf2_Dwell
            // 
            lblRf2_Dwell.AutoSize = true;
            lblRf2_Dwell.Font = new Font("Segoe UI", 9F);
            lblRf2_Dwell.Location = new Point(9, 101);
            lblRf2_Dwell.Name = "lblRf2_Dwell";
            lblRf2_Dwell.Padding = new Padding(6, 0, 0, 0);
            lblRf2_Dwell.Size = new Size(74, 15);
            lblRf2_Dwell.TabIndex = 9;
            lblRf2_Dwell.Text = "Dwell (min)";
            lblRf2_Dwell.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblRf2_Ramp
            // 
            lblRf2_Ramp.AutoSize = true;
            lblRf2_Ramp.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblRf2_Ramp.Location = new Point(5, 67);
            lblRf2_Ramp.Name = "lblRf2_Ramp";
            lblRf2_Ramp.Padding = new Padding(6, 0, 0, 0);
            lblRf2_Ramp.Size = new Size(83, 17);
            lblRf2_Ramp.TabIndex = 8;
            lblRf2_Ramp.Text = "Ramp (W/s)";
            lblRf2_Ramp.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label34.Location = new Point(119, 7);
            label34.Name = "label34";
            label34.Padding = new Padding(6, 0, 0, 0);
            label34.Size = new Size(50, 17);
            label34.TabIndex = 7;
            label34.Text = "Step 1";
            label34.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRf2_Set
            // 
            lblRf2_Set.AutoSize = true;
            lblRf2_Set.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblRf2_Set.Location = new Point(32, 33);
            lblRf2_Set.Name = "lblRf2_Set";
            lblRf2_Set.Padding = new Padding(6, 0, 0, 0);
            lblRf2_Set.Size = new Size(56, 17);
            lblRf2_Set.TabIndex = 6;
            lblRf2_Set.Text = "Set (W)";
            lblRf2_Set.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtRf1_Step1_SetW
            // 
            txtRf1_Step1_SetW.Location = new Point(94, 30);
            txtRf1_Step1_SetW.Name = "txtRf1_Step1_SetW";
            txtRf1_Step1_SetW.Size = new Size(100, 23);
            txtRf1_Step1_SetW.TabIndex = 0;
            txtRf1_Step1_SetW.Text = "10";
            // 
            // txtRf1_Step1_Ramp
            // 
            txtRf1_Step1_Ramp.Location = new Point(94, 64);
            txtRf1_Step1_Ramp.Name = "txtRf1_Step1_Ramp";
            txtRf1_Step1_Ramp.Size = new Size(100, 23);
            txtRf1_Step1_Ramp.TabIndex = 1;
            txtRf1_Step1_Ramp.Text = "1";
            // 
            // txtRf1_Step1_Dwell
            // 
            txtRf1_Step1_Dwell.Location = new Point(94, 98);
            txtRf1_Step1_Dwell.Name = "txtRf1_Step1_Dwell";
            txtRf1_Step1_Dwell.Size = new Size(100, 23);
            txtRf1_Step1_Dwell.TabIndex = 3;
            txtRf1_Step1_Dwell.Text = "1";
            // 
            // panel10
            // 
            panel10.BackColor = SystemColors.GradientInactiveCaption;
            panel10.BorderStyle = BorderStyle.Fixed3D;
            panel10.Controls.Add(txtRf1_Step2_rTime);
            panel10.Controls.Add(txtRf1_Step1_rTime);
            panel10.Controls.Add(label36);
            panel10.Controls.Add(label37);
            panel10.Location = new Point(10, 506);
            panel10.Name = "panel10";
            panel10.Size = new Size(386, 72);
            panel10.TabIndex = 10;
            // 
            // txtRf1_Step2_rTime
            // 
            txtRf1_Step2_rTime.Font = new Font("Segoe UI", 9.75F);
            txtRf1_Step2_rTime.Location = new Point(213, 35);
            txtRf1_Step2_rTime.Name = "txtRf1_Step2_rTime";
            txtRf1_Step2_rTime.ReadOnly = true;
            txtRf1_Step2_rTime.Size = new Size(113, 25);
            txtRf1_Step2_rTime.TabIndex = 20;
            txtRf1_Step2_rTime.Text = "00:00:00";
            txtRf1_Step2_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // txtRf1_Step1_rTime
            // 
            txtRf1_Step1_rTime.Font = new Font("Segoe UI", 9.75F);
            txtRf1_Step1_rTime.Location = new Point(34, 35);
            txtRf1_Step1_rTime.Name = "txtRf1_Step1_rTime";
            txtRf1_Step1_rTime.ReadOnly = true;
            txtRf1_Step1_rTime.Size = new Size(113, 25);
            txtRf1_Step1_rTime.TabIndex = 19;
            txtRf1_Step1_rTime.Text = "00:00:00";
            txtRf1_Step1_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // label36
            // 
            label36.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label36.AutoSize = true;
            label36.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label36.Location = new Point(204, 13);
            label36.Name = "label36";
            label36.Padding = new Padding(6, 0, 0, 0);
            label36.Size = new Size(130, 15);
            label36.TabIndex = 18;
            label36.Text = "Step-2 Remainig Time";
            label36.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label37
            // 
            label37.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label37.AutoSize = true;
            label37.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label37.Location = new Point(25, 13);
            label37.Name = "label37";
            label37.Padding = new Padding(6, 0, 0, 0);
            label37.Size = new Size(130, 15);
            label37.TabIndex = 17;
            label37.Text = "Step-1 Remainig Time";
            label37.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpDC
            // 
            grpDC.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDC.BackColor = Color.LightSteelBlue;
            grpDC.Controls.Add(groupBox1);
            grpDC.Controls.Add(panel5);
            grpDC.Controls.Add(panel3);
            grpDC.Controls.Add(panel2);
            grpDC.Controls.Add(panel1);
            grpDC.Controls.Add(panel4);
            grpDC.Location = new Point(442, 6);
            grpDC.Name = "grpDC";
            grpDC.Size = new Size(408, 756);
            grpDC.TabIndex = 19;
            grpDC.TabStop = false;
            grpDC.Text = "DC Control";
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.GradientInactiveCaption;
            groupBox1.Controls.Add(tableLayoutPanel6);
            groupBox1.Location = new Point(10, 606);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(386, 132);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "MFC Control DC";
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel6.ColumnCount = 3;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel6.Controls.Add(tableLayoutPanel10, 0, 2);
            tableLayoutPanel6.Controls.Add(numID5_SetPointWrite, 1, 2);
            tableLayoutPanel6.Controls.Add(btnID5_ValveToggle, 2, 2);
            tableLayoutPanel6.Controls.Add(label65, 2, 0);
            tableLayoutPanel6.Controls.Add(label66, 1, 0);
            tableLayoutPanel6.Controls.Add(label67, 0, 0);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel11, 0, 1);
            tableLayoutPanel6.Controls.Add(numID4_SetPointWrite, 1, 1);
            tableLayoutPanel6.Controls.Add(btnID4_ValveToggle, 2, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 19);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 3;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel6.Size = new Size(380, 110);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.BackColor = SystemColors.Window;
            tableLayoutPanel10.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.0575523F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.9424438F));
            tableLayoutPanel10.Controls.Add(lblID5_Title_O2, 0, 0);
            tableLayoutPanel10.Controls.Add(lblID5_ActualFlowValue, 1, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(5, 75);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel10.Size = new Size(135, 30);
            tableLayoutPanel10.TabIndex = 17;
            // 
            // lblID5_Title_O2
            // 
            lblID5_Title_O2.AutoSize = true;
            lblID5_Title_O2.Dock = DockStyle.Fill;
            lblID5_Title_O2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic);
            lblID5_Title_O2.Location = new Point(4, 1);
            lblID5_Title_O2.Name = "lblID5_Title_O2";
            lblID5_Title_O2.Size = new Size(31, 28);
            lblID5_Title_O2.TabIndex = 0;
            lblID5_Title_O2.Text = "O2";
            lblID5_Title_O2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblID5_ActualFlowValue
            // 
            lblID5_ActualFlowValue.AutoSize = true;
            lblID5_ActualFlowValue.Dock = DockStyle.Fill;
            lblID5_ActualFlowValue.Font = new Font("Segoe UI", 9.75F);
            lblID5_ActualFlowValue.ForeColor = SystemColors.WindowText;
            lblID5_ActualFlowValue.Location = new Point(42, 1);
            lblID5_ActualFlowValue.Name = "lblID5_ActualFlowValue";
            lblID5_ActualFlowValue.Size = new Size(89, 28);
            lblID5_ActualFlowValue.TabIndex = 1;
            lblID5_ActualFlowValue.Text = "0.00";
            lblID5_ActualFlowValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numID5_SetPointWrite
            // 
            numID5_SetPointWrite.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numID5_SetPointWrite.DecimalPlaces = 2;
            numID5_SetPointWrite.Font = new Font("Segoe UI", 10F);
            numID5_SetPointWrite.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numID5_SetPointWrite.Location = new Point(148, 77);
            numID5_SetPointWrite.Name = "numID5_SetPointWrite";
            numID5_SetPointWrite.Size = new Size(105, 25);
            numID5_SetPointWrite.TabIndex = 7;
            numID5_SetPointWrite.TextAlign = HorizontalAlignment.Center;
            // 
            // btnID5_ValveToggle
            // 
            btnID5_ValveToggle.AccessibleName = "DC Valve O2";
            btnID5_ValveToggle.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnID5_ValveToggle.BackColor = Color.IndianRed;
            btnID5_ValveToggle.FlatStyle = FlatStyle.Flat;
            btnID5_ValveToggle.Font = new Font("Segoe UI", 10F);
            btnID5_ValveToggle.ForeColor = Color.White;
            btnID5_ValveToggle.Location = new Point(261, 77);
            btnID5_ValveToggle.Name = "btnID5_ValveToggle";
            btnID5_ValveToggle.Size = new Size(114, 25);
            btnID5_ValveToggle.TabIndex = 6;
            btnID5_ValveToggle.Text = "CLOSE";
            btnID5_ValveToggle.UseVisualStyleBackColor = false;
            // 
            // label65
            // 
            label65.AutoSize = true;
            label65.Dock = DockStyle.Fill;
            label65.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label65.Location = new Point(261, 2);
            label65.Name = "label65";
            label65.Size = new Size(114, 33);
            label65.TabIndex = 2;
            label65.Text = "VALVE";
            label65.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label66
            // 
            label66.AutoSize = true;
            label66.Dock = DockStyle.Fill;
            label66.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label66.Location = new Point(148, 2);
            label66.Name = "label66";
            label66.Size = new Size(105, 33);
            label66.TabIndex = 1;
            label66.Text = "SETPOINT";
            label66.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label67
            // 
            label67.AutoSize = true;
            label67.Dock = DockStyle.Fill;
            label67.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label67.Location = new Point(5, 2);
            label67.Name = "label67";
            label67.Size = new Size(135, 33);
            label67.TabIndex = 0;
            label67.Text = "ACTUAL FLOW";
            label67.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.BackColor = SystemColors.Window;
            tableLayoutPanel11.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel11.ColumnCount = 2;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.0575523F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.9424438F));
            tableLayoutPanel11.Controls.Add(lblID4_Title_Ar, 0, 0);
            tableLayoutPanel11.Controls.Add(lblID4_ActualFlowValue, 1, 0);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(5, 40);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 1;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.Size = new Size(135, 27);
            tableLayoutPanel11.TabIndex = 3;
            // 
            // lblID4_Title_Ar
            // 
            lblID4_Title_Ar.AutoSize = true;
            lblID4_Title_Ar.Dock = DockStyle.Fill;
            lblID4_Title_Ar.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold | FontStyle.Italic);
            lblID4_Title_Ar.Location = new Point(4, 1);
            lblID4_Title_Ar.Name = "lblID4_Title_Ar";
            lblID4_Title_Ar.Size = new Size(31, 25);
            lblID4_Title_Ar.TabIndex = 0;
            lblID4_Title_Ar.Text = "Ar";
            lblID4_Title_Ar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblID4_ActualFlowValue
            // 
            lblID4_ActualFlowValue.AutoSize = true;
            lblID4_ActualFlowValue.Dock = DockStyle.Fill;
            lblID4_ActualFlowValue.Font = new Font("Segoe UI", 9.75F);
            lblID4_ActualFlowValue.ForeColor = SystemColors.WindowText;
            lblID4_ActualFlowValue.Location = new Point(42, 1);
            lblID4_ActualFlowValue.Name = "lblID4_ActualFlowValue";
            lblID4_ActualFlowValue.Size = new Size(89, 25);
            lblID4_ActualFlowValue.TabIndex = 1;
            lblID4_ActualFlowValue.Text = "0.00";
            lblID4_ActualFlowValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numID4_SetPointWrite
            // 
            numID4_SetPointWrite.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numID4_SetPointWrite.DecimalPlaces = 2;
            numID4_SetPointWrite.Font = new Font("Segoe UI", 10F);
            numID4_SetPointWrite.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numID4_SetPointWrite.Location = new Point(148, 41);
            numID4_SetPointWrite.Name = "numID4_SetPointWrite";
            numID4_SetPointWrite.Size = new Size(105, 25);
            numID4_SetPointWrite.TabIndex = 4;
            numID4_SetPointWrite.TextAlign = HorizontalAlignment.Center;
            // 
            // btnID4_ValveToggle
            // 
            btnID4_ValveToggle.AccessibleName = "DC Valve Ar";
            btnID4_ValveToggle.BackColor = Color.IndianRed;
            btnID4_ValveToggle.Dock = DockStyle.Fill;
            btnID4_ValveToggle.FlatStyle = FlatStyle.Flat;
            btnID4_ValveToggle.Font = new Font("Segoe UI", 10F);
            btnID4_ValveToggle.ForeColor = Color.White;
            btnID4_ValveToggle.Location = new Point(261, 40);
            btnID4_ValveToggle.Name = "btnID4_ValveToggle";
            btnID4_ValveToggle.Size = new Size(114, 27);
            btnID4_ValveToggle.TabIndex = 3;
            btnID4_ValveToggle.Text = "CLOSE";
            btnID4_ValveToggle.UseVisualStyleBackColor = false;
            // 
            // panel5
            // 
            panel5.BackColor = SystemColors.GradientInactiveCaption;
            panel5.BorderStyle = BorderStyle.Fixed3D;
            panel5.Controls.Add(btnSelectDcTarget);
            panel5.Controls.Add(label17);
            panel5.Controls.Add(txtDC_Target);
            panel5.Controls.Add(txtDC_TargetLife);
            panel5.Controls.Add(label16);
            panel5.Controls.Add(label15);
            panel5.Location = new Point(10, 410);
            panel5.Name = "panel5";
            panel5.Size = new Size(386, 68);
            panel5.TabIndex = 16;
            // 
            // btnSelectDcTarget
            // 
            btnSelectDcTarget.FlatStyle = FlatStyle.Popup;
            btnSelectDcTarget.Image = Properties.Resources.select;
            btnSelectDcTarget.ImageAlign = ContentAlignment.MiddleLeft;
            btnSelectDcTarget.Location = new Point(37, 5);
            btnSelectDcTarget.Name = "btnSelectDcTarget";
            btnSelectDcTarget.Size = new Size(107, 23);
            btnSelectDcTarget.TabIndex = 21;
            btnSelectDcTarget.Text = "Select Target";
            btnSelectDcTarget.TextAlign = ContentAlignment.MiddleRight;
            btnSelectDcTarget.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            label17.BackColor = Color.Transparent;
            label17.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label17.Location = new Point(313, 32);
            label17.Name = "label17";
            label17.Padding = new Padding(6, 0, 0, 0);
            label17.Size = new Size(41, 23);
            label17.TabIndex = 19;
            label17.Text = "Wh";
            label17.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDC_Target
            // 
            txtDC_Target.Location = new Point(34, 32);
            txtDC_Target.Name = "txtDC_Target";
            txtDC_Target.ReadOnly = true;
            txtDC_Target.Size = new Size(113, 23);
            txtDC_Target.TabIndex = 18;
            txtDC_Target.Text = "Ag";
            txtDC_Target.TextAlign = HorizontalAlignment.Center;
            // 
            // txtDC_TargetLife
            // 
            txtDC_TargetLife.Location = new Point(221, 32);
            txtDC_TargetLife.Name = "txtDC_TargetLife";
            txtDC_TargetLife.ReadOnly = true;
            txtDC_TargetLife.Size = new Size(96, 23);
            txtDC_TargetLife.TabIndex = 18;
            txtDC_TargetLife.Text = "30.38";
            txtDC_TargetLife.TextAlign = HorizontalAlignment.Center;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label16.Location = new Point(221, 9);
            label16.Name = "label16";
            label16.Padding = new Padding(6, 0, 0, 0);
            label16.Size = new Size(97, 15);
            label16.TabIndex = 17;
            label16.Text = "Used Target Life";
            label16.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label15.Location = new Point(56, 9);
            label15.Name = "label15";
            label15.Padding = new Padding(6, 0, 0, 0);
            label15.Size = new Size(65, 15);
            label15.TabIndex = 16;
            label15.Text = "DC Target";
            label15.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.GradientInactiveCaption;
            panel3.BorderStyle = BorderStyle.Fixed3D;
            panel3.Controls.Add(txtDC_ReadPower);
            panel3.Controls.Add(label59);
            panel3.Controls.Add(label58);
            panel3.Controls.Add(label57);
            panel3.Controls.Add(txtDC_ReadVoltage);
            panel3.Controls.Add(txtDC_ReadCurrent);
            panel3.Location = new Point(10, 314);
            panel3.Name = "panel3";
            panel3.Size = new Size(386, 68);
            panel3.TabIndex = 8;
            // 
            // txtDC_ReadPower
            // 
            txtDC_ReadPower.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtDC_ReadPower.Font = new Font("Segoe UI", 9.75F);
            txtDC_ReadPower.Location = new Point(269, 28);
            txtDC_ReadPower.Name = "txtDC_ReadPower";
            txtDC_ReadPower.ReadOnly = true;
            txtDC_ReadPower.Size = new Size(100, 25);
            txtDC_ReadPower.TabIndex = 20;
            txtDC_ReadPower.Text = "0";
            txtDC_ReadPower.TextAlign = HorizontalAlignment.Center;
            // 
            // label59
            // 
            label59.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label59.AutoSize = true;
            label59.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label59.Location = new Point(285, 9);
            label59.Name = "label59";
            label59.Padding = new Padding(6, 0, 0, 0);
            label59.Size = new Size(68, 15);
            label59.TabIndex = 19;
            label59.Text = "Power (W)";
            label59.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label58
            // 
            label58.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label58.AutoSize = true;
            label58.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label58.Location = new Point(155, 9);
            label58.Name = "label58";
            label58.Padding = new Padding(6, 0, 0, 0);
            label58.Size = new Size(70, 15);
            label58.TabIndex = 18;
            label58.Text = "Voltage (V)";
            label58.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label57
            // 
            label57.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label57.AutoSize = true;
            label57.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label57.Location = new Point(20, 9);
            label57.Name = "label57";
            label57.Padding = new Padding(6, 0, 0, 0);
            label57.Size = new Size(83, 15);
            label57.TabIndex = 17;
            label57.Text = "Current (mA)";
            label57.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDC_ReadVoltage
            // 
            txtDC_ReadVoltage.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtDC_ReadVoltage.Font = new Font("Segoe UI", 9.75F);
            txtDC_ReadVoltage.Location = new Point(140, 28);
            txtDC_ReadVoltage.Name = "txtDC_ReadVoltage";
            txtDC_ReadVoltage.ReadOnly = true;
            txtDC_ReadVoltage.Size = new Size(100, 25);
            txtDC_ReadVoltage.TabIndex = 5;
            txtDC_ReadVoltage.Text = "0";
            txtDC_ReadVoltage.TextAlign = HorizontalAlignment.Center;
            // 
            // txtDC_ReadCurrent
            // 
            txtDC_ReadCurrent.Font = new Font("Segoe UI", 9.75F);
            txtDC_ReadCurrent.Location = new Point(11, 28);
            txtDC_ReadCurrent.Name = "txtDC_ReadCurrent";
            txtDC_ReadCurrent.ReadOnly = true;
            txtDC_ReadCurrent.Size = new Size(100, 25);
            txtDC_ReadCurrent.TabIndex = 4;
            txtDC_ReadCurrent.Tag = "";
            txtDC_ReadCurrent.Text = "0";
            txtDC_ReadCurrent.TextAlign = HorizontalAlignment.Center;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(btnControlDC);
            panel2.Location = new Point(10, 16);
            panel2.Name = "panel2";
            panel2.Size = new Size(386, 40);
            panel2.TabIndex = 6;
            // 
            // btnControlDC
            // 
            btnControlDC.AccessibleName = "DC Supply Command";
            btnControlDC.Anchor = AnchorStyles.None;
            btnControlDC.BackColor = Color.IndianRed;
            btnControlDC.FlatStyle = FlatStyle.Flat;
            btnControlDC.Font = new Font("Segoe UI", 10F);
            btnControlDC.ForeColor = Color.White;
            btnControlDC.Location = new Point(4, 4);
            btnControlDC.Name = "btnControlDC";
            btnControlDC.Size = new Size(374, 29);
            btnControlDC.TabIndex = 5;
            btnControlDC.Text = "DC OFF";
            btnControlDC.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.GradientInactiveCaption;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(label27);
            panel1.Controls.Add(label26);
            panel1.Controls.Add(txtDC_SetCurrent);
            panel1.Controls.Add(numDC_OffRamp);
            panel1.Controls.Add(numDc_Step2_Dwell);
            panel1.Controls.Add(numDc_Step2_Ramp);
            panel1.Controls.Add(label13);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(txtDC_SetVoltage);
            panel1.Controls.Add(numDc_Step2_SetW);
            panel1.Controls.Add(lblDc_OffRamp);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(lblRf1_Dwell);
            panel1.Controls.Add(lblRf1_Ramp);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(lblRf1_Set);
            panel1.Controls.Add(txtDc_Step1_SetW);
            panel1.Controls.Add(txtDc_Step1_Ramp);
            panel1.Controls.Add(txtDc_Step1_Dwell);
            panel1.Location = new Point(10, 62);
            panel1.Name = "panel1";
            panel1.Size = new Size(386, 224);
            panel1.TabIndex = 5;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label27.Location = new Point(-2, 33);
            label27.Name = "label27";
            label27.Padding = new Padding(6, 0, 0, 0);
            label27.Size = new Size(96, 17);
            label27.TabIndex = 27;
            label27.Text = "Power Set (W)";
            label27.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label26.Location = new Point(319, 157);
            label26.Name = "label26";
            label26.Padding = new Padding(6, 0, 0, 0);
            label26.Size = new Size(50, 15);
            label26.TabIndex = 26;
            label26.Text = "(0-600)";
            label26.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDC_SetCurrent
            // 
            txtDC_SetCurrent.Location = new Point(217, 153);
            txtDC_SetCurrent.Name = "txtDC_SetCurrent";
            txtDC_SetCurrent.Size = new Size(100, 23);
            txtDC_SetCurrent.TabIndex = 25;
            txtDC_SetCurrent.Text = "50";
            // 
            // numDC_OffRamp
            // 
            numDC_OffRamp.Cursor = Cursors.Hand;
            numDC_OffRamp.Font = new Font("Segoe UI", 10F);
            numDC_OffRamp.ForeColor = SystemColors.WindowText;
            numDC_OffRamp.Location = new Point(155, 190);
            numDC_OffRamp.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numDC_OffRamp.Name = "numDC_OffRamp";
            numDC_OffRamp.Size = new Size(100, 25);
            numDC_OffRamp.TabIndex = 24;
            numDC_OffRamp.TextAlign = HorizontalAlignment.Center;
            numDC_OffRamp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numDc_Step2_Dwell
            // 
            numDc_Step2_Dwell.Cursor = Cursors.Hand;
            numDc_Step2_Dwell.Font = new Font("Segoe UI", 10F);
            numDc_Step2_Dwell.ForeColor = SystemColors.WindowText;
            numDc_Step2_Dwell.Location = new Point(217, 98);
            numDc_Step2_Dwell.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numDc_Step2_Dwell.Name = "numDc_Step2_Dwell";
            numDc_Step2_Dwell.Size = new Size(100, 25);
            numDc_Step2_Dwell.TabIndex = 24;
            numDc_Step2_Dwell.TextAlign = HorizontalAlignment.Center;
            numDc_Step2_Dwell.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numDc_Step2_Ramp
            // 
            numDc_Step2_Ramp.Cursor = Cursors.Hand;
            numDc_Step2_Ramp.Font = new Font("Segoe UI", 10F);
            numDc_Step2_Ramp.ForeColor = SystemColors.WindowText;
            numDc_Step2_Ramp.Location = new Point(217, 63);
            numDc_Step2_Ramp.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numDc_Step2_Ramp.Name = "numDc_Step2_Ramp";
            numDc_Step2_Ramp.Size = new Size(100, 25);
            numDc_Step2_Ramp.TabIndex = 24;
            numDc_Step2_Ramp.TextAlign = HorizontalAlignment.Center;
            numDc_Step2_Ramp.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label13.Location = new Point(39, 157);
            label13.Name = "label13";
            label13.Padding = new Padding(6, 0, 0, 0);
            label13.Size = new Size(50, 15);
            label13.TabIndex = 23;
            label13.Text = "(0-600)";
            label13.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label12.Location = new Point(265, 195);
            label12.Name = "label12";
            label12.Padding = new Padding(6, 0, 0, 0);
            label12.Size = new Size(44, 15);
            label12.TabIndex = 22;
            label12.Text = "(0-10)";
            label12.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label11.Location = new Point(96, 133);
            label11.Name = "label11";
            label11.Padding = new Padding(6, 0, 0, 0);
            label11.Size = new Size(100, 17);
            label11.TabIndex = 21;
            label11.Text = "Voltage Set (V)";
            label11.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDC_SetVoltage
            // 
            txtDC_SetVoltage.Location = new Point(96, 153);
            txtDC_SetVoltage.Name = "txtDC_SetVoltage";
            txtDC_SetVoltage.Size = new Size(100, 23);
            txtDC_SetVoltage.TabIndex = 20;
            txtDC_SetVoltage.Text = "200";
            // 
            // numDc_Step2_SetW
            // 
            numDc_Step2_SetW.Cursor = Cursors.Hand;
            numDc_Step2_SetW.Font = new Font("Segoe UI", 10F);
            numDc_Step2_SetW.ForeColor = SystemColors.WindowText;
            numDc_Step2_SetW.Location = new Point(217, 29);
            numDc_Step2_SetW.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numDc_Step2_SetW.Name = "numDc_Step2_SetW";
            numDc_Step2_SetW.Size = new Size(100, 25);
            numDc_Step2_SetW.TabIndex = 19;
            numDc_Step2_SetW.TextAlign = HorizontalAlignment.Center;
            numDc_Step2_SetW.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // lblDc_OffRamp
            // 
            lblDc_OffRamp.AutoSize = true;
            lblDc_OffRamp.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblDc_OffRamp.Location = new Point(35, 193);
            lblDc_OffRamp.Name = "lblDc_OffRamp";
            lblDc_OffRamp.Padding = new Padding(6, 0, 0, 0);
            lblDc_OffRamp.Size = new Size(112, 17);
            lblDc_OffRamp.TabIndex = 18;
            lblDc_OffRamp.Text = "Off Ramp (mA/s)";
            lblDc_OffRamp.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label9.Location = new Point(319, 101);
            label9.Name = "label9";
            label9.Padding = new Padding(6, 0, 0, 0);
            label9.Size = new Size(50, 15);
            label9.TabIndex = 16;
            label9.Text = "(0-999)";
            label9.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label8.Location = new Point(319, 68);
            label8.Name = "label8";
            label8.Padding = new Padding(6, 0, 0, 0);
            label8.Size = new Size(44, 15);
            label8.TabIndex = 15;
            label8.Text = "(0-10)";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label7.Location = new Point(319, 34);
            label7.Name = "label7";
            label7.Padding = new Padding(6, 0, 0, 0);
            label7.Size = new Size(56, 15);
            label7.TabIndex = 14;
            label7.Text = "(0-2000)";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label6.Location = new Point(239, 7);
            label6.Name = "label6";
            label6.Padding = new Padding(6, 0, 0, 0);
            label6.Size = new Size(52, 17);
            label6.TabIndex = 13;
            label6.Text = "Step 2";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRf1_Dwell
            // 
            lblRf1_Dwell.AutoSize = true;
            lblRf1_Dwell.Font = new Font("Segoe UI", 9F);
            lblRf1_Dwell.Location = new Point(13, 101);
            lblRf1_Dwell.Name = "lblRf1_Dwell";
            lblRf1_Dwell.Padding = new Padding(6, 0, 0, 0);
            lblRf1_Dwell.Size = new Size(74, 15);
            lblRf1_Dwell.TabIndex = 9;
            lblRf1_Dwell.Text = "Dwell (min)";
            lblRf1_Dwell.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblRf1_Ramp
            // 
            lblRf1_Ramp.AutoSize = true;
            lblRf1_Ramp.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblRf1_Ramp.Location = new Point(2, 67);
            lblRf1_Ramp.Name = "lblRf1_Ramp";
            lblRf1_Ramp.Padding = new Padding(6, 0, 0, 0);
            lblRf1_Ramp.Size = new Size(83, 17);
            lblRf1_Ramp.TabIndex = 8;
            lblRf1_Ramp.Text = "Ramp (W/s)";
            lblRf1_Ramp.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label5.Location = new Point(119, 7);
            label5.Name = "label5";
            label5.Padding = new Padding(6, 0, 0, 0);
            label5.Size = new Size(50, 17);
            label5.TabIndex = 7;
            label5.Text = "Step 1";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRf1_Set
            // 
            lblRf1_Set.AutoSize = true;
            lblRf1_Set.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblRf1_Set.Location = new Point(236, 133);
            lblRf1_Set.Name = "lblRf1_Set";
            lblRf1_Set.Padding = new Padding(6, 0, 0, 0);
            lblRf1_Set.Size = new Size(63, 17);
            lblRf1_Set.TabIndex = 6;
            lblRf1_Set.Text = "Set (mA)";
            lblRf1_Set.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDc_Step1_SetW
            // 
            txtDc_Step1_SetW.Location = new Point(96, 30);
            txtDc_Step1_SetW.Name = "txtDc_Step1_SetW";
            txtDc_Step1_SetW.Size = new Size(100, 23);
            txtDc_Step1_SetW.TabIndex = 0;
            txtDc_Step1_SetW.Text = "10";
            // 
            // txtDc_Step1_Ramp
            // 
            txtDc_Step1_Ramp.Location = new Point(96, 64);
            txtDc_Step1_Ramp.Name = "txtDc_Step1_Ramp";
            txtDc_Step1_Ramp.Size = new Size(100, 23);
            txtDc_Step1_Ramp.TabIndex = 1;
            txtDc_Step1_Ramp.Text = "1";
            // 
            // txtDc_Step1_Dwell
            // 
            txtDc_Step1_Dwell.Location = new Point(96, 98);
            txtDc_Step1_Dwell.Name = "txtDc_Step1_Dwell";
            txtDc_Step1_Dwell.Size = new Size(100, 23);
            txtDc_Step1_Dwell.TabIndex = 3;
            txtDc_Step1_Dwell.Text = "1";
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.GradientInactiveCaption;
            panel4.BorderStyle = BorderStyle.Fixed3D;
            panel4.Controls.Add(txtDC_Step2_rTime);
            panel4.Controls.Add(txtDC_Step1_rTime);
            panel4.Controls.Add(label19);
            panel4.Controls.Add(label18);
            panel4.Location = new Point(10, 506);
            panel4.Name = "panel4";
            panel4.Size = new Size(386, 72);
            panel4.TabIndex = 10;
            // 
            // txtDC_Step2_rTime
            // 
            txtDC_Step2_rTime.Font = new Font("Segoe UI", 9.75F);
            txtDC_Step2_rTime.Location = new Point(213, 35);
            txtDC_Step2_rTime.Name = "txtDC_Step2_rTime";
            txtDC_Step2_rTime.ReadOnly = true;
            txtDC_Step2_rTime.Size = new Size(113, 25);
            txtDC_Step2_rTime.TabIndex = 20;
            txtDC_Step2_rTime.Text = "00:00:00";
            txtDC_Step2_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // txtDC_Step1_rTime
            // 
            txtDC_Step1_rTime.Font = new Font("Segoe UI", 9.75F);
            txtDC_Step1_rTime.Location = new Point(34, 35);
            txtDC_Step1_rTime.Name = "txtDC_Step1_rTime";
            txtDC_Step1_rTime.ReadOnly = true;
            txtDC_Step1_rTime.Size = new Size(113, 25);
            txtDC_Step1_rTime.TabIndex = 19;
            txtDC_Step1_rTime.Text = "00:00:00";
            txtDC_Step1_rTime.TextAlign = HorizontalAlignment.Center;
            // 
            // label19
            // 
            label19.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label19.Location = new Point(204, 13);
            label19.Name = "label19";
            label19.Padding = new Padding(6, 0, 0, 0);
            label19.Size = new Size(130, 15);
            label19.TabIndex = 18;
            label19.Text = "Step-2 Remainig Time";
            label19.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            label18.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label18.Location = new Point(25, 13);
            label18.Name = "label18";
            label18.Padding = new Padding(6, 0, 0, 0);
            label18.Size = new Size(130, 15);
            label18.TabIndex = 17;
            label18.Text = "Step-1 Remainig Time";
            label18.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpStartControls
            // 
            grpStartControls.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpStartControls.BackColor = Color.LightSteelBlue;
            grpStartControls.Controls.Add(tlpAnalogFeedback);
            grpStartControls.Controls.Add(tlpStartControls);
            grpStartControls.Controls.Add(panel16);
            grpStartControls.Location = new Point(26, 6);
            grpStartControls.Name = "grpStartControls";
            grpStartControls.Size = new Size(393, 756);
            grpStartControls.TabIndex = 16;
            grpStartControls.TabStop = false;
            grpStartControls.Text = "Manuel Start Controls";
            // 
            // tlpAnalogFeedback
            // 
            tlpAnalogFeedback.BackColor = SystemColors.GradientInactiveCaption;
            tlpAnalogFeedback.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tlpAnalogFeedback.ColumnCount = 3;
            tlpAnalogFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36.69724F));
            tlpAnalogFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43.1732368F));
            tlpAnalogFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.12952F));
            tlpAnalogFeedback.Controls.Add(label80, 2, 2);
            tlpAnalogFeedback.Controls.Add(label79, 0, 2);
            tlpAnalogFeedback.Controls.Add(label35, 2, 3);
            tlpAnalogFeedback.Controls.Add(lblTurboFrequency, 1, 3);
            tlpAnalogFeedback.Controls.Add(label32, 0, 3);
            tlpAnalogFeedback.Controls.Add(lblRotationMotor, 0, 0);
            tlpAnalogFeedback.Controls.Add(label10, 2, 1);
            tlpAnalogFeedback.Controls.Add(numGateValvePosition, 1, 1);
            tlpAnalogFeedback.Controls.Add(label2, 0, 1);
            tlpAnalogFeedback.Controls.Add(btnRotationMotor, 2, 0);
            tlpAnalogFeedback.Controls.Add(numMotorRpm, 1, 0);
            tlpAnalogFeedback.Controls.Add(tableLayoutPanel3, 1, 2);
            tlpAnalogFeedback.Location = new Point(12, 314);
            tlpAnalogFeedback.Name = "tlpAnalogFeedback";
            tlpAnalogFeedback.RowCount = 4;
            tlpAnalogFeedback.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpAnalogFeedback.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpAnalogFeedback.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpAnalogFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpAnalogFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpAnalogFeedback.Size = new Size(369, 228);
            tlpAnalogFeedback.TabIndex = 2;
            // 
            // label80
            // 
            label80.AutoSize = true;
            label80.Dock = DockStyle.Fill;
            label80.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label80.Location = new Point(296, 100);
            label80.Name = "label80";
            label80.Padding = new Padding(6, 0, 0, 0);
            label80.Size = new Size(68, 94);
            label80.TabIndex = 55;
            label80.Text = "°C";
            label80.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label79
            // 
            label79.AutoSize = true;
            label79.Dock = DockStyle.Fill;
            label79.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label79.Location = new Point(5, 100);
            label79.Name = "label79";
            label79.Padding = new Padding(6, 0, 0, 0);
            label79.Size = new Size(126, 94);
            label79.TabIndex = 53;
            label79.Tag = "";
            label79.Text = "Rezistance Control";
            label79.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Dock = DockStyle.Fill;
            label35.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label35.Location = new Point(296, 196);
            label35.Name = "label35";
            label35.Padding = new Padding(6, 0, 0, 0);
            label35.Size = new Size(68, 30);
            label35.TabIndex = 52;
            label35.Text = "Hz";
            label35.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTurboFrequency
            // 
            lblTurboFrequency.Dock = DockStyle.Fill;
            lblTurboFrequency.Location = new Point(139, 196);
            lblTurboFrequency.Name = "lblTurboFrequency";
            lblTurboFrequency.Size = new Size(149, 30);
            lblTurboFrequency.TabIndex = 51;
            lblTurboFrequency.Text = "--";
            lblTurboFrequency.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Dock = DockStyle.Fill;
            label32.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label32.Location = new Point(5, 196);
            label32.Name = "label32";
            label32.Padding = new Padding(6, 0, 0, 0);
            label32.Size = new Size(126, 30);
            label32.TabIndex = 50;
            label32.Tag = "";
            label32.Text = "Turbo Frequency";
            label32.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblRotationMotor
            // 
            lblRotationMotor.AutoSize = true;
            lblRotationMotor.Dock = DockStyle.Fill;
            lblRotationMotor.Font = new Font("Segoe UI", 9.75F);
            lblRotationMotor.Location = new Point(5, 2);
            lblRotationMotor.Name = "lblRotationMotor";
            lblRotationMotor.Padding = new Padding(6, 0, 0, 0);
            lblRotationMotor.Size = new Size(126, 47);
            lblRotationMotor.TabIndex = 49;
            lblRotationMotor.Text = "Rotation Motor";
            lblRotationMotor.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Fill;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label10.Location = new Point(296, 51);
            label10.Name = "label10";
            label10.Padding = new Padding(6, 0, 0, 0);
            label10.Size = new Size(68, 47);
            label10.TabIndex = 45;
            label10.Text = "(0-100)";
            label10.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numGateValvePosition
            // 
            numGateValvePosition.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numGateValvePosition.Cursor = Cursors.Hand;
            numGateValvePosition.Font = new Font("Segoe UI", 10F);
            numGateValvePosition.ForeColor = SystemColors.WindowText;
            numGateValvePosition.Location = new Point(139, 62);
            numGateValvePosition.Name = "numGateValvePosition";
            numGateValvePosition.Size = new Size(149, 25);
            numGateValvePosition.TabIndex = 44;
            numGateValvePosition.TextAlign = HorizontalAlignment.Center;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label2.Location = new Point(5, 51);
            label2.Name = "label2";
            label2.Padding = new Padding(6, 0, 0, 0);
            label2.Size = new Size(126, 47);
            label2.TabIndex = 43;
            label2.Tag = "";
            label2.Text = "Gate Valve Position";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRotationMotor
            // 
            btnRotationMotor.AccessibleName = "Rotation Motor";
            btnRotationMotor.BackColor = Color.IndianRed;
            btnRotationMotor.Dock = DockStyle.Fill;
            btnRotationMotor.FlatStyle = FlatStyle.Flat;
            btnRotationMotor.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRotationMotor.ForeColor = Color.White;
            btnRotationMotor.Location = new Point(296, 5);
            btnRotationMotor.Name = "btnRotationMotor";
            btnRotationMotor.Size = new Size(68, 41);
            btnRotationMotor.TabIndex = 42;
            btnRotationMotor.Tag = "";
            btnRotationMotor.Text = "STOP";
            btnRotationMotor.UseVisualStyleBackColor = false;
            // 
            // numMotorRpm
            // 
            numMotorRpm.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numMotorRpm.Cursor = Cursors.Hand;
            numMotorRpm.Font = new Font("Segoe UI", 10F);
            numMotorRpm.ForeColor = SystemColors.WindowText;
            numMotorRpm.Location = new Point(139, 13);
            numMotorRpm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numMotorRpm.Name = "numMotorRpm";
            numMotorRpm.Size = new Size(149, 25);
            numMotorRpm.TabIndex = 41;
            numMotorRpm.TextAlign = HorizontalAlignment.Center;
            numMotorRpm.Value = new decimal(new int[] { 1, 0, 0, 65536 });
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(label82, 0, 1);
            tableLayoutPanel3.Controls.Add(numRezistanceControl, 1, 0);
            tableLayoutPanel3.Controls.Add(rezistanceReadValue, 1, 1);
            tableLayoutPanel3.Controls.Add(btnSetRezistanceVal, 0, 0);
            tableLayoutPanel3.Location = new Point(139, 110);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(149, 73);
            tableLayoutPanel3.TabIndex = 56;
            // 
            // label82
            // 
            label82.AutoSize = true;
            label82.Dock = DockStyle.Fill;
            label82.Location = new Point(4, 37);
            label82.Name = "label82";
            label82.Size = new Size(49, 35);
            label82.TabIndex = 58;
            label82.Text = "Read";
            label82.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numRezistanceControl
            // 
            numRezistanceControl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numRezistanceControl.Cursor = Cursors.Hand;
            numRezistanceControl.DecimalPlaces = 1;
            numRezistanceControl.Font = new Font("Segoe UI", 10F);
            numRezistanceControl.ForeColor = SystemColors.WindowText;
            numRezistanceControl.Location = new Point(60, 6);
            numRezistanceControl.Maximum = new decimal(new int[] { 900, 0, 0, 0 });
            numRezistanceControl.Name = "numRezistanceControl";
            numRezistanceControl.Size = new Size(85, 25);
            numRezistanceControl.TabIndex = 55;
            numRezistanceControl.TextAlign = HorizontalAlignment.Center;
            // 
            // rezistanceReadValue
            // 
            rezistanceReadValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            rezistanceReadValue.Font = new Font("Segoe UI", 9.75F);
            rezistanceReadValue.Location = new Point(60, 42);
            rezistanceReadValue.Name = "rezistanceReadValue";
            rezistanceReadValue.ReadOnly = true;
            rezistanceReadValue.Size = new Size(85, 25);
            rezistanceReadValue.TabIndex = 56;
            rezistanceReadValue.Tag = "";
            rezistanceReadValue.Text = "0";
            rezistanceReadValue.TextAlign = HorizontalAlignment.Center;
            // 
            // btnSetRezistanceVal
            // 
            btnSetRezistanceVal.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnSetRezistanceVal.Image = Properties.Resources.verified16;
            btnSetRezistanceVal.ImageAlign = ContentAlignment.MiddleLeft;
            btnSetRezistanceVal.Location = new Point(4, 5);
            btnSetRezistanceVal.Name = "btnSetRezistanceVal";
            btnSetRezistanceVal.Size = new Size(49, 27);
            btnSetRezistanceVal.TabIndex = 59;
            btnSetRezistanceVal.Text = "Set";
            btnSetRezistanceVal.TextAlign = ContentAlignment.MiddleRight;
            btnSetRezistanceVal.UseVisualStyleBackColor = true;
            // 
            // tlpStartControls
            // 
            tlpStartControls.BackColor = SystemColors.GradientInactiveCaption;
            tlpStartControls.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tlpStartControls.ColumnCount = 4;
            tlpStartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27F));
            tlpStartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            tlpStartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27F));
            tlpStartControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            tlpStartControls.Controls.Add(btnRezistans, 3, 5);
            tlpStartControls.Controls.Add(lblRezistans, 2, 5);
            tlpStartControls.Controls.Add(btnMatchBoxPower, 1, 5);
            tlpStartControls.Controls.Add(lblGUC4, 0, 5);
            tlpStartControls.Controls.Add(btnDCShutter, 3, 4);
            tlpStartControls.Controls.Add(lblLeftFrontShutter, 2, 4);
            tlpStartControls.Controls.Add(btnDCPower, 1, 4);
            tlpStartControls.Controls.Add(lblGUC3, 0, 4);
            tlpStartControls.Controls.Add(btnRF2Power, 1, 3);
            tlpStartControls.Controls.Add(lblGUC2, 0, 3);
            tlpStartControls.Controls.Add(btnRF2Shutter, 3, 3);
            tlpStartControls.Controls.Add(lblRightRearShutter, 2, 3);
            tlpStartControls.Controls.Add(btnRF1Shutter, 3, 2);
            tlpStartControls.Controls.Add(lblRightFrontShutter, 2, 2);
            tlpStartControls.Controls.Add(btnRF1Power, 1, 2);
            tlpStartControls.Controls.Add(lblGUC1, 0, 2);
            tlpStartControls.Controls.Add(btnChamber, 3, 1);
            tlpStartControls.Controls.Add(lblChamberValve, 2, 1);
            tlpStartControls.Controls.Add(btnTurboVacuum, 1, 1);
            tlpStartControls.Controls.Add(lblTurbo_EVP_Vacuum, 0, 1);
            tlpStartControls.Controls.Add(btnChiller, 3, 0);
            tlpStartControls.Controls.Add(lblChiller, 2, 0);
            tlpStartControls.Controls.Add(btnScrollVacuum, 1, 0);
            tlpStartControls.Controls.Add(lblScrewVacuum, 0, 0);
            tlpStartControls.Dock = DockStyle.Top;
            tlpStartControls.Location = new Point(3, 19);
            tlpStartControls.Name = "tlpStartControls";
            tlpStartControls.RowCount = 6;
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tlpStartControls.Size = new Size(387, 267);
            tlpStartControls.TabIndex = 0;
            // 
            // btnRezistans
            // 
            btnRezistans.AccessibleName = "Rezistance";
            btnRezistans.BackColor = Color.IndianRed;
            btnRezistans.Dock = DockStyle.Fill;
            btnRezistans.FlatStyle = FlatStyle.Flat;
            btnRezistans.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRezistans.ForeColor = Color.White;
            btnRezistans.Location = new Point(299, 225);
            btnRezistans.Name = "btnRezistans";
            btnRezistans.Size = new Size(83, 37);
            btnRezistans.TabIndex = 89;
            btnRezistans.Tag = "";
            btnRezistans.Text = "STOP";
            btnRezistans.UseVisualStyleBackColor = false;
            btnRezistans.Visible = false;
            // 
            // lblRezistans
            // 
            lblRezistans.AutoSize = true;
            lblRezistans.Dock = DockStyle.Fill;
            lblRezistans.Font = new Font("Segoe UI", 9.75F);
            lblRezistans.Location = new Point(196, 222);
            lblRezistans.Name = "lblRezistans";
            lblRezistans.Padding = new Padding(6, 0, 0, 0);
            lblRezistans.Size = new Size(95, 43);
            lblRezistans.TabIndex = 88;
            lblRezistans.Text = "Rezistance";
            lblRezistans.TextAlign = ContentAlignment.MiddleLeft;
            lblRezistans.Visible = false;
            // 
            // btnMatchBoxPower
            // 
            btnMatchBoxPower.AccessibleName = "Match Box Power";
            btnMatchBoxPower.BackColor = Color.IndianRed;
            btnMatchBoxPower.Dock = DockStyle.Fill;
            btnMatchBoxPower.FlatStyle = FlatStyle.Flat;
            btnMatchBoxPower.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnMatchBoxPower.ForeColor = Color.White;
            btnMatchBoxPower.Location = new Point(108, 225);
            btnMatchBoxPower.Name = "btnMatchBoxPower";
            btnMatchBoxPower.Size = new Size(80, 37);
            btnMatchBoxPower.TabIndex = 87;
            btnMatchBoxPower.Tag = "";
            btnMatchBoxPower.Text = "STOP";
            btnMatchBoxPower.UseVisualStyleBackColor = false;
            // 
            // lblGUC4
            // 
            lblGUC4.AutoSize = true;
            lblGUC4.Dock = DockStyle.Fill;
            lblGUC4.Font = new Font("Segoe UI", 9.75F);
            lblGUC4.Location = new Point(5, 222);
            lblGUC4.Name = "lblGUC4";
            lblGUC4.Padding = new Padding(6, 0, 0, 0);
            lblGUC4.Size = new Size(95, 43);
            lblGUC4.TabIndex = 86;
            lblGUC4.Text = "Match Box Power";
            lblGUC4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnDCShutter
            // 
            btnDCShutter.AccessibleName = "DC Shutter";
            btnDCShutter.BackColor = Color.IndianRed;
            btnDCShutter.Dock = DockStyle.Fill;
            btnDCShutter.FlatStyle = FlatStyle.Flat;
            btnDCShutter.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnDCShutter.ForeColor = Color.White;
            btnDCShutter.Location = new Point(299, 181);
            btnDCShutter.Name = "btnDCShutter";
            btnDCShutter.Size = new Size(83, 36);
            btnDCShutter.TabIndex = 85;
            btnDCShutter.Tag = "";
            btnDCShutter.Text = "STOP";
            btnDCShutter.UseVisualStyleBackColor = false;
            // 
            // lblLeftFrontShutter
            // 
            lblLeftFrontShutter.AutoSize = true;
            lblLeftFrontShutter.Dock = DockStyle.Fill;
            lblLeftFrontShutter.Font = new Font("Segoe UI", 9.75F);
            lblLeftFrontShutter.Location = new Point(196, 178);
            lblLeftFrontShutter.Name = "lblLeftFrontShutter";
            lblLeftFrontShutter.Padding = new Padding(6, 0, 0, 0);
            lblLeftFrontShutter.Size = new Size(95, 42);
            lblLeftFrontShutter.TabIndex = 84;
            lblLeftFrontShutter.Text = "DC Shutter";
            lblLeftFrontShutter.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnDCPower
            // 
            btnDCPower.AccessibleName = "DC Power";
            btnDCPower.BackColor = Color.IndianRed;
            btnDCPower.Dock = DockStyle.Fill;
            btnDCPower.FlatStyle = FlatStyle.Flat;
            btnDCPower.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnDCPower.ForeColor = Color.White;
            btnDCPower.Location = new Point(108, 181);
            btnDCPower.Name = "btnDCPower";
            btnDCPower.Size = new Size(80, 36);
            btnDCPower.TabIndex = 83;
            btnDCPower.Tag = "";
            btnDCPower.Text = "STOP";
            btnDCPower.UseVisualStyleBackColor = false;
            // 
            // lblGUC3
            // 
            lblGUC3.AutoSize = true;
            lblGUC3.Dock = DockStyle.Fill;
            lblGUC3.Font = new Font("Segoe UI", 9.75F);
            lblGUC3.Location = new Point(5, 178);
            lblGUC3.Name = "lblGUC3";
            lblGUC3.Padding = new Padding(6, 0, 0, 0);
            lblGUC3.Size = new Size(95, 42);
            lblGUC3.TabIndex = 82;
            lblGUC3.Text = "DC Power";
            lblGUC3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRF2Power
            // 
            btnRF2Power.AccessibleName = "RF2 Power";
            btnRF2Power.BackColor = Color.IndianRed;
            btnRF2Power.Dock = DockStyle.Fill;
            btnRF2Power.FlatStyle = FlatStyle.Flat;
            btnRF2Power.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRF2Power.ForeColor = Color.White;
            btnRF2Power.Location = new Point(108, 137);
            btnRF2Power.Name = "btnRF2Power";
            btnRF2Power.Size = new Size(80, 36);
            btnRF2Power.TabIndex = 81;
            btnRF2Power.Tag = "";
            btnRF2Power.Text = "STOP";
            btnRF2Power.UseVisualStyleBackColor = false;
            // 
            // lblGUC2
            // 
            lblGUC2.AutoSize = true;
            lblGUC2.Dock = DockStyle.Fill;
            lblGUC2.Font = new Font("Segoe UI", 9.75F);
            lblGUC2.Location = new Point(5, 134);
            lblGUC2.Name = "lblGUC2";
            lblGUC2.Padding = new Padding(6, 0, 0, 0);
            lblGUC2.Size = new Size(95, 42);
            lblGUC2.TabIndex = 80;
            lblGUC2.Text = "RF2 Power";
            lblGUC2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRF2Shutter
            // 
            btnRF2Shutter.AccessibleName = "RF2 Shutter";
            btnRF2Shutter.BackColor = Color.IndianRed;
            btnRF2Shutter.Dock = DockStyle.Fill;
            btnRF2Shutter.FlatStyle = FlatStyle.Flat;
            btnRF2Shutter.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRF2Shutter.ForeColor = Color.White;
            btnRF2Shutter.Location = new Point(299, 137);
            btnRF2Shutter.Name = "btnRF2Shutter";
            btnRF2Shutter.Size = new Size(83, 36);
            btnRF2Shutter.TabIndex = 79;
            btnRF2Shutter.Tag = "";
            btnRF2Shutter.Text = "STOP";
            btnRF2Shutter.UseVisualStyleBackColor = false;
            // 
            // lblRightRearShutter
            // 
            lblRightRearShutter.AutoSize = true;
            lblRightRearShutter.Dock = DockStyle.Fill;
            lblRightRearShutter.Font = new Font("Segoe UI", 9.75F);
            lblRightRearShutter.Location = new Point(196, 134);
            lblRightRearShutter.Name = "lblRightRearShutter";
            lblRightRearShutter.Padding = new Padding(6, 0, 0, 0);
            lblRightRearShutter.Size = new Size(95, 42);
            lblRightRearShutter.TabIndex = 78;
            lblRightRearShutter.Text = "RF2 Shutter";
            lblRightRearShutter.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRF1Shutter
            // 
            btnRF1Shutter.AccessibleName = "RF1 Shutter";
            btnRF1Shutter.BackColor = Color.IndianRed;
            btnRF1Shutter.Dock = DockStyle.Fill;
            btnRF1Shutter.FlatStyle = FlatStyle.Flat;
            btnRF1Shutter.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRF1Shutter.ForeColor = Color.White;
            btnRF1Shutter.Location = new Point(299, 93);
            btnRF1Shutter.Name = "btnRF1Shutter";
            btnRF1Shutter.Size = new Size(83, 36);
            btnRF1Shutter.TabIndex = 76;
            btnRF1Shutter.Tag = "";
            btnRF1Shutter.Text = "STOP";
            btnRF1Shutter.UseVisualStyleBackColor = false;
            // 
            // lblRightFrontShutter
            // 
            lblRightFrontShutter.AutoSize = true;
            lblRightFrontShutter.Dock = DockStyle.Fill;
            lblRightFrontShutter.Font = new Font("Segoe UI", 9.75F);
            lblRightFrontShutter.Location = new Point(196, 90);
            lblRightFrontShutter.Name = "lblRightFrontShutter";
            lblRightFrontShutter.Padding = new Padding(6, 0, 0, 0);
            lblRightFrontShutter.Size = new Size(95, 42);
            lblRightFrontShutter.TabIndex = 75;
            lblRightFrontShutter.Text = "RF1 Shutter";
            lblRightFrontShutter.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRF1Power
            // 
            btnRF1Power.AccessibleName = "RF1 Power";
            btnRF1Power.BackColor = Color.IndianRed;
            btnRF1Power.Dock = DockStyle.Fill;
            btnRF1Power.FlatStyle = FlatStyle.Flat;
            btnRF1Power.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnRF1Power.ForeColor = Color.White;
            btnRF1Power.Location = new Point(108, 93);
            btnRF1Power.Name = "btnRF1Power";
            btnRF1Power.Size = new Size(80, 36);
            btnRF1Power.TabIndex = 74;
            btnRF1Power.Tag = "";
            btnRF1Power.Text = "STOP";
            btnRF1Power.UseVisualStyleBackColor = false;
            // 
            // lblGUC1
            // 
            lblGUC1.AutoSize = true;
            lblGUC1.Dock = DockStyle.Fill;
            lblGUC1.Font = new Font("Segoe UI", 9.75F);
            lblGUC1.Location = new Point(5, 90);
            lblGUC1.Name = "lblGUC1";
            lblGUC1.Padding = new Padding(6, 0, 0, 0);
            lblGUC1.Size = new Size(95, 42);
            lblGUC1.TabIndex = 73;
            lblGUC1.Text = "RF1 Power";
            lblGUC1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnChamber
            // 
            btnChamber.AccessibleName = "Chamber";
            btnChamber.BackColor = Color.IndianRed;
            btnChamber.Dock = DockStyle.Fill;
            btnChamber.FlatStyle = FlatStyle.Flat;
            btnChamber.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnChamber.ForeColor = Color.White;
            btnChamber.Location = new Point(299, 49);
            btnChamber.Name = "btnChamber";
            btnChamber.Size = new Size(83, 36);
            btnChamber.TabIndex = 72;
            btnChamber.Tag = "Chamber Vent";
            btnChamber.Text = "STOP";
            btnChamber.UseVisualStyleBackColor = false;
            // 
            // lblChamberValve
            // 
            lblChamberValve.AutoSize = true;
            lblChamberValve.Dock = DockStyle.Fill;
            lblChamberValve.Font = new Font("Segoe UI", 9.75F);
            lblChamberValve.Location = new Point(196, 46);
            lblChamberValve.Name = "lblChamberValve";
            lblChamberValve.Padding = new Padding(6, 0, 0, 0);
            lblChamberValve.Size = new Size(95, 42);
            lblChamberValve.TabIndex = 71;
            lblChamberValve.Text = "Chamber Vent";
            lblChamberValve.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnTurboVacuum
            // 
            btnTurboVacuum.AccessibleName = "Turbo EVP";
            btnTurboVacuum.BackColor = Color.IndianRed;
            btnTurboVacuum.Dock = DockStyle.Fill;
            btnTurboVacuum.FlatStyle = FlatStyle.Flat;
            btnTurboVacuum.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnTurboVacuum.ForeColor = Color.White;
            btnTurboVacuum.Location = new Point(108, 49);
            btnTurboVacuum.Name = "btnTurboVacuum";
            btnTurboVacuum.Size = new Size(80, 36);
            btnTurboVacuum.TabIndex = 70;
            btnTurboVacuum.Tag = "Turbo Vacuum";
            btnTurboVacuum.Text = "STOP";
            btnTurboVacuum.UseVisualStyleBackColor = false;
            // 
            // lblTurbo_EVP_Vacuum
            // 
            lblTurbo_EVP_Vacuum.AutoSize = true;
            lblTurbo_EVP_Vacuum.Dock = DockStyle.Fill;
            lblTurbo_EVP_Vacuum.Font = new Font("Segoe UI", 9.75F);
            lblTurbo_EVP_Vacuum.Location = new Point(5, 46);
            lblTurbo_EVP_Vacuum.Name = "lblTurbo_EVP_Vacuum";
            lblTurbo_EVP_Vacuum.Padding = new Padding(6, 0, 0, 0);
            lblTurbo_EVP_Vacuum.Size = new Size(95, 42);
            lblTurbo_EVP_Vacuum.TabIndex = 69;
            lblTurbo_EVP_Vacuum.Text = "Turbo/EVP Vacuum";
            lblTurbo_EVP_Vacuum.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnChiller
            // 
            btnChiller.AccessibleName = "Chiller";
            btnChiller.BackColor = Color.IndianRed;
            btnChiller.Dock = DockStyle.Fill;
            btnChiller.FlatStyle = FlatStyle.Flat;
            btnChiller.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnChiller.ForeColor = Color.White;
            btnChiller.Location = new Point(299, 5);
            btnChiller.Name = "btnChiller";
            btnChiller.Size = new Size(83, 36);
            btnChiller.TabIndex = 68;
            btnChiller.Tag = "Chiller";
            btnChiller.Text = "STOP";
            btnChiller.UseVisualStyleBackColor = false;
            // 
            // lblChiller
            // 
            lblChiller.AutoSize = true;
            lblChiller.Dock = DockStyle.Fill;
            lblChiller.Font = new Font("Segoe UI", 9.75F);
            lblChiller.Location = new Point(196, 2);
            lblChiller.Name = "lblChiller";
            lblChiller.Padding = new Padding(6, 0, 0, 0);
            lblChiller.Size = new Size(95, 42);
            lblChiller.TabIndex = 67;
            lblChiller.Text = "Chiller";
            lblChiller.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnScrollVacuum
            // 
            btnScrollVacuum.AccessibleName = "Scroll Vacuum";
            btnScrollVacuum.BackColor = Color.IndianRed;
            btnScrollVacuum.Dock = DockStyle.Fill;
            btnScrollVacuum.FlatStyle = FlatStyle.Flat;
            btnScrollVacuum.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnScrollVacuum.ForeColor = Color.White;
            btnScrollVacuum.Location = new Point(108, 5);
            btnScrollVacuum.Name = "btnScrollVacuum";
            btnScrollVacuum.Size = new Size(80, 36);
            btnScrollVacuum.TabIndex = 65;
            btnScrollVacuum.Tag = "Scroll Vacuum";
            btnScrollVacuum.Text = "STOP";
            btnScrollVacuum.UseVisualStyleBackColor = false;
            // 
            // lblScrewVacuum
            // 
            lblScrewVacuum.AutoSize = true;
            lblScrewVacuum.Dock = DockStyle.Fill;
            lblScrewVacuum.Font = new Font("Segoe UI", 9.75F);
            lblScrewVacuum.Location = new Point(5, 2);
            lblScrewVacuum.Name = "lblScrewVacuum";
            lblScrewVacuum.Padding = new Padding(6, 0, 0, 0);
            lblScrewVacuum.Size = new Size(95, 42);
            lblScrewVacuum.TabIndex = 64;
            lblScrewVacuum.Text = "Scroll Vacuum";
            lblScrewVacuum.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel16
            // 
            panel16.BackColor = SystemColors.GradientInactiveCaption;
            panel16.BorderStyle = BorderStyle.Fixed3D;
            panel16.Controls.Add(label84);
            panel16.Controls.Add(lblTorDegerAux);
            panel16.Controls.Add(label83);
            panel16.Controls.Add(label1);
            panel16.Controls.Add(cmbTorUnit);
            panel16.Controls.Add(lblTorDeger);
            panel16.Controls.Add(lblTor);
            panel16.Controls.Add(panelTorMini);
            panel16.Location = new Point(12, 574);
            panel16.Name = "panel16";
            panel16.Size = new Size(369, 156);
            panel16.TabIndex = 4;
            // 
            // label84
            // 
            label84.AutoSize = true;
            label84.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 162);
            label84.ForeColor = Color.Black;
            label84.Location = new Point(291, 64);
            label84.Name = "label84";
            label84.Padding = new Padding(6, 0, 0, 0);
            label84.Size = new Size(34, 15);
            label84.TabIndex = 30;
            label84.Text = "torr";
            label84.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTorDegerAux
            // 
            lblTorDegerAux.BorderStyle = BorderStyle.FixedSingle;
            lblTorDegerAux.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTorDegerAux.ForeColor = SystemColors.ControlText;
            lblTorDegerAux.Location = new Point(260, 31);
            lblTorDegerAux.Name = "lblTorDegerAux";
            lblTorDegerAux.Size = new Size(97, 33);
            lblTorDegerAux.TabIndex = 29;
            lblTorDegerAux.Text = "--";
            lblTorDegerAux.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label83
            // 
            label83.AutoSize = true;
            label83.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label83.Location = new Point(270, 12);
            label83.Name = "label83";
            label83.Padding = new Padding(6, 0, 0, 0);
            label83.Size = new Size(77, 15);
            label83.TabIndex = 28;
            label83.Text = "Capacitance";
            label83.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label1.Location = new Point(142, 79);
            label1.Name = "label1";
            label1.Padding = new Padding(6, 0, 0, 0);
            label1.Size = new Size(75, 15);
            label1.TabIndex = 8;
            label1.Text = "Mini Graph ";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbTorUnit
            // 
            cmbTorUnit.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbTorUnit.FlatStyle = FlatStyle.Flat;
            cmbTorUnit.FormattingEnabled = true;
            cmbTorUnit.Items.AddRange(new object[] { "Torr", "mBar", "Pascal" });
            cmbTorUnit.Location = new Point(166, 37);
            cmbTorUnit.Name = "cmbTorUnit";
            cmbTorUnit.Size = new Size(63, 23);
            cmbTorUnit.TabIndex = 27;
            // 
            // lblTorDeger
            // 
            lblTorDeger.BorderStyle = BorderStyle.FixedSingle;
            lblTorDeger.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblTorDeger.Location = new Point(11, 31);
            lblTorDeger.Name = "lblTorDeger";
            lblTorDeger.Size = new Size(149, 33);
            lblTorDeger.TabIndex = 10;
            lblTorDeger.Text = "--";
            lblTorDeger.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTor
            // 
            lblTor.AutoSize = true;
            lblTor.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold | FontStyle.Underline);
            lblTor.Location = new Point(37, 3);
            lblTor.Name = "lblTor";
            lblTor.Padding = new Padding(6, 0, 0, 0);
            lblTor.Size = new Size(94, 19);
            lblTor.TabIndex = 7;
            lblTor.Text = "Vacuum Info";
            lblTor.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelTorMini
            // 
            panelTorMini.BackColor = SystemColors.Window;
            panelTorMini.BorderStyle = BorderStyle.Fixed3D;
            panelTorMini.Location = new Point(11, 97);
            panelTorMini.Name = "panelTorMini";
            panelTorMini.Size = new Size(342, 46);
            panelTorMini.TabIndex = 3;
            // 
            // tpGraph
            // 
            tpGraph.Controls.Add(btnExport);
            tpGraph.Controls.Add(label77);
            tpGraph.Controls.Add(button1);
            tpGraph.Controls.Add(button2);
            tpGraph.Controls.Add(button3);
            tpGraph.Controls.Add(lblThicknessFrozen);
            tpGraph.Controls.Add(btnThicknessFreeze);
            tpGraph.Controls.Add(btnThicknessAll);
            tpGraph.Controls.Add(btnThicknessLive);
            tpGraph.Controls.Add(lblRateFrozen);
            tpGraph.Controls.Add(btnRateFreeze);
            tpGraph.Controls.Add(btnRateAll);
            tpGraph.Controls.Add(btnRateLive);
            tpGraph.Controls.Add(chkTorLog);
            tpGraph.Controls.Add(lblTorFrozen);
            tpGraph.Controls.Add(btnTorFreeze);
            tpGraph.Controls.Add(btnTorAll);
            tpGraph.Controls.Add(btnTorLive);
            tpGraph.Controls.Add(label76);
            tpGraph.Controls.Add(panel19);
            tpGraph.Controls.Add(label68);
            tpGraph.Controls.Add(label64);
            tpGraph.Controls.Add(label63);
            tpGraph.Controls.Add(panelRate);
            tpGraph.Controls.Add(panelThickness);
            tpGraph.Controls.Add(panelTorque);
            tpGraph.Location = new Point(4, 27);
            tpGraph.Name = "tpGraph";
            tpGraph.Padding = new Padding(3);
            tpGraph.Size = new Size(1884, 902);
            tpGraph.TabIndex = 1;
            tpGraph.Text = "Graph";
            tpGraph.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.ForestGreen;
            btnExport.ForeColor = SystemColors.Control;
            btnExport.Image = Properties.Resources.excel;
            btnExport.ImageAlign = ContentAlignment.MiddleLeft;
            btnExport.Location = new Point(1459, 0);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(130, 37);
            btnExport.TabIndex = 23;
            btnExport.Text = "Export All Data";
            btnExport.TextAlign = ContentAlignment.MiddleRight;
            btnExport.UseVisualStyleBackColor = false;
            // 
            // label77
            // 
            label77.AutoSize = true;
            label77.Location = new Point(1187, 385);
            label77.Name = "label77";
            label77.Size = new Size(66, 15);
            label77.TabIndex = 22;
            label77.Text = "🔒 FROZEN";
            // 
            // button1
            // 
            button1.Location = new Point(984, 374);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 19;
            button1.Text = "Freeze";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(903, 374);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 20;
            button2.Text = "All";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(822, 374);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 21;
            button3.Text = "Live (2m)";
            button3.UseVisualStyleBackColor = true;
            // 
            // lblThicknessFrozen
            // 
            lblThicknessFrozen.AutoSize = true;
            lblThicknessFrozen.Location = new Point(1187, 26);
            lblThicknessFrozen.Name = "lblThicknessFrozen";
            lblThicknessFrozen.Size = new Size(66, 15);
            lblThicknessFrozen.TabIndex = 18;
            lblThicknessFrozen.Text = "🔒 FROZEN";
            // 
            // btnThicknessFreeze
            // 
            btnThicknessFreeze.Location = new Point(984, 15);
            btnThicknessFreeze.Name = "btnThicknessFreeze";
            btnThicknessFreeze.Size = new Size(75, 23);
            btnThicknessFreeze.TabIndex = 15;
            btnThicknessFreeze.Text = "Freeze";
            btnThicknessFreeze.UseVisualStyleBackColor = true;
            // 
            // btnThicknessAll
            // 
            btnThicknessAll.Location = new Point(903, 15);
            btnThicknessAll.Name = "btnThicknessAll";
            btnThicknessAll.Size = new Size(75, 23);
            btnThicknessAll.TabIndex = 16;
            btnThicknessAll.Text = "All";
            btnThicknessAll.UseVisualStyleBackColor = true;
            // 
            // btnThicknessLive
            // 
            btnThicknessLive.Location = new Point(822, 15);
            btnThicknessLive.Name = "btnThicknessLive";
            btnThicknessLive.Size = new Size(75, 23);
            btnThicknessLive.TabIndex = 17;
            btnThicknessLive.Text = "Live (2m)";
            btnThicknessLive.UseVisualStyleBackColor = true;
            // 
            // lblRateFrozen
            // 
            lblRateFrozen.AutoSize = true;
            lblRateFrozen.Location = new Point(373, 385);
            lblRateFrozen.Name = "lblRateFrozen";
            lblRateFrozen.Size = new Size(66, 15);
            lblRateFrozen.TabIndex = 14;
            lblRateFrozen.Text = "🔒 FROZEN";
            // 
            // btnRateFreeze
            // 
            btnRateFreeze.Location = new Point(170, 374);
            btnRateFreeze.Name = "btnRateFreeze";
            btnRateFreeze.Size = new Size(75, 23);
            btnRateFreeze.TabIndex = 11;
            btnRateFreeze.Text = "Freeze";
            btnRateFreeze.UseVisualStyleBackColor = true;
            // 
            // btnRateAll
            // 
            btnRateAll.Location = new Point(89, 374);
            btnRateAll.Name = "btnRateAll";
            btnRateAll.Size = new Size(75, 23);
            btnRateAll.TabIndex = 12;
            btnRateAll.Text = "All";
            btnRateAll.UseVisualStyleBackColor = true;
            // 
            // btnRateLive
            // 
            btnRateLive.Location = new Point(8, 374);
            btnRateLive.Name = "btnRateLive";
            btnRateLive.Size = new Size(75, 23);
            btnRateLive.TabIndex = 13;
            btnRateLive.Text = "Live (2m)";
            btnRateLive.UseVisualStyleBackColor = true;
            // 
            // chkTorLog
            // 
            chkTorLog.AutoSize = true;
            chkTorLog.Location = new Point(251, 18);
            chkTorLog.Name = "chkTorLog";
            chkTorLog.Size = new Size(46, 19);
            chkTorLog.TabIndex = 10;
            chkTorLog.Text = "Log";
            chkTorLog.UseVisualStyleBackColor = true;
            // 
            // lblTorFrozen
            // 
            lblTorFrozen.AutoSize = true;
            lblTorFrozen.Location = new Point(373, 26);
            lblTorFrozen.Name = "lblTorFrozen";
            lblTorFrozen.Size = new Size(66, 15);
            lblTorFrozen.TabIndex = 9;
            lblTorFrozen.Text = "🔒 FROZEN";
            // 
            // btnTorFreeze
            // 
            btnTorFreeze.Location = new Point(170, 15);
            btnTorFreeze.Name = "btnTorFreeze";
            btnTorFreeze.Size = new Size(75, 23);
            btnTorFreeze.TabIndex = 8;
            btnTorFreeze.Text = "Freeze";
            btnTorFreeze.UseVisualStyleBackColor = true;
            // 
            // btnTorAll
            // 
            btnTorAll.Location = new Point(89, 15);
            btnTorAll.Name = "btnTorAll";
            btnTorAll.Size = new Size(75, 23);
            btnTorAll.TabIndex = 8;
            btnTorAll.Text = "All";
            btnTorAll.UseVisualStyleBackColor = true;
            // 
            // btnTorLive
            // 
            btnTorLive.Location = new Point(8, 15);
            btnTorLive.Name = "btnTorLive";
            btnTorLive.Size = new Size(75, 23);
            btnTorLive.TabIndex = 8;
            btnTorLive.Text = "Live (2m)";
            btnTorLive.UseVisualStyleBackColor = true;
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label76.Location = new Point(1182, 366);
            label76.Name = "label76";
            label76.Size = new Size(96, 19);
            label76.TabIndex = 7;
            label76.Text = "xxxxx - Time";
            // 
            // panel19
            // 
            panel19.Location = new Point(822, 403);
            panel19.Name = "panel19";
            panel19.Size = new Size(800, 300);
            panel19.TabIndex = 6;
            // 
            // label68
            // 
            label68.AutoSize = true;
            label68.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label68.Location = new Point(363, 366);
            label68.Name = "label68";
            label68.Size = new Size(86, 19);
            label68.TabIndex = 5;
            label68.Text = "Rate - Time";
            // 
            // label64
            // 
            label64.AutoSize = true;
            label64.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label64.Location = new Point(1163, 7);
            label64.Name = "label64";
            label64.Size = new Size(119, 19);
            label64.TabIndex = 4;
            label64.Text = "Thickness - Time";
            // 
            // label63
            // 
            label63.AutoSize = true;
            label63.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label63.Location = new Point(306, 5);
            label63.Name = "label63";
            label63.Size = new Size(200, 19);
            label63.TabIndex = 3;
            label63.Text = "Vacuum/Temperature - Time";
            // 
            // panelRate
            // 
            panelRate.Location = new Point(6, 403);
            panelRate.Name = "panelRate";
            panelRate.Size = new Size(800, 300);
            panelRate.TabIndex = 2;
            // 
            // panelThickness
            // 
            panelThickness.Location = new Point(822, 44);
            panelThickness.Name = "panelThickness";
            panelThickness.Size = new Size(800, 300);
            panelThickness.TabIndex = 1;
            // 
            // panelTorque
            // 
            panelTorque.Location = new Point(6, 44);
            panelTorque.Name = "panelTorque";
            panelTorque.Size = new Size(800, 300);
            panelTorque.TabIndex = 0;
            // 
            // tpQCM
            // 
            tpQCM.Controls.Add(label78);
            tpQCM.Controls.Add(tlpCrystalLife1);
            tpQCM.Controls.Add(gbQcmSensorInfo);
            tpQCM.Controls.Add(groupBox4);
            tpQCM.Controls.Add(grpActiveLayerDetail);
            tpQCM.Controls.Add(grpLayerList);
            tpQCM.Controls.Add(grpProcessList);
            tpQCM.Controls.Add(grpMaterialList);
            tpQCM.Location = new Point(4, 27);
            tpQCM.Name = "tpQCM";
            tpQCM.Size = new Size(1884, 902);
            tpQCM.TabIndex = 2;
            tpQCM.Text = "QCM";
            tpQCM.UseVisualStyleBackColor = true;
            // 
            // label78
            // 
            label78.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label78.BackColor = SystemColors.ActiveCaption;
            label78.BorderStyle = BorderStyle.FixedSingle;
            label78.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label78.Location = new Point(1520, 314);
            label78.Name = "label78";
            label78.Size = new Size(163, 21);
            label78.TabIndex = 9;
            label78.Text = "Sensor Crystal Life";
            label78.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tlpCrystalLife1
            // 
            tlpCrystalLife1.BackColor = SystemColors.ActiveCaption;
            tlpCrystalLife1.CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble;
            tlpCrystalLife1.ColumnCount = 2;
            tlpCrystalLife1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tlpCrystalLife1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpCrystalLife1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpCrystalLife1.Controls.Add(lblValue_DcCrystalLife, 1, 2);
            tlpCrystalLife1.Controls.Add(lblValue_Rf2CrystalLife, 1, 1);
            tlpCrystalLife1.Controls.Add(label87, 0, 0);
            tlpCrystalLife1.Controls.Add(label88, 0, 1);
            tlpCrystalLife1.Controls.Add(label92, 0, 2);
            tlpCrystalLife1.Controls.Add(lblValue_Rf1CrystalLife, 1, 0);
            tlpCrystalLife1.Location = new Point(1520, 334);
            tlpCrystalLife1.Name = "tlpCrystalLife1";
            tlpCrystalLife1.RowCount = 3;
            tlpCrystalLife1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpCrystalLife1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpCrystalLife1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpCrystalLife1.Size = new Size(164, 130);
            tlpCrystalLife1.TabIndex = 8;
            // 
            // lblValue_DcCrystalLife
            // 
            lblValue_DcCrystalLife.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_DcCrystalLife.BackColor = Color.White;
            lblValue_DcCrystalLife.BorderStyle = BorderStyle.FixedSingle;
            lblValue_DcCrystalLife.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_DcCrystalLife.Location = new Point(59, 94);
            lblValue_DcCrystalLife.Name = "lblValue_DcCrystalLife";
            lblValue_DcCrystalLife.Size = new Size(99, 30);
            lblValue_DcCrystalLife.TabIndex = 4;
            lblValue_DcCrystalLife.Text = "-";
            lblValue_DcCrystalLife.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue_Rf2CrystalLife
            // 
            lblValue_Rf2CrystalLife.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf2CrystalLife.BackColor = Color.White;
            lblValue_Rf2CrystalLife.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf2CrystalLife.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf2CrystalLife.Location = new Point(59, 51);
            lblValue_Rf2CrystalLife.Name = "lblValue_Rf2CrystalLife";
            lblValue_Rf2CrystalLife.Size = new Size(99, 30);
            lblValue_Rf2CrystalLife.TabIndex = 2;
            lblValue_Rf2CrystalLife.Text = "-";
            lblValue_Rf2CrystalLife.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label87
            // 
            label87.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label87.AutoSize = true;
            label87.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label87.Location = new Point(6, 14);
            label87.Name = "label87";
            label87.Size = new Size(44, 17);
            label87.TabIndex = 0;
            label87.Text = "RF1";
            label87.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label88
            // 
            label88.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label88.AutoSize = true;
            label88.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label88.Location = new Point(6, 57);
            label88.Name = "label88";
            label88.Size = new Size(44, 17);
            label88.TabIndex = 0;
            label88.Text = "RF2";
            label88.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label92
            // 
            label92.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label92.AutoSize = true;
            label92.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label92.Location = new Point(6, 100);
            label92.Name = "label92";
            label92.Size = new Size(44, 17);
            label92.TabIndex = 0;
            label92.Text = "DC";
            label92.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblValue_Rf1CrystalLife
            // 
            lblValue_Rf1CrystalLife.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf1CrystalLife.BackColor = Color.White;
            lblValue_Rf1CrystalLife.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf1CrystalLife.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf1CrystalLife.Location = new Point(59, 8);
            lblValue_Rf1CrystalLife.Name = "lblValue_Rf1CrystalLife";
            lblValue_Rf1CrystalLife.Size = new Size(99, 30);
            lblValue_Rf1CrystalLife.TabIndex = 0;
            lblValue_Rf1CrystalLife.Text = "-";
            lblValue_Rf1CrystalLife.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // gbQcmSensorInfo
            // 
            gbQcmSensorInfo.BackColor = SystemColors.ActiveCaption;
            gbQcmSensorInfo.Controls.Add(label75);
            gbQcmSensorInfo.Controls.Add(label70);
            gbQcmSensorInfo.Controls.Add(label74);
            gbQcmSensorInfo.Controls.Add(label69);
            gbQcmSensorInfo.Controls.Add(tableLayoutPanel2);
            gbQcmSensorInfo.Font = new Font("Segoe UI", 12F);
            gbQcmSensorInfo.Location = new Point(1381, 488);
            gbQcmSensorInfo.Name = "gbQcmSensorInfo";
            gbQcmSensorInfo.Size = new Size(303, 193);
            gbQcmSensorInfo.TabIndex = 7;
            gbQcmSensorInfo.TabStop = false;
            gbQcmSensorInfo.Text = "Sensor Info";
            // 
            // label75
            // 
            label75.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label75.AutoSize = true;
            label75.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label75.Location = new Point(217, 39);
            label75.Name = "label75";
            label75.Size = new Size(34, 17);
            label75.TabIndex = 2;
            label75.Text = " (kÅ)";
            label75.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label70
            // 
            label70.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label70.AutoSize = true;
            label70.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label70.Location = new Point(203, 22);
            label70.Name = "label70";
            label70.Size = new Size(63, 17);
            label70.TabIndex = 2;
            label70.Text = "Thickness";
            label70.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label74
            // 
            label74.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label74.AutoSize = true;
            label74.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label74.Location = new Point(95, 39);
            label74.Name = "label74";
            label74.Size = new Size(35, 17);
            label74.TabIndex = 1;
            label74.Text = "(Å/s)";
            label74.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label69
            // 
            label69.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label69.AutoSize = true;
            label69.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label69.Location = new Point(95, 22);
            label69.Name = "label69";
            label69.Size = new Size(34, 17);
            label69.TabIndex = 1;
            label69.Text = "Rate";
            label69.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(lblValue_DcThickness, 2, 2);
            tableLayoutPanel2.Controls.Add(lblValue_DcRate, 1, 2);
            tableLayoutPanel2.Controls.Add(lblValue_Rf2Thickness, 2, 1);
            tableLayoutPanel2.Controls.Add(lblValue_Rf2Rate, 1, 1);
            tableLayoutPanel2.Controls.Add(lblValue_Rf1Thickness, 2, 0);
            tableLayoutPanel2.Controls.Add(label89, 0, 0);
            tableLayoutPanel2.Controls.Add(label90, 0, 1);
            tableLayoutPanel2.Controls.Add(label91, 0, 2);
            tableLayoutPanel2.Controls.Add(lblValue_Rf1Rate, 1, 0);
            tableLayoutPanel2.Location = new Point(3, 62);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.Size = new Size(296, 122);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblValue_DcThickness
            // 
            lblValue_DcThickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_DcThickness.BackColor = Color.White;
            lblValue_DcThickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue_DcThickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_DcThickness.Location = new Point(177, 88);
            lblValue_DcThickness.Name = "lblValue_DcThickness";
            lblValue_DcThickness.Size = new Size(115, 30);
            lblValue_DcThickness.TabIndex = 5;
            lblValue_DcThickness.Text = "-";
            lblValue_DcThickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue_DcRate
            // 
            lblValue_DcRate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_DcRate.BackColor = Color.White;
            lblValue_DcRate.BorderStyle = BorderStyle.FixedSingle;
            lblValue_DcRate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_DcRate.Location = new Point(55, 88);
            lblValue_DcRate.Name = "lblValue_DcRate";
            lblValue_DcRate.Size = new Size(115, 30);
            lblValue_DcRate.TabIndex = 4;
            lblValue_DcRate.Text = "-";
            lblValue_DcRate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue_Rf2Thickness
            // 
            lblValue_Rf2Thickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf2Thickness.BackColor = Color.White;
            lblValue_Rf2Thickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf2Thickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf2Thickness.Location = new Point(177, 47);
            lblValue_Rf2Thickness.Name = "lblValue_Rf2Thickness";
            lblValue_Rf2Thickness.Size = new Size(115, 30);
            lblValue_Rf2Thickness.TabIndex = 3;
            lblValue_Rf2Thickness.Text = "-";
            lblValue_Rf2Thickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue_Rf2Rate
            // 
            lblValue_Rf2Rate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf2Rate.BackColor = Color.White;
            lblValue_Rf2Rate.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf2Rate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf2Rate.Location = new Point(55, 47);
            lblValue_Rf2Rate.Name = "lblValue_Rf2Rate";
            lblValue_Rf2Rate.Size = new Size(115, 30);
            lblValue_Rf2Rate.TabIndex = 2;
            lblValue_Rf2Rate.Text = "-";
            lblValue_Rf2Rate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblValue_Rf1Thickness
            // 
            lblValue_Rf1Thickness.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf1Thickness.BackColor = Color.White;
            lblValue_Rf1Thickness.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf1Thickness.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf1Thickness.Location = new Point(177, 6);
            lblValue_Rf1Thickness.Name = "lblValue_Rf1Thickness";
            lblValue_Rf1Thickness.Size = new Size(115, 30);
            lblValue_Rf1Thickness.TabIndex = 1;
            lblValue_Rf1Thickness.Text = "-";
            lblValue_Rf1Thickness.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label89
            // 
            label89.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label89.AutoSize = true;
            label89.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label89.Location = new Point(4, 12);
            label89.Name = "label89";
            label89.Size = new Size(44, 17);
            label89.TabIndex = 0;
            label89.Text = "RF1";
            label89.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label90
            // 
            label90.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label90.AutoSize = true;
            label90.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label90.Location = new Point(4, 53);
            label90.Name = "label90";
            label90.Size = new Size(44, 17);
            label90.TabIndex = 0;
            label90.Text = "RF2";
            label90.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label91
            // 
            label91.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label91.AutoSize = true;
            label91.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label91.Location = new Point(4, 94);
            label91.Name = "label91";
            label91.Size = new Size(44, 17);
            label91.TabIndex = 0;
            label91.Text = "DC";
            label91.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblValue_Rf1Rate
            // 
            lblValue_Rf1Rate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblValue_Rf1Rate.BackColor = Color.White;
            lblValue_Rf1Rate.BorderStyle = BorderStyle.FixedSingle;
            lblValue_Rf1Rate.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblValue_Rf1Rate.Location = new Point(55, 6);
            lblValue_Rf1Rate.Name = "lblValue_Rf1Rate";
            lblValue_Rf1Rate.Size = new Size(115, 30);
            lblValue_Rf1Rate.TabIndex = 0;
            lblValue_Rf1Rate.Text = "-";
            lblValue_Rf1Rate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // groupBox4
            // 
            groupBox4.BackColor = SystemColors.ActiveCaption;
            groupBox4.Controls.Add(btnZeroThickness);
            groupBox4.Controls.Add(btnNextLayer);
            groupBox4.Controls.Add(btnStartLayer);
            groupBox4.Font = new Font("Segoe UI", 12F);
            groupBox4.Location = new Point(1381, 314);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(133, 150);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "Layer Control";
            // 
            // btnZeroThickness
            // 
            btnZeroThickness.Font = new Font("Segoe UI", 9F);
            btnZeroThickness.Image = Properties.Resources.reset16;
            btnZeroThickness.ImageAlign = ContentAlignment.MiddleLeft;
            btnZeroThickness.Location = new Point(12, 109);
            btnZeroThickness.Name = "btnZeroThickness";
            btnZeroThickness.Size = new Size(106, 35);
            btnZeroThickness.TabIndex = 6;
            btnZeroThickness.Text = "Zero Thick";
            btnZeroThickness.UseVisualStyleBackColor = true;
            btnZeroThickness.Click += btnZeroThickness_Click;
            // 
            // btnNextLayer
            // 
            btnNextLayer.Font = new Font("Segoe UI", 9F);
            btnNextLayer.Image = Properties.Resources.next_button16;
            btnNextLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnNextLayer.Location = new Point(12, 67);
            btnNextLayer.Name = "btnNextLayer";
            btnNextLayer.Size = new Size(106, 35);
            btnNextLayer.TabIndex = 6;
            btnNextLayer.Text = "Next Layer";
            btnNextLayer.UseVisualStyleBackColor = true;
            btnNextLayer.Click += btnNextLayer_Click;
            // 
            // btnStartLayer
            // 
            btnStartLayer.Font = new Font("Segoe UI", 9F);
            btnStartLayer.Image = Properties.Resources.power_button161;
            btnStartLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnStartLayer.Location = new Point(12, 26);
            btnStartLayer.Name = "btnStartLayer";
            btnStartLayer.Size = new Size(106, 35);
            btnStartLayer.TabIndex = 6;
            btnStartLayer.Text = "Start Layer";
            btnStartLayer.UseVisualStyleBackColor = true;
            btnStartLayer.Click += btnStartLayer_Click;
            // 
            // grpActiveLayerDetail
            // 
            grpActiveLayerDetail.BackColor = SystemColors.ActiveCaption;
            grpActiveLayerDetail.Controls.Add(cmbLayerDetailTitle);
            grpActiveLayerDetail.Controls.Add(lblLd);
            grpActiveLayerDetail.Controls.Add(tlpLayerDetail);
            grpActiveLayerDetail.Font = new Font("Segoe UI", 12F);
            grpActiveLayerDetail.Location = new Point(1380, 3);
            grpActiveLayerDetail.Name = "grpActiveLayerDetail";
            grpActiveLayerDetail.Size = new Size(303, 279);
            grpActiveLayerDetail.TabIndex = 5;
            grpActiveLayerDetail.TabStop = false;
            grpActiveLayerDetail.Text = "Active Layer Detail";
            // 
            // cmbLayerDetailTitle
            // 
            cmbLayerDetailTitle.BackColor = SystemColors.GradientInactiveCaption;
            cmbLayerDetailTitle.DropDownHeight = 105;
            cmbLayerDetailTitle.FormattingEnabled = true;
            cmbLayerDetailTitle.IntegralHeight = false;
            cmbLayerDetailTitle.Location = new Point(34, 25);
            cmbLayerDetailTitle.Name = "cmbLayerDetailTitle";
            cmbLayerDetailTitle.Size = new Size(265, 29);
            cmbLayerDetailTitle.TabIndex = 3;
            // 
            // lblLd
            // 
            lblLd.BackColor = SystemColors.GradientInactiveCaption;
            lblLd.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLd.Image = Properties.Resources.layer_2;
            lblLd.ImageAlign = ContentAlignment.MiddleLeft;
            lblLd.Location = new Point(3, 26);
            lblLd.Name = "lblLd";
            lblLd.Size = new Size(33, 27);
            lblLd.TabIndex = 1;
            lblLd.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tlpLayerDetail
            // 
            tlpLayerDetail.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tlpLayerDetail.ColumnCount = 2;
            tlpLayerDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tlpLayerDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            tlpLayerDetail.Controls.Add(lblLdStartMode, 1, 6);
            tlpLayerDetail.Controls.Add(lblLdSource, 1, 5);
            tlpLayerDetail.Controls.Add(lblLdFinalThk, 1, 4);
            tlpLayerDetail.Controls.Add(lblLdTooling, 1, 3);
            tlpLayerDetail.Controls.Add(lblLdZRatio, 1, 2);
            tlpLayerDetail.Controls.Add(lblLdDensity, 1, 1);
            tlpLayerDetail.Controls.Add(label3, 0, 0);
            tlpLayerDetail.Controls.Add(label4, 0, 1);
            tlpLayerDetail.Controls.Add(label23, 0, 2);
            tlpLayerDetail.Controls.Add(label24, 0, 3);
            tlpLayerDetail.Controls.Add(label33, 0, 4);
            tlpLayerDetail.Controls.Add(label41, 0, 5);
            tlpLayerDetail.Controls.Add(label62, 0, 6);
            tlpLayerDetail.Controls.Add(lblLdMaterial, 1, 0);
            tlpLayerDetail.Dock = DockStyle.Bottom;
            tlpLayerDetail.Location = new Point(3, 60);
            tlpLayerDetail.Name = "tlpLayerDetail";
            tlpLayerDetail.RowCount = 7;
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tlpLayerDetail.Size = new Size(297, 216);
            tlpLayerDetail.TabIndex = 0;
            // 
            // lblLdStartMode
            // 
            lblLdStartMode.BackColor = Color.White;
            lblLdStartMode.BorderStyle = BorderStyle.FixedSingle;
            lblLdStartMode.Dock = DockStyle.Fill;
            lblLdStartMode.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdStartMode.Location = new Point(137, 187);
            lblLdStartMode.Name = "lblLdStartMode";
            lblLdStartMode.Size = new Size(156, 30);
            lblLdStartMode.TabIndex = 6;
            lblLdStartMode.Text = "-";
            lblLdStartMode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblLdSource
            // 
            lblLdSource.BackColor = Color.White;
            lblLdSource.BorderStyle = BorderStyle.FixedSingle;
            lblLdSource.Dock = DockStyle.Fill;
            lblLdSource.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdSource.Location = new Point(137, 156);
            lblLdSource.Name = "lblLdSource";
            lblLdSource.Size = new Size(156, 30);
            lblLdSource.TabIndex = 5;
            lblLdSource.Text = "-";
            lblLdSource.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblLdFinalThk
            // 
            lblLdFinalThk.BackColor = Color.White;
            lblLdFinalThk.BorderStyle = BorderStyle.FixedSingle;
            lblLdFinalThk.Dock = DockStyle.Fill;
            lblLdFinalThk.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdFinalThk.Location = new Point(137, 125);
            lblLdFinalThk.Name = "lblLdFinalThk";
            lblLdFinalThk.Size = new Size(156, 30);
            lblLdFinalThk.TabIndex = 4;
            lblLdFinalThk.Text = "-";
            lblLdFinalThk.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblLdTooling
            // 
            lblLdTooling.BackColor = Color.White;
            lblLdTooling.BorderStyle = BorderStyle.FixedSingle;
            lblLdTooling.Dock = DockStyle.Fill;
            lblLdTooling.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdTooling.Location = new Point(137, 94);
            lblLdTooling.Name = "lblLdTooling";
            lblLdTooling.Size = new Size(156, 30);
            lblLdTooling.TabIndex = 3;
            lblLdTooling.Text = "-";
            lblLdTooling.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblLdZRatio
            // 
            lblLdZRatio.BackColor = Color.White;
            lblLdZRatio.BorderStyle = BorderStyle.FixedSingle;
            lblLdZRatio.Dock = DockStyle.Fill;
            lblLdZRatio.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdZRatio.Location = new Point(137, 63);
            lblLdZRatio.Name = "lblLdZRatio";
            lblLdZRatio.Size = new Size(156, 30);
            lblLdZRatio.TabIndex = 2;
            lblLdZRatio.Text = "-";
            lblLdZRatio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblLdDensity
            // 
            lblLdDensity.BackColor = Color.White;
            lblLdDensity.BorderStyle = BorderStyle.FixedSingle;
            lblLdDensity.Dock = DockStyle.Fill;
            lblLdDensity.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdDensity.Location = new Point(137, 32);
            lblLdDensity.Name = "lblLdDensity";
            lblLdDensity.Size = new Size(156, 30);
            lblLdDensity.TabIndex = 1;
            lblLdDensity.Text = "-";
            lblLdDensity.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label3.Location = new Point(4, 7);
            label3.Name = "label3";
            label3.Size = new Size(126, 17);
            label3.TabIndex = 0;
            label3.Text = "Material";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label4.Location = new Point(4, 38);
            label4.Name = "label4";
            label4.Size = new Size(126, 17);
            label4.TabIndex = 0;
            label4.Text = "Density";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            label23.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label23.Location = new Point(4, 69);
            label23.Name = "label23";
            label23.Size = new Size(126, 17);
            label23.TabIndex = 0;
            label23.Text = "Z-Ratio";
            label23.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            label24.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label24.AutoSize = true;
            label24.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label24.Location = new Point(4, 100);
            label24.Name = "label24";
            label24.Size = new Size(126, 17);
            label24.TabIndex = 0;
            label24.Text = "Tooling";
            label24.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            label33.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label33.AutoSize = true;
            label33.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label33.Location = new Point(4, 131);
            label33.Name = "label33";
            label33.Size = new Size(126, 17);
            label33.TabIndex = 0;
            label33.Text = "Final Thickness";
            label33.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label41
            // 
            label41.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label41.AutoSize = true;
            label41.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label41.Location = new Point(4, 162);
            label41.Name = "label41";
            label41.Size = new Size(126, 17);
            label41.TabIndex = 0;
            label41.Text = "Source";
            label41.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label62
            // 
            label62.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label62.AutoSize = true;
            label62.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label62.Location = new Point(4, 193);
            label62.Name = "label62";
            label62.Size = new Size(126, 17);
            label62.TabIndex = 0;
            label62.Text = "Start Mode";
            label62.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblLdMaterial
            // 
            lblLdMaterial.BackColor = Color.White;
            lblLdMaterial.BorderStyle = BorderStyle.FixedSingle;
            lblLdMaterial.Dock = DockStyle.Fill;
            lblLdMaterial.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lblLdMaterial.Location = new Point(137, 1);
            lblLdMaterial.Name = "lblLdMaterial";
            lblLdMaterial.Size = new Size(156, 30);
            lblLdMaterial.TabIndex = 0;
            lblLdMaterial.Text = "-";
            lblLdMaterial.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpLayerList
            // 
            grpLayerList.BackColor = SystemColors.ActiveCaption;
            grpLayerList.Controls.Add(dgvLayers);
            grpLayerList.Controls.Add(pnlLayerButtons);
            grpLayerList.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            grpLayerList.Location = new Point(740, 3);
            grpLayerList.Name = "grpLayerList";
            grpLayerList.Size = new Size(609, 896);
            grpLayerList.TabIndex = 4;
            grpLayerList.TabStop = false;
            grpLayerList.Text = "Layer List";
            // 
            // dgvLayers
            // 
            dgvLayers.AllowUserToAddRows = false;
            dgvLayers.AllowUserToDeleteRows = false;
            dgvLayers.AllowUserToResizeColumns = false;
            dgvLayers.AllowUserToResizeRows = false;
            dgvLayers.BackgroundColor = SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvLayers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvLayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLayers.Columns.AddRange(new DataGridViewColumn[] { colLayerInfo, colProcessNumber, colProcessName, colFinalThickness, colSource, colRF1Sensor, colRF2Sensor, colDCSensor, colStartMode });
            dgvLayers.Dock = DockStyle.Top;
            dgvLayers.Location = new Point(3, 25);
            dgvLayers.MultiSelect = false;
            dgvLayers.Name = "dgvLayers";
            dgvLayers.ReadOnly = true;
            dgvLayers.RowHeadersVisible = false;
            dgvLayers.RowHeadersWidth = 51;
            dgvLayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLayers.Size = new Size(603, 819);
            dgvLayers.TabIndex = 2;
            // 
            // colLayerInfo
            // 
            colLayerInfo.HeaderText = "Layer Info";
            colLayerInfo.MinimumWidth = 6;
            colLayerInfo.Name = "colLayerInfo";
            colLayerInfo.ReadOnly = true;
            colLayerInfo.Width = 85;
            // 
            // colProcessNumber
            // 
            colProcessNumber.HeaderText = "Process Number";
            colProcessNumber.MinimumWidth = 6;
            colProcessNumber.Name = "colProcessNumber";
            colProcessNumber.ReadOnly = true;
            colProcessNumber.Width = 70;
            // 
            // colProcessName
            // 
            colProcessName.HeaderText = "Process Name";
            colProcessName.MinimumWidth = 6;
            colProcessName.Name = "colProcessName";
            colProcessName.ReadOnly = true;
            colProcessName.Width = 105;
            // 
            // colFinalThickness
            // 
            colFinalThickness.HeaderText = "Final Thick(kÅ)";
            colFinalThickness.MinimumWidth = 6;
            colFinalThickness.Name = "colFinalThickness";
            colFinalThickness.ReadOnly = true;
            colFinalThickness.Width = 85;
            // 
            // colSource
            // 
            colSource.HeaderText = "Source";
            colSource.MinimumWidth = 6;
            colSource.Name = "colSource";
            colSource.ReadOnly = true;
            colSource.Width = 55;
            // 
            // colRF1Sensor
            // 
            colRF1Sensor.HeaderText = "RF1 Sensor";
            colRF1Sensor.MinimumWidth = 6;
            colRF1Sensor.Name = "colRF1Sensor";
            colRF1Sensor.ReadOnly = true;
            colRF1Sensor.Width = 50;
            // 
            // colRF2Sensor
            // 
            colRF2Sensor.HeaderText = "RF2 Sensor";
            colRF2Sensor.MinimumWidth = 6;
            colRF2Sensor.Name = "colRF2Sensor";
            colRF2Sensor.ReadOnly = true;
            colRF2Sensor.Width = 50;
            // 
            // colDCSensor
            // 
            colDCSensor.HeaderText = "DC Sensor";
            colDCSensor.MinimumWidth = 6;
            colDCSensor.Name = "colDCSensor";
            colDCSensor.ReadOnly = true;
            colDCSensor.Width = 50;
            // 
            // colStartMode
            // 
            colStartMode.HeaderText = "Start Mode";
            colStartMode.MinimumWidth = 6;
            colStartMode.Name = "colStartMode";
            colStartMode.ReadOnly = true;
            colStartMode.Width = 50;
            // 
            // pnlLayerButtons
            // 
            pnlLayerButtons.BackColor = SystemColors.GradientActiveCaption;
            pnlLayerButtons.BorderStyle = BorderStyle.FixedSingle;
            pnlLayerButtons.Controls.Add(btnDeleteLayer);
            pnlLayerButtons.Controls.Add(btnEditLayer);
            pnlLayerButtons.Controls.Add(btnInsertCoDepLayer);
            pnlLayerButtons.Controls.Add(btnInsertNormalLayer);
            pnlLayerButtons.Location = new Point(6, 850);
            pnlLayerButtons.Name = "pnlLayerButtons";
            pnlLayerButtons.Size = new Size(597, 40);
            pnlLayerButtons.TabIndex = 1;
            // 
            // btnDeleteLayer
            // 
            btnDeleteLayer.Font = new Font("Segoe UI", 9F);
            btnDeleteLayer.Image = Properties.Resources.layer_remove;
            btnDeleteLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnDeleteLayer.Location = new Point(446, 4);
            btnDeleteLayer.Name = "btnDeleteLayer";
            btnDeleteLayer.Size = new Size(138, 31);
            btnDeleteLayer.TabIndex = 7;
            btnDeleteLayer.Text = "Delete Layer";
            btnDeleteLayer.UseVisualStyleBackColor = true;
            btnDeleteLayer.Click += btnDeleteLayer_Click;
            // 
            // btnEditLayer
            // 
            btnEditLayer.Font = new Font("Segoe UI", 9F);
            btnEditLayer.Image = Properties.Resources.overlay;
            btnEditLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditLayer.Location = new Point(302, 4);
            btnEditLayer.Name = "btnEditLayer";
            btnEditLayer.Size = new Size(138, 31);
            btnEditLayer.TabIndex = 6;
            btnEditLayer.Text = "Edit Layer";
            btnEditLayer.UseVisualStyleBackColor = true;
            btnEditLayer.Click += btnEditLayer_Click;
            // 
            // btnInsertCoDepLayer
            // 
            btnInsertCoDepLayer.Enabled = false;
            btnInsertCoDepLayer.Font = new Font("Segoe UI", 9F);
            btnInsertCoDepLayer.Image = Properties.Resources.layer;
            btnInsertCoDepLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnInsertCoDepLayer.Location = new Point(158, 4);
            btnInsertCoDepLayer.Name = "btnInsertCoDepLayer";
            btnInsertCoDepLayer.Size = new Size(138, 31);
            btnInsertCoDepLayer.TabIndex = 5;
            btnInsertCoDepLayer.Text = "Insert CoDep Layer";
            btnInsertCoDepLayer.TextAlign = ContentAlignment.MiddleRight;
            btnInsertCoDepLayer.UseVisualStyleBackColor = true;
            // 
            // btnInsertNormalLayer
            // 
            btnInsertNormalLayer.Font = new Font("Segoe UI", 9F);
            btnInsertNormalLayer.Image = Properties.Resources.layer_add;
            btnInsertNormalLayer.ImageAlign = ContentAlignment.MiddleLeft;
            btnInsertNormalLayer.Location = new Point(14, 4);
            btnInsertNormalLayer.Name = "btnInsertNormalLayer";
            btnInsertNormalLayer.Size = new Size(138, 31);
            btnInsertNormalLayer.TabIndex = 4;
            btnInsertNormalLayer.Text = "Insert Normal Layer";
            btnInsertNormalLayer.TextAlign = ContentAlignment.MiddleRight;
            btnInsertNormalLayer.UseVisualStyleBackColor = true;
            btnInsertNormalLayer.Click += btnInsertNormalLayer_Click;
            // 
            // grpProcessList
            // 
            grpProcessList.BackColor = SystemColors.ActiveCaption;
            grpProcessList.Controls.Add(dgvProcesses);
            grpProcessList.Controls.Add(pnlProcessButtons);
            grpProcessList.Font = new Font("Segoe UI", 12F);
            grpProcessList.Location = new Point(373, 3);
            grpProcessList.Name = "grpProcessList";
            grpProcessList.Size = new Size(349, 896);
            grpProcessList.TabIndex = 3;
            grpProcessList.TabStop = false;
            grpProcessList.Text = "Process List";
            // 
            // dgvProcesses
            // 
            dgvProcesses.AllowUserToAddRows = false;
            dgvProcesses.AllowUserToDeleteRows = false;
            dgvProcesses.AllowUserToResizeColumns = false;
            dgvProcesses.AllowUserToResizeRows = false;
            dgvProcesses.BackgroundColor = SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvProcesses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvProcesses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProcesses.Columns.AddRange(new DataGridViewColumn[] { colProcessNum, colProcName, colProcMaterialNumber, colProcTooling });
            dgvProcesses.Dock = DockStyle.Top;
            dgvProcesses.Location = new Point(3, 25);
            dgvProcesses.MultiSelect = false;
            dgvProcesses.Name = "dgvProcesses";
            dgvProcesses.ReadOnly = true;
            dgvProcesses.RowHeadersVisible = false;
            dgvProcesses.RowHeadersWidth = 51;
            dgvProcesses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProcesses.Size = new Size(343, 819);
            dgvProcesses.TabIndex = 2;
            dgvProcesses.RowPostPaint += dgvProcesses_RowPostPaint;
            // 
            // colProcessNum
            // 
            colProcessNum.HeaderText = "Number";
            colProcessNum.MinimumWidth = 6;
            colProcessNum.Name = "colProcessNum";
            colProcessNum.ReadOnly = true;
            colProcessNum.Width = 65;
            // 
            // colProcName
            // 
            colProcName.DataPropertyName = "Name";
            colProcName.HeaderText = "Process Name";
            colProcName.MinimumWidth = 6;
            colProcName.Name = "colProcName";
            colProcName.ReadOnly = true;
            colProcName.Width = 125;
            // 
            // colProcMaterialNumber
            // 
            colProcMaterialNumber.HeaderText = "Material Number";
            colProcMaterialNumber.MinimumWidth = 6;
            colProcMaterialNumber.Name = "colProcMaterialNumber";
            colProcMaterialNumber.ReadOnly = true;
            colProcMaterialNumber.Width = 75;
            // 
            // colProcTooling
            // 
            colProcTooling.DataPropertyName = "Tooling";
            colProcTooling.HeaderText = "Tooling (%)";
            colProcTooling.MinimumWidth = 6;
            colProcTooling.Name = "colProcTooling";
            colProcTooling.ReadOnly = true;
            colProcTooling.Width = 75;
            // 
            // pnlProcessButtons
            // 
            pnlProcessButtons.BackColor = SystemColors.GradientActiveCaption;
            pnlProcessButtons.BorderStyle = BorderStyle.FixedSingle;
            pnlProcessButtons.Controls.Add(btnProcDelete);
            pnlProcessButtons.Controls.Add(btnProcEdit);
            pnlProcessButtons.Controls.Add(btnProcAdd);
            pnlProcessButtons.Location = new Point(23, 850);
            pnlProcessButtons.Name = "pnlProcessButtons";
            pnlProcessButtons.Size = new Size(298, 40);
            pnlProcessButtons.TabIndex = 1;
            // 
            // btnProcDelete
            // 
            btnProcDelete.Font = new Font("Segoe UI", 9F);
            btnProcDelete.Image = Properties.Resources.remove;
            btnProcDelete.ImageAlign = ContentAlignment.MiddleLeft;
            btnProcDelete.Location = new Point(202, 4);
            btnProcDelete.Name = "btnProcDelete";
            btnProcDelete.Size = new Size(89, 31);
            btnProcDelete.TabIndex = 6;
            btnProcDelete.Text = "Delete";
            btnProcDelete.UseVisualStyleBackColor = true;
            btnProcDelete.Click += btnProcDelete_Click;
            // 
            // btnProcEdit
            // 
            btnProcEdit.Font = new Font("Segoe UI", 9F);
            btnProcEdit.Image = Properties.Resources.edit;
            btnProcEdit.ImageAlign = ContentAlignment.MiddleLeft;
            btnProcEdit.Location = new Point(103, 4);
            btnProcEdit.Name = "btnProcEdit";
            btnProcEdit.Size = new Size(89, 31);
            btnProcEdit.TabIndex = 5;
            btnProcEdit.Text = "Edit";
            btnProcEdit.UseVisualStyleBackColor = true;
            btnProcEdit.Click += btnProcEdit_Click;
            // 
            // btnProcAdd
            // 
            btnProcAdd.Font = new Font("Segoe UI", 9F);
            btnProcAdd.Image = Properties.Resources.add;
            btnProcAdd.ImageAlign = ContentAlignment.MiddleLeft;
            btnProcAdd.Location = new Point(4, 4);
            btnProcAdd.Name = "btnProcAdd";
            btnProcAdd.Size = new Size(89, 31);
            btnProcAdd.TabIndex = 4;
            btnProcAdd.Text = "Add";
            btnProcAdd.UseVisualStyleBackColor = true;
            btnProcAdd.Click += btnProcAdd_Click;
            // 
            // grpMaterialList
            // 
            grpMaterialList.BackColor = SystemColors.ActiveCaption;
            grpMaterialList.Controls.Add(dgvMaterials);
            grpMaterialList.Controls.Add(pnlMaterialButtons);
            grpMaterialList.Font = new Font("Segoe UI", 12F);
            grpMaterialList.Location = new Point(3, 3);
            grpMaterialList.Name = "grpMaterialList";
            grpMaterialList.Size = new Size(352, 896);
            grpMaterialList.TabIndex = 2;
            grpMaterialList.TabStop = false;
            grpMaterialList.Text = "Material List";
            // 
            // dgvMaterials
            // 
            dgvMaterials.AllowUserToAddRows = false;
            dgvMaterials.AllowUserToDeleteRows = false;
            dgvMaterials.AllowUserToResizeColumns = false;
            dgvMaterials.AllowUserToResizeRows = false;
            dgvMaterials.BackgroundColor = SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvMaterials.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvMaterials.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMaterials.Columns.AddRange(new DataGridViewColumn[] { colMatNumber, colMatName, colMatDensity, colMatZ });
            dgvMaterials.Location = new Point(3, 25);
            dgvMaterials.MultiSelect = false;
            dgvMaterials.Name = "dgvMaterials";
            dgvMaterials.ReadOnly = true;
            dgvMaterials.RowHeadersVisible = false;
            dgvMaterials.RowHeadersWidth = 51;
            dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterials.Size = new Size(343, 819);
            dgvMaterials.TabIndex = 0;
            dgvMaterials.RowPostPaint += dgvMaterials_RowPostPaint;
            // 
            // colMatNumber
            // 
            colMatNumber.DataPropertyName = "Number";
            colMatNumber.HeaderText = "Number";
            colMatNumber.MinimumWidth = 6;
            colMatNumber.Name = "colMatNumber";
            colMatNumber.ReadOnly = true;
            colMatNumber.Width = 65;
            // 
            // colMatName
            // 
            colMatName.DataPropertyName = "Name";
            colMatName.HeaderText = "Material Name";
            colMatName.MinimumWidth = 6;
            colMatName.Name = "colMatName";
            colMatName.ReadOnly = true;
            colMatName.Width = 125;
            // 
            // colMatDensity
            // 
            colMatDensity.DataPropertyName = "Density";
            colMatDensity.HeaderText = "Density (g/cm³)";
            colMatDensity.MinimumWidth = 6;
            colMatDensity.Name = "colMatDensity";
            colMatDensity.ReadOnly = true;
            colMatDensity.Width = 75;
            // 
            // colMatZ
            // 
            colMatZ.DataPropertyName = "ZRatio";
            colMatZ.HeaderText = "Z-Ratio";
            colMatZ.MinimumWidth = 6;
            colMatZ.Name = "colMatZ";
            colMatZ.ReadOnly = true;
            colMatZ.Width = 75;
            // 
            // pnlMaterialButtons
            // 
            pnlMaterialButtons.BackColor = SystemColors.GradientActiveCaption;
            pnlMaterialButtons.BorderStyle = BorderStyle.FixedSingle;
            pnlMaterialButtons.Controls.Add(btnMatDelete);
            pnlMaterialButtons.Controls.Add(btnMatEdit);
            pnlMaterialButtons.Controls.Add(btnMatAdd);
            pnlMaterialButtons.Location = new Point(22, 850);
            pnlMaterialButtons.Name = "pnlMaterialButtons";
            pnlMaterialButtons.Size = new Size(298, 40);
            pnlMaterialButtons.TabIndex = 1;
            // 
            // btnMatDelete
            // 
            btnMatDelete.Font = new Font("Segoe UI", 9F);
            btnMatDelete.Image = Properties.Resources.remove;
            btnMatDelete.ImageAlign = ContentAlignment.MiddleLeft;
            btnMatDelete.Location = new Point(201, 4);
            btnMatDelete.Name = "btnMatDelete";
            btnMatDelete.Size = new Size(89, 31);
            btnMatDelete.TabIndex = 6;
            btnMatDelete.Text = "Delete";
            btnMatDelete.UseVisualStyleBackColor = true;
            btnMatDelete.Click += btnMatDelete_Click;
            // 
            // btnMatEdit
            // 
            btnMatEdit.Font = new Font("Segoe UI", 9F);
            btnMatEdit.Image = Properties.Resources.edit;
            btnMatEdit.ImageAlign = ContentAlignment.MiddleLeft;
            btnMatEdit.Location = new Point(102, 4);
            btnMatEdit.Name = "btnMatEdit";
            btnMatEdit.Size = new Size(89, 31);
            btnMatEdit.TabIndex = 5;
            btnMatEdit.Text = "Edit";
            btnMatEdit.UseVisualStyleBackColor = true;
            btnMatEdit.Click += btnMatEdit_Click;
            // 
            // btnMatAdd
            // 
            btnMatAdd.Font = new Font("Segoe UI", 9F);
            btnMatAdd.Image = Properties.Resources.add;
            btnMatAdd.ImageAlign = ContentAlignment.MiddleLeft;
            btnMatAdd.Location = new Point(3, 4);
            btnMatAdd.Name = "btnMatAdd";
            btnMatAdd.Size = new Size(89, 31);
            btnMatAdd.TabIndex = 4;
            btnMatAdd.Text = "Add";
            btnMatAdd.UseVisualStyleBackColor = true;
            btnMatAdd.Click += btnMatAdd_Click;
            // 
            // tpLogPage
            // 
            tpLogPage.Controls.Add(panelEventsBottom);
            tpLogPage.Controls.Add(panelEventsTop);
            tpLogPage.Controls.Add(dgvEvents);
            tpLogPage.Location = new Point(4, 27);
            tpLogPage.Name = "tpLogPage";
            tpLogPage.Padding = new Padding(3);
            tpLogPage.Size = new Size(1884, 902);
            tpLogPage.TabIndex = 3;
            tpLogPage.Text = "Log Event";
            tpLogPage.UseVisualStyleBackColor = true;
            // 
            // panelEventsBottom
            // 
            panelEventsBottom.Controls.Add(lblEvtStats);
            panelEventsBottom.Dock = DockStyle.Bottom;
            panelEventsBottom.Location = new Point(3, 879);
            panelEventsBottom.Name = "panelEventsBottom";
            panelEventsBottom.Size = new Size(1878, 20);
            panelEventsBottom.TabIndex = 8;
            // 
            // lblEvtStats
            // 
            lblEvtStats.AutoSize = true;
            lblEvtStats.Dock = DockStyle.Fill;
            lblEvtStats.Location = new Point(0, 0);
            lblEvtStats.Name = "lblEvtStats";
            lblEvtStats.Size = new Size(65, 15);
            lblEvtStats.TabIndex = 6;
            lblEvtStats.Text = "lblLogStats";
            // 
            // panelEventsTop
            // 
            panelEventsTop.Controls.Add(button4);
            panelEventsTop.Controls.Add(dtpEvtTo);
            panelEventsTop.Controls.Add(dtpEvtFrom);
            panelEventsTop.Controls.Add(cmbEvtSource);
            panelEventsTop.Controls.Add(cmbEvtLevel);
            panelEventsTop.Controls.Add(cmbEvtTimeRange);
            panelEventsTop.Controls.Add(chkEvtAutoScroll);
            panelEventsTop.Controls.Add(txtEvtSearch);
            panelEventsTop.Controls.Add(btnEvtApplyRange);
            panelEventsTop.Controls.Add(btnEvtExport);
            panelEventsTop.Dock = DockStyle.Top;
            panelEventsTop.Location = new Point(3, 3);
            panelEventsTop.Name = "panelEventsTop";
            panelEventsTop.Size = new Size(1878, 29);
            panelEventsTop.TabIndex = 7;
            // 
            // button4
            // 
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.Location = new Point(617, 3);
            button4.Name = "button4";
            button4.Size = new Size(121, 23);
            button4.TabIndex = 9;
            button4.Text = "Clear Log";
            button4.UseVisualStyleBackColor = true;
            // 
            // dtpEvtTo
            // 
            dtpEvtTo.Location = new Point(1056, 2);
            dtpEvtTo.Name = "dtpEvtTo";
            dtpEvtTo.Size = new Size(200, 23);
            dtpEvtTo.TabIndex = 7;
            // 
            // dtpEvtFrom
            // 
            dtpEvtFrom.Location = new Point(835, 2);
            dtpEvtFrom.Name = "dtpEvtFrom";
            dtpEvtFrom.Size = new Size(200, 23);
            dtpEvtFrom.TabIndex = 6;
            // 
            // cmbEvtSource
            // 
            cmbEvtSource.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEvtSource.FormattingEnabled = true;
            cmbEvtSource.Location = new Point(130, 3);
            cmbEvtSource.Name = "cmbEvtSource";
            cmbEvtSource.Size = new Size(121, 23);
            cmbEvtSource.TabIndex = 1;
            // 
            // cmbEvtLevel
            // 
            cmbEvtLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEvtLevel.FormattingEnabled = true;
            cmbEvtLevel.Location = new Point(3, 3);
            cmbEvtLevel.Name = "cmbEvtLevel";
            cmbEvtLevel.Size = new Size(121, 23);
            cmbEvtLevel.TabIndex = 0;
            // 
            // cmbEvtTimeRange
            // 
            cmbEvtTimeRange.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEvtTimeRange.FormattingEnabled = true;
            cmbEvtTimeRange.Items.AddRange(new object[] { "ALL, Last 1m, Last 5m, Last 15m, Last 1h, Today, Custom" });
            cmbEvtTimeRange.Location = new Point(257, 3);
            cmbEvtTimeRange.Name = "cmbEvtTimeRange";
            cmbEvtTimeRange.Size = new Size(121, 23);
            cmbEvtTimeRange.TabIndex = 1;
            // 
            // chkEvtAutoScroll
            // 
            chkEvtAutoScroll.AutoSize = true;
            chkEvtAutoScroll.Location = new Point(745, 4);
            chkEvtAutoScroll.Name = "chkEvtAutoScroll";
            chkEvtAutoScroll.Size = new Size(84, 19);
            chkEvtAutoScroll.TabIndex = 4;
            chkEvtAutoScroll.Text = "Auto Scroll";
            chkEvtAutoScroll.UseVisualStyleBackColor = true;
            // 
            // txtEvtSearch
            // 
            txtEvtSearch.Location = new Point(384, 3);
            txtEvtSearch.Name = "txtEvtSearch";
            txtEvtSearch.Size = new Size(100, 23);
            txtEvtSearch.TabIndex = 2;
            // 
            // btnEvtApplyRange
            // 
            btnEvtApplyRange.Image = Properties.Resources.checked16;
            btnEvtApplyRange.ImageAlign = ContentAlignment.MiddleLeft;
            btnEvtApplyRange.Location = new Point(1262, 2);
            btnEvtApplyRange.Name = "btnEvtApplyRange";
            btnEvtApplyRange.Size = new Size(121, 23);
            btnEvtApplyRange.TabIndex = 3;
            btnEvtApplyRange.Text = "Apply";
            btnEvtApplyRange.UseVisualStyleBackColor = true;
            // 
            // btnEvtExport
            // 
            btnEvtExport.Image = Properties.Resources.csv_icon16;
            btnEvtExport.ImageAlign = ContentAlignment.MiddleLeft;
            btnEvtExport.Location = new Point(490, 3);
            btnEvtExport.Name = "btnEvtExport";
            btnEvtExport.Size = new Size(121, 23);
            btnEvtExport.TabIndex = 3;
            btnEvtExport.Text = "Export";
            btnEvtExport.UseVisualStyleBackColor = true;
            // 
            // dgvEvents
            // 
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEvents.Columns.AddRange(new DataGridViewColumn[] { colTime, colType, colCode, colMessage, colSourceLog, colRegistrationNo });
            dgvEvents.Location = new Point(3, 51);
            dgvEvents.Name = "dgvEvents";
            dgvEvents.ReadOnly = true;
            dgvEvents.RowHeadersWidth = 51;
            dgvEvents.Size = new Size(1878, 822);
            dgvEvents.TabIndex = 5;
            // 
            // colTime
            // 
            colTime.HeaderText = "Time";
            colTime.MinimumWidth = 6;
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            colTime.Width = 125;
            // 
            // colType
            // 
            colType.HeaderText = "Type";
            colType.MinimumWidth = 6;
            colType.Name = "colType";
            colType.ReadOnly = true;
            colType.Width = 125;
            // 
            // colCode
            // 
            colCode.HeaderText = "Code";
            colCode.MinimumWidth = 6;
            colCode.Name = "colCode";
            colCode.ReadOnly = true;
            colCode.Width = 125;
            // 
            // colMessage
            // 
            colMessage.HeaderText = "Message";
            colMessage.MinimumWidth = 6;
            colMessage.Name = "colMessage";
            colMessage.ReadOnly = true;
            colMessage.Width = 125;
            // 
            // colSourceLog
            // 
            colSourceLog.HeaderText = "Source";
            colSourceLog.MinimumWidth = 6;
            colSourceLog.Name = "colSourceLog";
            colSourceLog.ReadOnly = true;
            colSourceLog.Width = 125;
            // 
            // colRegistrationNo
            // 
            colRegistrationNo.HeaderText = "Registration No ";
            colRegistrationNo.Name = "colRegistrationNo";
            colRegistrationNo.ReadOnly = true;
            colRegistrationNo.Width = 120;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lstLog);
            tabPage1.Location = new Point(4, 27);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1884, 902);
            tabPage1.TabIndex = 4;
            tabPage1.Text = "Debug";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lstLog
            // 
            lstLog.Dock = DockStyle.Fill;
            lstLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            lstLog.FormattingEnabled = true;
            lstLog.HorizontalScrollbar = true;
            lstLog.IntegralHeight = false;
            lstLog.Location = new Point(3, 3);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(1878, 896);
            lstLog.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(1754, 985);
            Controls.Add(statusStrip1);
            Controls.Add(pnlTopBar);
            Controls.Add(menuMain);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuMain;
            MinimumSize = new Size(1750, 1020);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sputter Control HMI";
            Load += MainForm_Load;
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            pnlTopBar.ResumeLayout(false);
            pnlTopBar.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tpProcess.ResumeLayout(false);
            gbQcmSensorInfo2.ResumeLayout(false);
            gbQcmSensorInfo2.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            grpRf2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numID2_SetPointWrite).EndInit();
            tableLayoutPanel13.ResumeLayout(false);
            tableLayoutPanel13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numID1_SetPointWrite).EndInit();
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            panel12.ResumeLayout(false);
            panel12.PerformLayout();
            panel13.ResumeLayout(false);
            panel14.ResumeLayout(false);
            panel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRf2_OffRamp).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_Dwell).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_Ramp).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf2_Step2_SetW).EndInit();
            panel15.ResumeLayout(false);
            panel15.PerformLayout();
            grpRf1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel9.ResumeLayout(false);
            tableLayoutPanel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numID3_SetPointWrite).EndInit();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel8.ResumeLayout(false);
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_SetW).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_OffRamp).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_Dwell).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRf1_Step2_Ramp).EndInit();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            grpDC.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numID5_SetPointWrite).EndInit();
            tableLayoutPanel11.ResumeLayout(false);
            tableLayoutPanel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numID4_SetPointWrite).EndInit();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDC_OffRamp).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_Dwell).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_Ramp).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDc_Step2_SetW).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            grpStartControls.ResumeLayout(false);
            tlpAnalogFeedback.ResumeLayout(false);
            tlpAnalogFeedback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numGateValvePosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMotorRpm).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRezistanceControl).EndInit();
            tlpStartControls.ResumeLayout(false);
            tlpStartControls.PerformLayout();
            panel16.ResumeLayout(false);
            panel16.PerformLayout();
            tpGraph.ResumeLayout(false);
            tpGraph.PerformLayout();
            tpQCM.ResumeLayout(false);
            tlpCrystalLife1.ResumeLayout(false);
            tlpCrystalLife1.PerformLayout();
            gbQcmSensorInfo.ResumeLayout(false);
            gbQcmSensorInfo.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            groupBox4.ResumeLayout(false);
            grpActiveLayerDetail.ResumeLayout(false);
            tlpLayerDetail.ResumeLayout(false);
            tlpLayerDetail.PerformLayout();
            grpLayerList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLayers).EndInit();
            pnlLayerButtons.ResumeLayout(false);
            grpProcessList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProcesses).EndInit();
            pnlProcessButtons.ResumeLayout(false);
            grpMaterialList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMaterials).EndInit();
            pnlMaterialButtons.ResumeLayout(false);
            tpLogPage.ResumeLayout(false);
            panelEventsBottom.ResumeLayout(false);
            panelEventsBottom.PerformLayout();
            panelEventsTop.ResumeLayout(false);
            panelEventsTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEvents).EndInit();
            tabPage1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuMain;
        private ToolStripMenuItem connectionToolStripMenuItem;
        private ToolStripMenuItem miDisconnect;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem miManualRead;
        private Panel pnlTopBar;
        private Button btnDisconnect;
        private Label lblPollState;
        private Label lblConnState;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tsStatus;
        private Button btnRefresh;
        private TabControl tabControl1;
        private TabPage tpProcess;
        private GroupBox grpRf2;
        private GroupBox groupBox3;
        private TableLayoutPanel tableLayoutPanel8;
        private TableLayoutPanel tableLayoutPanel12;
        private Label lblID2_Title_O2;
        private Label lblID2_ActualFlowValue;
        private NumericUpDown numID2_SetPointWrite;
        private Button btnID2_ValveToggle;
        private Label label71;
        private Label label72;
        private Label label73;
        private TableLayoutPanel tableLayoutPanel13;
        private Label lblID1_Title_Ar;
        private Label lblID1_ActualFlowValue;
        private NumericUpDown numID1_SetPointWrite;
        private Button btnID1_ValveToggle;
        private Panel panel11;
        private Label label38;
        private TextBox txtRf2_TargetLife;
        private Label label39;
        private Label label40;
        private Panel panel12;
        private Label lblRf2_RefP;
        private Label lblRf2_FwP;
        private Label label43;
        private ProgressBar pbRf2_RefP;
        private Label label44;
        private ProgressBar pbRf2_FwP;
        private Panel panel13;
        private Panel panel14;
        private Label label56;
        private NumericUpDown numRf2_Step2_SetW;
        private Label label45;
        private Label label46;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label53;
        private TextBox txtRf2_Step1_SetW;
        private TextBox txtRf2_Step1_Ramp;
        private TextBox txtRf2_Step1_Dwell;
        private Panel panel15;
        private TextBox txtRf2_Step2_rTime;
        private TextBox txtRf2_Step1_rTime;
        private Label label54;
        private Label label55;
        private GroupBox grpRf1;
        private GroupBox groupBox2;
        private TableLayoutPanel tableLayoutPanel7;
        private Label label42;
        private Label label60;
        private Label label61;
        private TableLayoutPanel tableLayoutPanel9;
        private Label lblID3_Title_Ar;
        private Label lblID3_ActualFlowValue;
        private NumericUpDown numID3_SetPointWrite;
        private Button btnID3_ValveToggle;
        private Panel panel6;
        private Label label20;
        private TextBox txtRf1_TargetLife;
        private Label label21;
        private Label label22;
        private Panel panel7;
        private Label lblRf1_RefP;
        private Label lblRf1_FwP;
        private Label label25;
        private ProgressBar pbRf1_RefP;
        private Label lbl;
        private ProgressBar pbRf1_FwP;
        private Panel panel8;
        private Panel panel9;
        private Label label14;
        private NumericUpDown numRf1_Step2_SetW;
        private NumericUpDown numRf1_OffRamp;
        private NumericUpDown numRf1_Step2_Dwell;
        private NumericUpDown numRf1_Step2_Ramp;
        private Label lblRf1_OffRamp;
        private Label label28;
        private Label label29;
        private Label label30;
        private Label label31;
        private Label lblRf2_Dwell;
        private Label lblRf2_Ramp;
        private Label label34;
        private Label lblRf2_Set;
        private TextBox txtRf1_Step1_SetW;
        private TextBox txtRf1_Step1_Ramp;
        private TextBox txtRf1_Step1_Dwell;
        private Panel panel10;
        private TextBox txtRf1_Step2_rTime;
        private TextBox txtRf1_Step1_rTime;
        private Label label36;
        private Label label37;
        private GroupBox grpDC;
        private GroupBox groupBox1;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel10;
        private Label lblID5_Title_O2;
        private Label lblID5_ActualFlowValue;
        private NumericUpDown numID5_SetPointWrite;
        private Button btnID5_ValveToggle;
        private Label label65;
        private Label label66;
        private Label label67;
        private TableLayoutPanel tableLayoutPanel11;
        private Label lblID4_Title_Ar;
        private Label lblID4_ActualFlowValue;
        private NumericUpDown numID4_SetPointWrite;
        private Button btnID4_ValveToggle;
        private Panel panel5;
        private Label label17;
        private TextBox txtDC_TargetLife;
        private Label label16;
        private Label label15;
        private Panel panel3;
        private TextBox txtDC_ReadPower;
        private Label label59;
        private Label label58;
        private Label label57;
        private TextBox txtDC_ReadVoltage;
        private TextBox txtDC_ReadCurrent;
        private Panel panel2;
        private Panel panel1;
        private Label label27;
        private Label label26;
        private TextBox txtDC_SetCurrent;
        private NumericUpDown numDC_OffRamp;
        private NumericUpDown numDc_Step2_Dwell;
        private NumericUpDown numDc_Step2_Ramp;
        private Label label13;
        private Label label12;
        private Label label11;
        private TextBox txtDC_SetVoltage;
        private NumericUpDown numDc_Step2_SetW;
        private Label lblDc_OffRamp;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label lblRf1_Dwell;
        private Label lblRf1_Ramp;
        private Label label5;
        private Label lblRf1_Set;
        private TextBox txtDc_Step1_SetW;
        private TextBox txtDc_Step1_Ramp;
        private TextBox txtDc_Step1_Dwell;
        private Panel panel4;
        private TextBox txtDC_Step2_rTime;
        private TextBox txtDC_Step1_rTime;
        private Label label19;
        private Label label18;
        private GroupBox grpStartControls;
        private TableLayoutPanel tlpAnalogFeedback;
        private Label label35;
        private Label lblTurboFrequency;
        private Label label32;
        private Label lblRotationMotor;
        private Label label10;
        private NumericUpDown numGateValvePosition;
        private Label label2;
        private Button btnRotationMotor;
        private NumericUpDown numMotorRpm;
        private TableLayoutPanel tlpStartControls;
        private Button btnRezistans;
        private Button btnMatchBoxPower;
        private Label lblGUC4;
        private Button btnDCShutter;
        private Label lblLeftFrontShutter;
        private Button btnDCPower;
        private Label lblGUC3;
        private Button btnRF2Power;
        private Label lblGUC2;
        private Button btnRF2Shutter;
        private Label lblRightRearShutter;
        private Button btnRF1Shutter;
        private Label lblRightFrontShutter;
        private Button btnRF1Power;
        private Label lblGUC1;
        private Button btnChamber;
        private Label lblChamberValve;
        private Button btnTurboVacuum;
        private Label lblTurbo_EVP_Vacuum;
        private Button btnChiller;
        private Label lblChiller;
        private Button btnScrollVacuum;
        private Label lblScrewVacuum;
        private Panel panel16;
        private Label label1;
        private ComboBox cmbTorUnit;
        private Label lblTorDeger;
        private Label lblTor;
        private Panel panelTorMini;
        private TabPage tpGraph;
        private Panel panelTorque;
        private Button btnControlRf2;
        private Button btnControlRf1;
        private Button btnControlDC;
        private TabPage tpQCM;
        private TabPage tpLogPage;
        private GroupBox grpProcessList;
        private DataGridView dgvProcesses;
        private Panel pnlProcessButtons;
        private Button btnProcDelete;
        private Button btnProcEdit;
        private Button btnProcAdd;
        private GroupBox grpMaterialList;
        private DataGridView dgvMaterials;
        private Panel pnlMaterialButtons;
        private Button btnMatDelete;
        private Button btnMatEdit;
        private Button btnMatAdd;
        private GroupBox grpLayerList;
        private DataGridView dgvLayers;
        private Panel pnlLayerButtons;
        private Button btnInsertNormalLayer;
        private Button btnDeleteLayer;
        private Button btnEditLayer;
        private Button btnInsertCoDepLayer;
        private GroupBox grpActiveLayerDetail;
        private TableLayoutPanel tlpLayerDetail;
        private Label label3;
        private Label lblLdStartMode;
        private Label lblLdSource;
        private Label lblLdFinalThk;
        private Label lblLdTooling;
        private Label lblLdZRatio;
        private Label lblLdDensity;
        private Label label4;
        private Label label23;
        private Label label24;
        private Label label33;
        private Label label41;
        private Label label62;
        private Label lblLdMaterial;
        private DataGridViewTextBoxColumn colProcessNum;
        private DataGridViewTextBoxColumn colProcName;
        private DataGridViewTextBoxColumn colProcMaterialNumber;
        private DataGridViewTextBoxColumn colProcTooling;
        private GroupBox groupBox4;
        private GroupBox gbQcmSensorInfo;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label89;
        private Label label90;
        private Label label91;
        private Label lblValue_Rf1Rate;
        private Label label70;
        private Label label69;
        private Label label75;
        private Label label74;
        private Button btnStartLayer;
        private Button btnZeroThickness;
        private Button btnNextLayer;
        private DataGridViewTextBoxColumn colLayerInfo;
        private DataGridViewTextBoxColumn colProcessNumber;
        private DataGridViewTextBoxColumn colProcessName;
        private DataGridViewTextBoxColumn colFinalThickness;
        private DataGridViewTextBoxColumn colSource;
        private DataGridViewTextBoxColumn colRF1Sensor;
        private DataGridViewTextBoxColumn colRF2Sensor;
        private DataGridViewTextBoxColumn colDCSensor;
        private DataGridViewTextBoxColumn colStartMode;
        private TableLayoutPanel tlpCrystalLife1;
        private Label lblValue_DcCrystalLife;
        private Label lblValue_Rf2CrystalLife;
        private Label label87;
        private Label label88;
        private Label label92;
        private Label lblValue_Rf1CrystalLife;
        private Label lblValue_DcThickness;
        private Label lblValue_DcRate;
        private Label lblValue_Rf2Thickness;
        private Label lblValue_Rf2Rate;
        private Label lblValue_Rf1Thickness;
        private Label label78;
        private Label lblLd;
        private ComboBox cmbLayerDetailTitle;
        private DataGridViewTextBoxColumn colMatNumber;
        private DataGridViewTextBoxColumn colMatName;
        private DataGridViewTextBoxColumn colMatDensity;
        private DataGridViewTextBoxColumn colMatZ;
        private CheckBox chkEvtAutoScroll;
        private Button btnEvtExport;
        private TextBox txtEvtSearch;
        private ComboBox cmbEvtTimeRange;
        private ComboBox cmbEvtSource;
        private ComboBox cmbEvtLevel;
        private Label lblEvtStats;
        private DataGridView dgvEvents;
        private Panel panelEventsTop;
        private Panel panelEventsBottom;
        private DateTimePicker dtpEvtTo;
        private DateTimePicker dtpEvtFrom;
        private Button btnEvtApplyRange;
        private Label label64;
        private Label label63;
        private Panel panelRate;
        private Panel panelThickness;
        private Label label76;
        private Panel panel19;
        private Label label68;
        private Button btnTorLive;
        private Button btnTorAll;
        private Button btnTorFreeze;
        private Label lblTorFrozen;
        private CheckBox chkTorLog;
        private Label lblRateFrozen;
        private Button btnRateFreeze;
        private Button btnRateAll;
        private Button btnRateLive;
        private Label lblThicknessFrozen;
        private Button btnThicknessFreeze;
        private Button btnThicknessAll;
        private Button btnThicknessLive;
        private Label label77;
        private Button button1;
        private Button button2;
        private Button button3;
        private TabPage tabPage1;
        private ListBox lstLog;
        private Label label79;
        private Label label80;
        private NumericUpDown numRf2_Step2_Dwell;
        private NumericUpDown numRf2_Step2_Ramp;
        private NumericUpDown numRf2_OffRamp;
        private TableLayoutPanel tableLayoutPanel3;
        private NumericUpDown numRezistanceControl;
        private TextBox rezistanceReadValue;
        private Label label82;
        private Label lblTorDegerAux;
        private Label label83;
        private Label lblRezistans;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colCode;
        private DataGridViewTextBoxColumn colMessage;
        private DataGridViewTextBoxColumn colSourceLog;
        private DataGridViewTextBoxColumn colRegistrationNo;
        private Button button4;
        private Label label84;
        private Button btnSetRezistanceVal;
        private Button btnExport;
        private ToolStripStatusLabel tsStatusQcm;
        private Button btnSelectDcTarget;
        private TextBox txtDC_Target;
        private Button btnSelectRf2Target;
        private TextBox txtRf2_Target;
        private Button btnSelectRf1Target;
        private TextBox txtRf1_Target;
        private GroupBox gbQcmSensorInfo2;
        private Label label97;
        private Label label98;
        private Label label99;
        private Label label100;
        private TableLayoutPanel tableLayoutPanel5;
        private Label lblValue2_Rf2Thickness;
        private Label lblValue2_Rf1Thickness;
        private Label label106;
        private Label lblValue2_Rf1Rate;
        private Label label81;
        private Label label85;
        private Label lblValue2_Rf2Rate;
        private Label label110;
        private Label label111;
        private Label label112;
        private Label label113;
        private Label label114;
        private Label label115;
        private Label label107;
        private Label label108;
        private Label label118;
        private Label label119;
        private Label label120;
        private Label label121;
        private Label lblValue2_DcLife;
        private Label lblValue2_DcThickness;
        private Label label95;
        private Label lblValue2_Rf2Life;
        private Label label93;
        private Label lblValue2_Rf1Life;
        private Label lblValue2_DcRate;
    }
}
