using SputterControlApp.Core.Helpers;
using SputterControlApp.Core.Registers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        // Snapshot sırasında uyarı/dirty bastırmak için
        private bool _snapshotBusy;
        private bool _snapshotApplying;

        // MFC blok cache: aynı snapshot içinde tekrar Modbus atmayalım
        private ushort[]? _mfcBlockRegs;

        // --- Embedded task table gibi ---
        private enum SnapshotTaskId
        {
            StartStates,
            MfcBlockAndUi,
            TorDeger,
            GateValvePosition,
            MotorRpm,
            RezistanceSetValue,
            Rf1Steps,
            Rf2Steps,
            DcBlockAndUi
        }

        private Dictionary<SnapshotTaskId, Func<Task>> _snapshotTasks = new();
        private List<SnapshotTaskId> _snapshotPlan = new();

        private void InitSnapshotReadModule()
        {
            // Tek yerde: task table
            _snapshotTasks = new Dictionary<SnapshotTaskId, Func<Task>>
            {
                [SnapshotTaskId.StartStates] = ReadStartStatesAsync,
                [SnapshotTaskId.MfcBlockAndUi] = ReadMfcBlockAndApplyUiAsync,
                [SnapshotTaskId.TorDeger] = ReadTorDegerAsync,
                [SnapshotTaskId.GateValvePosition] = ReadGateValvePositionAsync,
                [SnapshotTaskId.MotorRpm] = ReadMotorRpmAsync,
                [SnapshotTaskId.RezistanceSetValue] = ReadRezistanceSetValueAsync,
                [SnapshotTaskId.Rf1Steps] = ReadRf1StepsAsync,
                [SnapshotTaskId.Rf2Steps] = ReadRf2StepsAsync,
                [SnapshotTaskId.DcBlockAndUi] = ReadDcBlockAndApplyUiAsync
            };

            // Tek yerde: “bu snapshot’ta neler okunacak?”
            _snapshotPlan = new List<SnapshotTaskId>
            {
                SnapshotTaskId.StartStates,
                SnapshotTaskId.MfcBlockAndUi,
                SnapshotTaskId.TorDeger,
                SnapshotTaskId.GateValvePosition,
                SnapshotTaskId.MotorRpm,
                SnapshotTaskId.RezistanceSetValue,
                SnapshotTaskId.Rf1Steps,
                SnapshotTaskId.Rf2Steps,
                SnapshotTaskId.DcBlockAndUi
            };
        }

        /// <summary>
        /// Tek seferlik “anlık okuma” metodu.
        /// Açılışta 1 kez ya da Yenile butonu ile tetiklenir.
        /// </summary>
        private async Task ReadSnapshotAsync(bool rethrowOnFailure = false)
        {
            if (_snapshotBusy) return;
            if (_modbus?.IsConnected != true) return;

            _snapshotBusy = true;
            _snapshotApplying = true;
            _mfcBlockRegs = null;

            try
            {
                // Task planını sırayla çalıştır
                foreach (var id in _snapshotPlan)
                {
                    if (_snapshotTasks.TryGetValue(id, out var job))
                        await job();
                }

                PostStatus("Refresh completed.");
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
                PostStatus("Refresh failed.");

                if (rethrowOnFailure)
                    throw;
            }
            finally
            {
                _snapshotApplying = false;
                _snapshotBusy = false;
            }
        }

        // --------------------------
        // Button handler: Yenile
        // --------------------------
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (sender is Control c) c.Enabled = false;
            try { await ReadSnapshotAsync(); }
            finally
            {
                if (sender is Control c2) c2.Enabled = true;
            }
        }

        // ==========================
        // TASK IMPLEMENTATIONS
        // ==========================

        // --------------------------
        // Manual start buton durumlarını oku
        // --------------------------
        private async Task ReadStartStatesAsync()
        {
            if (_modbus?.IsConnected != true) return;

            const ushort startAddr = 100;
            const ushort count = 21; // 100..120

            var regs = await _modbus.ReadHoldingRegistersAsync(startAddr, count);

            for (int i = 0; i < count; i++)
            {
                ushort addr = (ushort)(startAddr + i);
                bool isActive = regs[i] != 0;

                ApplyManualStartUi(addr, isActive);
            }
        }
        // --------------------------
        // Helpers: start buton UI uygula
        // --------------------------
        private void ApplyManualStartUi(ushort addr, bool isActive) 
        {
            switch (addr)
            {
                case 100: ApplyStartButtonUi(btnRotationMotor, isActive); break;
                case 101: ApplyStartButtonUi(btnRezistans, isActive); break;

                case 102: ApplyStartButtonUi(btnRF1Power, isActive); break;
                case 103: ApplyStartButtonUi(btnRF2Power, isActive); break;
                case 104: ApplyStartButtonUi(btnDCPower, isActive); break;
                case 105: ApplyStartButtonUi(btnMatchBoxPower, isActive); break;

                case 106: ApplyStartButtonUi(btnScrollVacuum, isActive); break;
                case 107: ApplyStartButtonUi(btnTurboVacuum, isActive); break;
                case 108: ApplyStartButtonUi(btnChiller, isActive); break;

                // 109 kullanılmıyor → break yok (ignore)

                case 110: ApplyStartButtonUi(btnChamber, isActive); break;

                // 111 kullanılmıyor → ignore

                case 112: ApplyStartButtonUi(btnRF1Shutter, isActive); break;
                case 113: ApplyStartButtonUi(btnDCShutter, isActive); break;
                case 114: ApplyStartButtonUi(btnRF2Shutter, isActive); break;

                    // 115..120 kullanılmıyor → ignore
            }
        }
        // --------------------------
        // MFC block oku ve UI uygula
        // --------------------------
        private async Task ReadMfcBlockAndApplyUiAsync()
        {
            if (_modbus?.IsConnected != true) return;

            const ushort baseAddr = (ushort)RegisterId.D1ActualFlow; // 10
            const ushort count = 80; // 10..89

            _mfcBlockRegs = await _modbus.ReadHoldingRegistersAsync(baseAddr, count);

            // Helpers
            int Idx(ushort addr) => addr - baseAddr;

            float ReadF32(ushort addr)
            {
                int i = Idx(addr);
                return ModbusTypeExtensions.ToFloat32_CDAB(_mfcBlockRegs[i], _mfcBlockRegs[i + 1]);
            }

            short ReadI16(ushort addr)
            {
                int i = Idx(addr);
                return unchecked((short)_mfcBlockRegs[i]);
            }

            // ---- SetPointRead (float32) ----
            // numID2_SetPointRead yok → numID2_SetPointWrite kullanılacak (senin istediğin gibi)
            float sp1 = ReadF32((ushort)RegisterId.D1ReadSetPoint);
            float sp2 = ReadF32((ushort)RegisterId.D2WriteSetPoint);
            float sp3 = ReadF32((ushort)RegisterId.D3ReadSetPoint);
            float sp4 = ReadF32((ushort)RegisterId.D4ReadSetPoint);
            float sp5 = ReadF32((ushort)RegisterId.D5ReadSetPoint);

            numID1_SetPointWrite.Value = ClampToNud(numID1_SetPointWrite, sp1);
            numID2_SetPointWrite.Value = ClampToNud(numID2_SetPointWrite, sp2);
            numID3_SetPointWrite.Value = ClampToNud(numID3_SetPointWrite, sp3);
            numID4_SetPointWrite.Value = ClampToNud(numID4_SetPointWrite, sp4);
            numID5_SetPointWrite.Value = ClampToNud(numID5_SetPointWrite, sp5);

            // ---- Valve toggle (int16) ----
            short v1 = ReadI16((ushort)RegisterId.D1ReadValvePosition);
            short v2 = ReadI16((ushort)RegisterId.D2ReadValvePosition); // D2 için böyle tanımlı
            short v3 = ReadI16((ushort)RegisterId.D3ReadValvePosition);
            short v4 = ReadI16((ushort)RegisterId.D4ReadValvePosition);
            short v5 = ReadI16((ushort)RegisterId.D5ReadValvePosition);

            ApplyValveUi(btnID1_ValveToggle, v1);
            ApplyValveUi(btnID2_ValveToggle, v2);
            ApplyValveUi(btnID3_ValveToggle, v3);
            ApplyValveUi(btnID4_ValveToggle, v4);
            ApplyValveUi(btnID5_ValveToggle, v5);
        }
        // --------------------------
        // Diğer okuma görevleri
        // --------------------------
        private Task ReadTorDegerAsync()
            => PollTorAsync(); // zaten UI conversion var
        // --------------------------
        // Gate valve position oku
        // --------------------------
        private async Task ReadGateValvePositionAsync()
        {
            if (_modbus?.IsConnected != true) return;

            var r = await _modbus.ReadHoldingRegistersAsync((ushort)RegisterId.GateValve_Position, 1);
            short gv = unchecked((short)r[0]);

            // 0..100 bekleniyor 
            numGateValvePosition.Value = ClampToNud(numGateValvePosition, gv);
        }
        // --------------------------
        // Motor RPM oku
        // --------------------------
        private async Task ReadMotorRpmAsync()
        {
            if (_modbus?.IsConnected != true) return;

            var r = await _modbus.ReadHoldingRegistersAsync((ushort)RegisterId.RotationMotor_Rpm, 1);
            short rpm = unchecked((short)r[0]);

            // UI sınırları designer'dan gelsin
            numMotorRpm.Value = ClampToNud(numMotorRpm, rpm);
        }
        private async Task ReadRezistanceSetValueAsync()
        {
            if (_modbus?.IsConnected != true) return;

            // D292 = Ordel_SetValue (senin yazdığın register)
            var r = await _modbus.ReadHoldingRegistersAsync((ushort)RegisterId.Ordel_SetValue, 1);
            short raw = unchecked((short)r[0]);

            decimal ui = raw / 10m;

            // Eğer ValueChanged'da "pending" yakıyorsan, snapshot sırasında tetiklenmesin:
            try
            {
                numRezistanceControl.Value = ClampToNud(numRezistanceControl, (short)ui);
            }
            finally
            {
            }
        }
        // --------------------------
        // RF1 adımlarını oku
        // --------------------------
        private async Task ReadRf1StepsAsync()
        {
            if (_modbus?.IsConnected != true) return;

            // RF1_Step1_SetW (base) -> Step1 Ramp, Step1 Dwell, Step2 SetW, Step2 Ramp, Step2 Dwell, OffRamp
            var regs = await _modbus.ReadHoldingRegistersAsync(
                (ushort)RegisterId.RF1_Step1_SetW, 10);

            // STEP 1 (TextBox)
            txtRf1_Step1_SetW.Text = regs[0].ToString();
            txtRf1_Step1_Ramp.Text = regs[1].ToString();
            txtRf1_Step1_Dwell.Text = regs[2].ToString();

            // STEP 2 (NumericUpDown)
            numRf1_Step2_SetW.Value = ClampToNud(numRf1_Step2_SetW, unchecked((short)regs[3]));
            numRf1_Step2_Ramp.Value = ClampToNud(numRf1_Step2_Ramp, unchecked((short)regs[4]));
            numRf1_Step2_Dwell.Value = ClampToNud(numRf1_Step2_Dwell, unchecked((short)regs[5]));

            // OffRamp
            numRf1_OffRamp.Value = ClampToNud(numRf1_OffRamp, unchecked((short)regs[6]));

            short RfState = unchecked((short)regs[9]);
            ApplyRf1ButtonUi(RfState == 1 ? true : RfState == 0 ? false : (bool?)null);
        }
        // --------------------------
        // RF2 adımlarını oku
        // --------------------------
        private async Task ReadRf2StepsAsync()
        {
            if (_modbus?.IsConnected != true) return;

            // RF2_Step1_SetW (base) -> Step1 Ramp, Step1 Dwell, Step2 SetW, Step2 Ramp, Step2 Dwell, OffRamp
            var regs = await _modbus.ReadHoldingRegistersAsync(
                (ushort)RegisterId.RF2_Step1_SetW, 10);

            // STEP 1 (TextBox)
            txtRf2_Step1_SetW.Text = regs[0].ToString();
            txtRf2_Step1_Ramp.Text = regs[1].ToString();
            txtRf2_Step1_Dwell.Text = regs[2].ToString();

            // STEP 2 (NumericUpDown)
            numRf2_Step2_SetW.Value = ClampToNud(numRf2_Step2_SetW, unchecked((short)regs[3]));
            numRf2_Step2_Ramp.Value = ClampToNud(numRf2_Step2_Ramp, unchecked((short)regs[4]));
            numRf2_Step2_Dwell.Value = ClampToNud(numRf2_Step2_Dwell, unchecked((short)regs[5]));

            // OffRamp
            numRf2_OffRamp.Value = ClampToNud(numRf2_OffRamp, unchecked((short)regs[6]));

            short RfState = unchecked((short)regs[9]);
            ApplyRf2ButtonUi(RfState == 1 ? true : RfState == 0 ? false : (bool?)null);
        }
        // --------------------------
        // DC blok oku ve UI uygula
        // --------------------------
        private async Task ReadDcBlockAndApplyUiAsync()
        {
            if (_modbus?.IsConnected != true) return;

            // 1) DC params 0..9 (10 reg)
            var dcParams = await _modbus.ReadHoldingRegistersAsync(
                (ushort)RegisterId.DC_Control_Command, 10);

            // Step1 (TextBox)
            txtDc_Step1_SetW.Text = dcParams[1].ToString();
            txtDc_Step1_Ramp.Text = dcParams[2].ToString();
            txtDc_Step1_Dwell.Text = dcParams[3].ToString();

            // Step2 (NumericUpDown)
            numDc_Step2_SetW.Value = ClampToNud(numDc_Step2_SetW, unchecked((short)dcParams[4]));
            numDc_Step2_Ramp.Value = ClampToNud(numDc_Step2_Ramp, unchecked((short)dcParams[5]));
            numDc_Step2_Dwell.Value = ClampToNud(numDc_Step2_Dwell, unchecked((short)dcParams[6]));

            // OffRamp (NumericUpDown)
            numDC_OffRamp.Value = ClampToNud(numDC_OffRamp, unchecked((short)dcParams[7]));

            // DC set V/I (TextBox)
            txtDC_SetVoltage.Text = dcParams[8].ToString();
            txtDC_SetCurrent.Text = dcParams[9].ToString();

            // 2) DC readback 163..165 (V, I, P)
            var dcVip = await _modbus.ReadHoldingRegistersAsync(
                (ushort)RegisterId.DC_Read_Voltage, 3);

            short voltage = unchecked((short)dcVip[0]);
            short current = unchecked((short)dcVip[1]);
            short power = unchecked((short)dcVip[2]);

            txtDC_ReadVoltage.Text = voltage.ToString();
            txtDC_ReadCurrent.Text = current.ToString();
            txtDC_ReadPower.Text = power.ToString();

            // 3) DC button state 222 (btnControlDC feedback)
            var dcStateReg = await _modbus.ReadHoldingRegistersAsync(
                (ushort)RegisterId.DC_Set_Out, 1);
            // 0=off, 1=on, else=indeterminate
            short dcState = unchecked((short)dcStateReg[0]);
            // Apply
            ApplyDcButtonUi(dcState == 1 ? true : dcState == 0 ? false : (bool?)null);
        }
    }
}
