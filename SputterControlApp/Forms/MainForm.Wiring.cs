using SputterControlApp.Core.Registers;
using SputterControlApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private bool _rezistanceWriteInProgress;
        private System.Windows.Forms.Timer? _rezistanceDebounceTimer;
        private readonly ToolTip _powerInterlockToolTip = new ToolTip();
        private string _activeInterlockTip = string.Empty;

        // -----------------------------
        //  Wiring Methods
        // -----------------------------
        private void WireToggleButtons()
        {
            if (_modbus == null || _toolsSvc == null) return;
            BindIdxValveToggleButton();
        }
        // -----------------------------
        // SetPoint Handlers
        // -----------------------------
        private void WireSetPointEnterHandlers()
        {
            if (_toolsSvc == null) return;

            SetPointNumeric(1, numID1_SetPointWrite);
            SetPointNumeric(2, numID2_SetPointWrite);
            SetPointNumeric(3, numID3_SetPointWrite);
            SetPointNumeric(4, numID4_SetPointWrite);
            SetPointNumeric(5, numID5_SetPointWrite);
        }
        // -----------------------------
        // bind IDX Valve Toggle Buttons
        // -----------------------------
        private void BindIdxValveToggleButton()
        {
            _toolsSvc!.BindOnOffButton(
                btnID1_ValveToggle,
                writeAddress: (ushort)RegisterId.D1WriteValvePosition,
                readAddress: (ushort)RegisterId.D1ReadValvePosition,
                offValue: 2,          // CLOSED
                onValue: 1,           // OPEN
                offText: "CLOSE",
                onText: "OPEN"
            );

            _toolsSvc!.BindOnOffButton(
                btnID2_ValveToggle,
                writeAddress: (ushort)RegisterId.D2WriteValvePosition,
                readAddress: (ushort)RegisterId.D2WriteValvePosition,
                offValue: 2,          // CLOSED
                onValue: 1,           // OPEN
                offText: "CLOSE",
                onText: "OPEN"
            );

            _toolsSvc!.BindOnOffButton(
                btnID3_ValveToggle,
                writeAddress: (ushort)RegisterId.D3WriteValvePosition,
                readAddress: (ushort)RegisterId.D3ReadValvePosition,
                offValue: 2,          // CLOSED
                onValue: 1,           // OPEN
                offText: "CLOSE",
                onText: "OPEN"
            );

            _toolsSvc!.BindOnOffButton(
                btnID4_ValveToggle,
                writeAddress: (ushort)RegisterId.D4WriteValvePosition,
                readAddress: (ushort)RegisterId.D4ReadValvePosition,
                offValue: 2,          // CLOSED
                onValue: 1,           // OPEN
                offText: "CLOSE",
                onText: "OPEN"
            );

            _toolsSvc!.BindOnOffButton(
                btnID5_ValveToggle,
                writeAddress: (ushort)RegisterId.D5WriteValvePosition,
                readAddress: (ushort)RegisterId.D5ReadValvePosition,
                offValue: 2,          // CLOSED
                onValue: 1,           // OPEN
                offText: "CLOSE",
                onText: "OPEN"
            );
        }

        // -----------------------------
        // Manual Start Buttons
        // -----------------------------
        private void WireManualStartButtons()
        {
            // MANUAL CONTROL START COMMANDS (100–114)
            _toolsSvc!.BindOnOffButton(
                btnRotationMotor, 100, 100, 0, 1, "STOP", "START",
                onOnPressed_NoConfirm: async () =>
                {
                    ushort rpm = (ushort)numMotorRpm.Value;
                    await _modbus!.WriteSingleRegisterAsync((ushort)RegisterId.RotationMotor_Rpm, rpm);
                }
            );

            _toolsSvc!.BindOnOffButton(btnRezistans, 101, 101, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnRF1Power, 102, 102, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnRF2Power, 103, 103, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnDCPower, 104, 104, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnMatchBoxPower, 105, 105, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnScrollVacuum, 106, 106, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnTurboVacuum, 107, 107, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnChiller, 108, 108, 0, 1, "STOP", "START");
            // 109 Gatevalve → kullanılmıyor
            _toolsSvc!.BindOnOffButton(btnChamber, 110, 110, 0, 1, "STOP", "START");
            // 111 Soft_Turbo_Valf → kullanılmıyor
            _toolsSvc!.BindOnOffButton(btnRF1Shutter, 112, 112, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnDCShutter, 113, 113, 0, 1, "STOP", "START");
            _toolsSvc!.BindOnOffButton(btnRF2Shutter, 114, 114, 0, 1, "STOP", "START");
            // 115–120 şu an kullanılmıyor

            WirePowerInterlocks();
        }

        private void WirePowerInterlocks()
        {
            btnMatchBoxPower.TextChanged -= PowerButtons_TextChanged;
            btnRF1Power.TextChanged -= PowerButtons_TextChanged;
            btnRF2Power.TextChanged -= PowerButtons_TextChanged;
            btnDCPower.TextChanged -= PowerButtons_TextChanged;

            btnMatchBoxPower.TextChanged += PowerButtons_TextChanged;
            btnRF1Power.TextChanged += PowerButtons_TextChanged;
            btnRF2Power.TextChanged += PowerButtons_TextChanged;
            btnDCPower.TextChanged += PowerButtons_TextChanged;

            tlpStartControls.MouseMove -= TlpStartControls_MouseMove;
            tlpStartControls.MouseMove += TlpStartControls_MouseMove;

            tlpStartControls.MouseLeave -= TlpStartControls_MouseLeave;
            tlpStartControls.MouseLeave += TlpStartControls_MouseLeave;

            UpdatePowerInterlocks();
            WireChillerInterlocks();
        }

        private void WireChillerInterlocks()
        {
            btnChiller.TextChanged -= Chiller_TextChanged;
            btnChiller.TextChanged += Chiller_TextChanged;

            UpdateChillerInterlocks();
        }

        private void Chiller_TextChanged(object? sender, EventArgs e)
            => UpdateChillerInterlocks();

        private void UpdateChillerInterlocks()
        {
            bool isChillerOn = string.Equals(btnChiller.Text, "START", StringComparison.OrdinalIgnoreCase);

            btnTurboVacuum.Enabled = isChillerOn;
            UpdateSourceControlInterlocks(isChillerOn);
            UpdateLayerUiState();

            if (!isChillerOn)
            {
                _powerInterlockToolTip.SetToolTip(btnControlRf1, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnControlRf2, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnControlDC, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnTurboVacuum, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnStartLayer, "Chiller must be START first.");
            }
            else
            {
                _powerInterlockToolTip.SetToolTip(btnControlRf1, string.Empty);
                _powerInterlockToolTip.SetToolTip(btnControlRf2, string.Empty);
                _powerInterlockToolTip.SetToolTip(btnControlDC, string.Empty);
                _powerInterlockToolTip.SetToolTip(btnTurboVacuum, string.Empty);
                _powerInterlockToolTip.SetToolTip(btnStartLayer, string.Empty);
            }
        }

        private void TlpStartControls_MouseLeave(object? sender, EventArgs e)
        {
            _activeInterlockTip = string.Empty;
            _powerInterlockToolTip.Hide(tlpStartControls);
        }

        private void TlpStartControls_MouseMove(object? sender, MouseEventArgs e)
        {
            string tip = string.Empty;

            var rf1Rect = tlpStartControls.RectangleToClient(btnRF1Power.RectangleToScreen(btnRF1Power.ClientRectangle));
            var rf2Rect = tlpStartControls.RectangleToClient(btnRF2Power.RectangleToScreen(btnRF2Power.ClientRectangle));
            var mbRect = tlpStartControls.RectangleToClient(btnMatchBoxPower.RectangleToScreen(btnMatchBoxPower.ClientRectangle));

            if (!btnRF1Power.Enabled && rf1Rect.Contains(e.Location))
                tip = "MatchBox Power must be ON first.";
            else if (!btnRF2Power.Enabled && rf2Rect.Contains(e.Location))
                tip = "MatchBox Power must be ON first.";
            else if (!btnMatchBoxPower.Enabled && mbRect.Contains(e.Location))
                tip = "Turn OFF RF1 and RF2 before turning OFF MatchBox Power.";

            if (tip == _activeInterlockTip) return;

            _activeInterlockTip = tip;
            _powerInterlockToolTip.Hide(tlpStartControls);

            if (!string.IsNullOrEmpty(tip))
                _powerInterlockToolTip.Show(tip, tlpStartControls, e.Location.X + 12, e.Location.Y + 12, 1800);
        }

        private void PowerButtons_TextChanged(object? sender, EventArgs e)
            => UpdatePowerInterlocks();

        private void UpdatePowerInterlocks()
        {
            bool matchBoxOn = string.Equals(btnMatchBoxPower.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool rf1On = string.Equals(btnRF1Power.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool rf2On = string.Equals(btnRF2Power.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool anyRfOn = rf1On || rf2On;

            btnRF1Power.Enabled = matchBoxOn;
            btnRF2Power.Enabled = matchBoxOn;
            UpdateSourceControlInterlocks();

            if (!matchBoxOn)
            {
                _powerInterlockToolTip.SetToolTip(btnRF1Power, "MatchBox Power must be ON first.");
                _powerInterlockToolTip.SetToolTip(btnRF2Power, "MatchBox Power must be ON first.");
            }
            else
            {
                _powerInterlockToolTip.SetToolTip(btnRF1Power, string.Empty);
                _powerInterlockToolTip.SetToolTip(btnRF2Power, string.Empty);
            }

            if (matchBoxOn && anyRfOn)
            {
                btnMatchBoxPower.Enabled = false;
                _powerInterlockToolTip.SetToolTip(btnMatchBoxPower, "Turn OFF RF1 and RF2 before turning OFF MatchBox Power.");
            }
            else
            {
                btnMatchBoxPower.Enabled = true;
                _powerInterlockToolTip.SetToolTip(btnMatchBoxPower, string.Empty);
            }
        }

        private void UpdateSourceControlInterlocks(bool? isChillerOnOverride = null)
        {
            bool isChillerOn = isChillerOnOverride
                ?? string.Equals(btnChiller.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool isRf1PowerOn = string.Equals(btnRF1Power.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool isRf2PowerOn = string.Equals(btnRF2Power.Text, "START", StringComparison.OrdinalIgnoreCase);
            bool isDcPowerOn = string.Equals(btnDCPower.Text, "START", StringComparison.OrdinalIgnoreCase);

            btnControlRf1.Enabled = isChillerOn && isRf1PowerOn;
            btnControlRf2.Enabled = isChillerOn && isRf2PowerOn;
            btnControlDC.Enabled = isChillerOn && isDcPowerOn;

            if (!isChillerOn)
            {
                _powerInterlockToolTip.SetToolTip(btnControlRf1, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnControlRf2, "Chiller must be START first.");
                _powerInterlockToolTip.SetToolTip(btnControlDC, "Chiller must be START first.");
                return;
            }

            _powerInterlockToolTip.SetToolTip(btnControlRf1, isRf1PowerOn ? string.Empty : "RF1 Power must be START first.");
            _powerInterlockToolTip.SetToolTip(btnControlRf2, isRf2PowerOn ? string.Empty : "RF2 Power must be START first.");
            _powerInterlockToolTip.SetToolTip(btnControlDC, isDcPowerOn ? string.Empty : "DC Power must be START first.");
        }

        // -----------------------------
        // 1) Common (Gate Valve, Motor RPM, previews)
        // -----------------------------
        private void WireCommonStepHandlers()
        {
            if (_toolsSvc == null) return;

            // =========================================================
            // GATE VALVE POSITION (NumericUpDown / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16NumericUpDown(
                nud: numGateValvePosition,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.GateValve_Position,
                getReadAddr: () => (ushort)RegisterId.GateValve_Position,
                min: 0, max: 100,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"Gate Valve Position\n{msg}")
            );

            // =========================================================
            // MOTOR RPM (NumericUpDown / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16NumericUpDown(
                nud: numMotorRpm,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RotationMotor_Rpm,
                getReadAddr: () => (ushort)RegisterId.RotationMotor_Rpm,
                min: 0, max: 30,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"Rotation Motor Rpm\n{msg}")
            );

            WireRezistanceControlHandlers();

            // RF1/RF2 CONTROL button uses optimistic command + 1Hz status reconciliation
            WireRf1TimePreviewHandlers();
            WireRf2TimePreviewHandlers();

            // DC CONTROL button uses optimistic command + 1Hz status reconciliation
            WireDcTimePreviewHandlers();
        }

        private void WireRezistanceControlHandlers()
        {
            // Artık timer/debounce yok
            _rezistanceDebounceTimer?.Stop();
            _rezistanceDebounceTimer?.Dispose();
            _rezistanceDebounceTimer = null;

            // İstersen ValueChanged'da sadece “beklemede” göstergesi yap:
            //lblRezPending.Visible = true; gibi.

            btnSetRezistanceVal.Click -= btnSetRezistanceVal_Click;
            btnSetRezistanceVal.Click += btnSetRezistanceVal_Click;
        }

        private async void btnSetRezistanceVal_Click(object? sender, EventArgs e)
        {
            await SafeCommitRezistanceControlAsync();
        }

        private async Task CommitRezistanceControlAsync()
        {
            if (_rezistanceWriteInProgress) return;
            if (_modbus?.IsConnected != true) return;

            _rezistanceWriteInProgress = true;
            numRezistanceControl.Enabled = false;
            btnSetRezistanceVal.Enabled = false;

            try
            {
                short targetRaw = (short)Math.Round(numRezistanceControl.Value * 10m, MidpointRounding.AwayFromZero);
                await _modbus.WriteSingleRegisterAsync((ushort)RegisterId.Ordel_SetValue, unchecked((ushort)targetRaw));

                // İstersen: UiLog($"[REZ] Set -> {numRezistanceControl.Value:0.0} (raw {targetRaw})");
            }
            catch (Exception ex)
            {
                Warn($"Rezistance write failed.\n{ex.Message}");
            }
            finally
            {
                btnSetRezistanceVal.Enabled = true;
                numRezistanceControl.Enabled = true;
                _rezistanceWriteInProgress = false;
            }
        }

        private async Task SafeCommitRezistanceControlAsync()
        {
            try { await CommitRezistanceControlAsync(); }
            catch (Exception ex) { Warn($"Rezistance write failed.\n{ex.Message}"); }
        }

        

        // -----------------------------
        // 2) RF1 Step Bindings
        // -----------------------------
        private void WireRf1StepHandlers()
        {
            if (_toolsSvc == null) return;

            // =========================================================
            // RF1 STEP1 (TextBox / AutoDebounce / STOP iken)
            // =========================================================
            _toolsSvc.BindInt16TextBox(
                tb: txtRf1_Step1_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step1_SetW,
                getReadAddr: () => (ushort)RegisterId.RF1_Step1_SetW,
                min: 0, max: 600,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step1 SetW\n{msg}")
            );
            _toolsSvc.BindInt16TextBox(
                tb: txtRf1_Step1_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step1_Ramp,
                getReadAddr: () => (ushort)RegisterId.RF1_Step1_Ramp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step1 Ramp\n{msg}")
            );
            _toolsSvc.BindInt16TextBox(
                tb: txtRf1_Step1_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step1_Dwell,
                getReadAddr: () => (ushort)RegisterId.RF1_Step1_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step1 Dwell\n{msg}")
            );

            // =========================================================
            // RF1 STEP2 (NumericUpDown / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf1_Step2_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step2_SetW,
                getReadAddr: () => (ushort)RegisterId.RF1_Step2_SetW,
                min: 0, max: 600,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step2 SetW\n{msg}")
            );
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf1_Step2_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step2_Ramp,
                getReadAddr: () => (ushort)RegisterId.RF1_Step2_Ramp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step2 Ramp\n{msg}")
            );
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf1_Step2_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_Step2_Dwell,
                getReadAddr: () => (ushort)RegisterId.RF1_Step2_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 Step2 Dwell\n{msg}")
            );

            // RF1 OffRamp (NumericUpDown)
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf1_OffRamp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF1_OffRamp,
                getReadAddr: () => (ushort)RegisterId.RF1_OffRamp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF1 OffRamp\n{msg}")
            );
        }

        // -----------------------------
        // RF2 Step Bindings
        // -----------------------------
        private void WireRf2StepHandlers()
        {
            if (_toolsSvc == null) return;

            // =========================================================
            // RF2 STEP1 (TextBox / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16TextBox(
                tb: txtRf2_Step1_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step1_SetW,
                getReadAddr: () => (ushort)RegisterId.RF2_Step1_SetW,
                min: 0, max: 600,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step1 SetW\n{msg}")
            );

            _toolsSvc.BindInt16TextBox(
                tb: txtRf2_Step1_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step1_Ramp,
                getReadAddr: () => (ushort)RegisterId.RF2_Step1_Ramp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step1 Ramp\n{msg}")
            );

            _toolsSvc.BindInt16TextBox(
                tb: txtRf2_Step1_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step1_Dwell,
                getReadAddr: () => (ushort)RegisterId.RF2_Step1_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step1 Dwell\n{msg}")
            );

            // =========================================================
            // RF2 STEP2 (NumericUpDown / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf2_Step2_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step2_SetW,
                getReadAddr: () => (ushort)RegisterId.RF2_Step2_SetW,
                min: 0, max: 600,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step2 SetW\n{msg}")
            );

            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf2_Step2_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step2_Ramp,
                getReadAddr: () => (ushort)RegisterId.RF2_Step2_Ramp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step2 Ramp\n{msg}")
            );

            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf2_Step2_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_Step2_Dwell,
                getReadAddr: () => (ushort)RegisterId.RF2_Step2_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 Step2 Dwell\n{msg}")
            );

            // RF2 OffRamp
            _toolsSvc.BindInt16NumericUpDown(
                nud: numRf2_OffRamp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.RF2_OffRamp,
                getReadAddr: () => (ushort)RegisterId.RF2_OffRamp,
                min: 0, max: 5,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"RF2 OffRamp\n{msg}")
            );
        }

        // -----------------------------
        // 3) DC Step Bindings
        // -----------------------------
        private void WireDcStepHandlers()
        {
            if (_toolsSvc == null) return;

            // =========================================================
            // DC STEP1 (TextBox / AutoDebounce / STOP iken)
            // =========================================================
            _toolsSvc.BindInt16TextBox(
                tb: txtDc_Step1_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step1_SetW,
                getReadAddr: () => (ushort)RegisterId.DC_Step1_SetW,
                min: 0, max: 2000,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step1 SetW\n{msg}")
            );
            _toolsSvc.BindInt16TextBox(
                tb: txtDc_Step1_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step1_Ramp,
                getReadAddr: () => (ushort)RegisterId.DC_Step1_Ramp,
                min: 0, max: 10,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step1 Ramp\n{msg}")
            );
            _toolsSvc.BindInt16TextBox(
                tb: txtDc_Step1_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step1_Dwell,
                getReadAddr: () => (ushort)RegisterId.DC_Step1_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step1 Dwell\n{msg}")
            );

            // =========================================================
            // DC STEP2 (NumericUpDown / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16NumericUpDown(
                nud: numDc_Step2_SetW,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step2_SetW,
                getReadAddr: () => (ushort)RegisterId.DC_Step2_SetW,
                min: 0, max: 2000,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step2 SetW\n{msg}")
            );
            _toolsSvc.BindInt16NumericUpDown(
                nud: numDc_Step2_Ramp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step2_Ramp,
                getReadAddr: () => (ushort)RegisterId.DC_Step2_Ramp,
                min: 0, max: 10,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step2 Ramp\n{msg}")
            );
            _toolsSvc.BindInt16NumericUpDown(
                nud: numDc_Step2_Dwell,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Step2_Dwell,
                getReadAddr: () => (ushort)RegisterId.DC_Step2_Dwell,
                min: 0, max: 999,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Step2 Dwell\n{msg}")
            );

            // DC OffRamp (NumericUpDown)
            _toolsSvc.BindInt16NumericUpDown(
                nud: numDC_OffRamp,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_OffRamp,
                getReadAddr: () => (ushort)RegisterId.DC_OffRamp,
                min: 0, max: 10,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC OffRamp\n{msg}")
            );

            // =========================================================
            // DC LIMITS (TextBox / AutoDebounce)
            // =========================================================
            _toolsSvc.BindInt16TextBox(
                tb: txtDC_SetVoltage,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Set_Voltage,
                getReadAddr: () => (ushort)RegisterId.DC_Set_Voltage,
                min: 0, max: 1000,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Set Voltage\n{msg}")
            );
            _toolsSvc.BindInt16TextBox(
                tb: txtDC_SetCurrent,
                ep: _ep,
                getWriteAddr: () => (ushort)RegisterId.DC_Set_Current,
                getReadAddr: () => (ushort)RegisterId.DC_Set_Current,
                min: 0, max: 1000,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => Warn($"DC Set Current\n{msg}")
            );
        }
        // -----------------------------
        // SetPoint Helpers
        // -----------------------------
        private static (ushort writeAddr, ushort readAddr) GetSetPointAddresses(int id)
        {
            return id switch
            {
                1 => ((ushort)RegisterId.D1WriteSetPoint, (ushort)RegisterId.D1ReadSetPoint),
                2 => ((ushort)RegisterId.D2WriteSetPoint, (ushort)RegisterId.D2WriteSetPoint),
                3 => ((ushort)RegisterId.D3WriteSetPoint, (ushort)RegisterId.D3ReadSetPoint),
                4 => ((ushort)RegisterId.D4WriteSetPoint, (ushort)RegisterId.D4ReadSetPoint),
                5 => ((ushort)RegisterId.D5WriteSetPoint, (ushort)RegisterId.D5ReadSetPoint),
                _ => throw new ArgumentOutOfRangeException(nameof(id))
            };
        }
        private void SetPointNumeric(int id, NumericUpDown nud)
        {
            if (_toolsSvc == null) return;

            (ushort writeAddr, ushort readAddr) GetAddrs() => GetSetPointAddresses(id);

            _toolsSvc.BindFloat32NumericUpDown(
                nud: nud,
                ep: _ep,
                getWriteAddr: () => GetAddrs().writeAddr,
                getReadAddr: () => GetAddrs().readAddr,
                min: (float)nud.Minimum,
                max: (float)nud.Maximum,
                mode: SetableToolsService.CommitMode.AutoDebounce,
                warn: msg => MessageBox.Show(
                    $"ID{id} SetPoint:\n{msg}",
                    "Command Not Confirmed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ),
                retries: 3,
                timeoutMs: 1500,
                pollDelayMs: 80,
                retryDelayMs: 200
            );
        }

    }
}
