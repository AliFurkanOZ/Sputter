using SputterControlApp.Core.Helpers;
using SputterControlApp.Core.Registers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        // -----------------------------
        // 250ms: ActualFlow (ID1..ID5)
        // -----------------------------
        private async Task PollMfcAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            const ushort baseAddr = (ushort)RegisterId.D1ActualFlow; // 10
            const ushort count = 80; // 10..89 => 89-10+1 = 80 register

            try
            {
                // ✅ Tek Modbus okuması
                var regs = await _modbus.ReadHoldingRegistersAsync(baseAddr, count);

                // Helper: baseAddr'a göre index hesapla
                int Idx(ushort addr) => addr - baseAddr;

                // Helper: CDAB float oku (2 reg)
                float ReadF32(ushort addr)
                {
                    int i = Idx(addr);
                    return ModbusTypeExtensions.ToFloat32_CDAB(regs[i], regs[i + 1]);
                }

                // Helper: int16 oku (1 reg)
                short ReadI16(ushort addr)
                {
                    int i = Idx(addr);
                    return unchecked((short)regs[i]);
                }

                // ---- ID1..ID5 ActualFlow (float) ----
                float f1 = ReadF32((ushort)RegisterId.D1ActualFlow);
                float f2 = ReadF32((ushort)RegisterId.D2ActualFlow);
                float f3 = ReadF32((ushort)RegisterId.D3ActualFlow);
                float f4 = ReadF32((ushort)RegisterId.D4ActualFlow);
                float f5 = ReadF32((ushort)RegisterId.D5ActualFlow);

                lblID1_ActualFlowValue.Text = f1.ToString("0.###");
                lblID2_ActualFlowValue.Text = f2.ToString("0.###");
                lblID3_ActualFlowValue.Text = f3.ToString("0.###");
                lblID4_ActualFlowValue.Text = f4.ToString("0.###");
                lblID5_ActualFlowValue.Text = f5.ToString("0.###");

                // ---- SetPoint (float) ----
                /*
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
                */

                // ---- Valve toggle (int16) ----
                /*
                short v1 = ReadI16((ushort)RegisterId.D1ReadValvePosition);
                short v2 = ReadI16((ushort)RegisterId.D2WriteValvePosition);
                short v3 = ReadI16((ushort)RegisterId.D3ReadValvePosition);
                short v4 = ReadI16((ushort)RegisterId.D4ReadValvePosition);
                short v5 = ReadI16((ushort)RegisterId.D5ReadValvePosition);

                ApplyValveUi(btnID1_ValveToggle, v1);
                ApplyValveUi(btnID2_ValveToggle, v2);
                ApplyValveUi(btnID3_ValveToggle, v3);
                ApplyValveUi(btnID4_ValveToggle, v4);
                ApplyValveUi(btnID5_ValveToggle, v5);
                */
            }
            catch
            {
                lblID1_ActualFlowValue.Text = "--";
                lblID2_ActualFlowValue.Text = "--";
                lblID3_ActualFlowValue.Text = "--";
                lblID4_ActualFlowValue.Text = "--";
                lblID5_ActualFlowValue.Text = "--";
            }
        }



        // -----------------------------
        // 500ms: TOR
        // -----------------------------
        private float? _lastTorTorr;
        private float? _lastTorAuxTorr;
        private async Task PollTorAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                // TorDegerAux’tan başlayarak 6 register okuyoruz
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.TorDegerAux, 6);

                // ✅ AUX: regs[0]-regs[1] = TorDegerAux (ham Torr)
                float torAuxTorr = ModbusTypeExtensions.ToFloat32_CDAB(regs[0], regs[1]);
                _lastTorAuxTorr = torAuxTorr;

                // ✅ sadece 10 üzeri eksi gösterim (üst simge)
                lblTorDegerAux.Text = FormatSci(torAuxTorr, significantDigits: 4);

                // ✅ Ana TOR: regs[4]-regs[5] = (senin mevcut mantığın)
                float tor = ModbusTypeExtensions.ToFloat32_CDAB(regs[4], regs[5]);
                _lastTorTorr = tor;
                UpdateTorUIFromRawTorr(tor);

                TorPlot_AddPoint(DateTime.Now, tor, showLastSeconds: 120);
                TorMiniSparkline_Refresh(lastSeconds: 15, logVisual: true);
            }
            catch
            {
                _lastTorTorr = null;
                _lastTorAuxTorr = null;

                lblTorDeger.Text = "--";
                lblTorDegerAux.Text = "--";
            }
        }

        private async Task PollTurboAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.Turbo_Frequency, 1);

                int v = unchecked((short)regs[0] / 10);
                short turboFrequency = (short)v;
                lblTurboFrequency.Text = turboFrequency.ToString();
            }
            catch
            {
                lblTurboFrequency.Text = "--";
            }
        }

        private async Task PollRF1Async()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.RF1_FwP, 2);

                short fwP = unchecked((short)regs[0]);
                short refP = unchecked((short)regs[1]);

                lblRf1_FwP.Text = fwP.ToString();
                lblRf1_RefP.Text = refP.ToString();

                pbRf1_FwP.Value = unchecked((short)regs[0]);
                pbRf1_RefP.Value = unchecked((short)regs[1]);

                if (_rf1LastStatus == 1 && !string.IsNullOrWhiteSpace(txtRf1_Target.Text))
                {
                    await AddRf1UsageToTargetLifeAsync(fwP);
                }
            }
            catch
            {
                lblRf1_FwP.Text = "--";
                lblRf1_RefP.Text = "--";
            }
        }

        private async Task PollRF2Async()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.RF2_FwP, 2);

                short fwP = unchecked((short)regs[0]);
                short refP = unchecked((short)regs[1]);

                lblRf2_FwP.Text = fwP.ToString();
                lblRf2_RefP.Text = refP.ToString();

                // Eski sürümde olduğu gibi direkt set
                pbRf2_FwP.Value = unchecked((short)regs[0]);
                pbRf2_RefP.Value = unchecked((short)regs[1]);

                if (_rf2LastStatus == 1 && !string.IsNullOrWhiteSpace(txtRf2_Target.Text))
                {
                    await AddRf2UsageToTargetLifeAsync(fwP);
                }
            }
            catch
            {
                lblRf2_FwP.Text = "--";
                lblRf2_RefP.Text = "--";
            }
        }

        private async Task PollDCAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.DC_Read_Voltage, 3);

                short voltage = unchecked((short)regs[0]);
                short current = unchecked((short)regs[1]);
                short power = unchecked((short)regs[2]);

                txtDC_ReadVoltage.Text = voltage.ToString();
                txtDC_ReadCurrent.Text = current.ToString();
                txtDC_ReadPower.Text = power.ToString();

                if (_dcLastStatus == 1 && !string.IsNullOrWhiteSpace(txtDC_Target.Text))
                {
                    await AddDcUsageToTargetLifeAsync(power);
                }

            }
            catch
            {
                txtDC_ReadVoltage.Text = "--";
                txtDC_ReadCurrent.Text = "--";
                txtDC_ReadPower.Text = "--";
            }
        }

        private async Task PollRezistanceAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync(
                    (ushort)RegisterId.Ordel_Instant_Value, 1);

                short readRaw = unchecked((short)regs[0]);
                double resistance = readRaw / 10f;
                rezistanceReadValue.Text = resistance.ToString("0.0");
                TorPlot_AddResistancePoint(DateTime.Now, resistance, showLastSeconds: 120);
            }
            catch
            {
                rezistanceReadValue.Text = "--";
            }
        }




    }
}
