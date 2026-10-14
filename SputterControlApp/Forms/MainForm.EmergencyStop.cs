using SputterControlApp.Core.Registers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm
    {
        private short? _emergencyStopLastStatus;
        private volatile bool _emergencyStopActive;
        private int _emergencyDialogShown; // 0/1 gate

        private async Task PollEmergencyStopAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;

            try
            {
                var regs = await _modbus.ReadHoldingRegistersAsync((ushort)RegisterId.EmergencyStop, 1);
                short current = unchecked((short)regs[0]);

                if (_emergencyStopLastStatus != current)
                    UiLog($"[EMERGENCY] D350 changed: {current}");

                // Edge detect
                bool risingEdge = (_emergencyStopLastStatus != 1 && current == 1);
                bool fallingEdge = (_emergencyStopLastStatus == 1 && current == 0);

                _emergencyStopLastStatus = current;

                if (current == 1)
                {
                    _emergencyStopActive = true;

                    if (risingEdge)
                        ShowEmergencyOnce();
                }
                else if (current == 0)
                {
                    _emergencyStopActive = false;

                    if (fallingEdge)
                        Interlocked.Exchange(ref _emergencyDialogShown, 0);
                }
            }
            catch (Exception ex)
            {
                UiLog($"[EMERGENCY] D350 read error: {ex.Message}");
            }
        }

        private void ShowEmergencyOnce()
        {
            if (Interlocked.Exchange(ref _emergencyDialogShown, 1) != 0)
                return;

            // UI thread'de göster
            if (IsHandleCreated)
            {
                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(
                        this,
                        "Emergency Stop Active!",
                        "Emergency Stop",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }));
            }
        }
    }
}
