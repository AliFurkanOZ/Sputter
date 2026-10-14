using SputterControlApp.Core.Registers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterControlApp
{
    public partial class MainForm
    {
        private readonly SemaphoreSlim _deviceStatusPollGate = new(1, 1);

        private short? _rf1LastStatus;
        private short? _rf2LastStatus;
        private short? _dcLastStatus;

        private void WireDeviceControlButtons()
        {
            btnControlRf1.Click -= btnControlRf1_Click;
            btnControlRf1.Click += btnControlRf1_Click;

            btnControlRf2.Click -= btnControlRf2_Click;
            btnControlRf2.Click += btnControlRf2_Click;

            btnControlDC.Click -= btnControlDC_Click;
            btnControlDC.Click += btnControlDC_Click;
        }

        private async void btnControlRf1_Click(object? sender, EventArgs e)
            => await SendDeviceCommandOptimisticAsync("RF1", (ushort)RegisterId.RF1_Control_Command, _rf1LastStatus, ApplyRf1ButtonUi);

        private async void btnControlRf2_Click(object? sender, EventArgs e)
            => await SendDeviceCommandOptimisticAsync("RF2", (ushort)RegisterId.RF2_Control_Command, _rf2LastStatus, ApplyRf2ButtonUi);

        private async void btnControlDC_Click(object? sender, EventArgs e)
            => await SendDeviceCommandOptimisticAsync("DC", (ushort)RegisterId.DC_Control_Command, _dcLastStatus, ApplyDcButtonUi);

        private async Task SendDeviceCommandOptimisticAsync(string deviceName, ushort commandAddress, short? lastStatus, Action<bool?> applyUi)
        {
            if (_modbus?.IsConnected != true)
            {
                UiLog($"[{deviceName}] command ignored: not connected");
                return;
            }

            ushort command;
            if (lastStatus == 1) command = 0;
            else if (lastStatus == 0) command = 1;
            else
            {
                command = 0; // safe default
                UiLog($"[{deviceName}] unknown last status -> default STOP command");
            }

            try
            {
                await _modbus.WriteSingleRegisterAsync(commandAddress, command);
                if (InvokeRequired) BeginInvoke(new Action(() => applyUi(null)));
                else applyUi(null);

                UiLog($"[{deviceName}] cmd sent: {(command == 1 ? "START" : "STOP")}");
            }
            catch (Exception ex)
            {
                UiLog($"[{deviceName}] command error: {ex.Message}");
            }
        }

        private async Task PollDeviceStatusesAsync()
        {
            if (_manualReadOpen) return;
            if (_modbus?.IsConnected != true) return;
            if (!await _deviceStatusPollGate.WaitAsync(0)) return;

            try
            {
                _rf1LastStatus = await PollSingleDeviceStatusAsync("RF1", (ushort)RegisterId.RF1_Status, _rf1LastStatus, ApplyRf1ButtonUi, StartRf1SequenceIfNotRunning, CancelRf1Sequence);
                _rf2LastStatus = await PollSingleDeviceStatusAsync("RF2", (ushort)RegisterId.RF2_Status, _rf2LastStatus, ApplyRf2ButtonUi, StartRf2SequenceIfNotRunning, CancelRf2Sequence);
                _dcLastStatus = await PollSingleDeviceStatusAsync("DC", (ushort)RegisterId.DC_Set_Out, _dcLastStatus, ApplyDcButtonUi, StartDcSequenceIfNotRunning, CancelDcSequence);
            }
            finally
            {
                _deviceStatusPollGate.Release();
            }
        }


        private async Task<short?> PollSingleDeviceStatusAsync(
            string deviceName,
            ushort statusAddress,
            short? lastStatus,
            Action<bool?> applyUi,
            Action? onOnTransition,
            Action? onOffTransition)
        {
            try
            {
                var regs = await _modbus!.ReadHoldingRegistersAsync(statusAddress, 1);
                short current = unchecked((short)regs[0]);
                if (lastStatus == current) return lastStatus;

                if (current == 1)
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            applyUi(true);
                            onOnTransition?.Invoke();
                        }));
                    }
                    else
                    {
                        applyUi(true);
                        onOnTransition?.Invoke();
                    }
                }
                else if (current == 0)
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            applyUi(false);
                            onOffTransition?.Invoke();
                        }));
                    }
                    else
                    {
                        applyUi(false);
                        onOffTransition?.Invoke();
                    }
                }
                else
                {
                    if (InvokeRequired) BeginInvoke(new Action(() => applyUi(null)));
                    else applyUi(null);
                }

                UiLog($"[{deviceName}] status changed: {current}");
                return current;
            }
            catch (Exception ex)
            {
                UiLog($"[{deviceName}] status read error: {ex.Message}");
                return lastStatus;
            }
        }


    }
}
