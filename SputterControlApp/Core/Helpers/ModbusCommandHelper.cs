using System;
using System.Threading;
using System.Threading.Tasks;
using SputterControlApp.Core.Services;

namespace SputterControlApp.Core.Helpers
{
    public static class ModbusCommandHelper
    {
        /// <summary>
        /// Writes a single register and confirms by reading back the same address until it matches or timeouts.
        /// retries: number of write attempts
        /// timeoutMs: confirm window per attempt
        /// </summary>
        public static async Task<(bool confirmed, ushort lastRead)> WriteSingleRegisterAndConfirmAsync(
            IModbusClientService modbus,
            ushort writeAddress,
            ushort targetValue,
            ushort? readAddress = null,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200,
            CancellationToken ct = default)
        {
            if (modbus is null) throw new ArgumentNullException(nameof(modbus));

            ushort rAddr = readAddress ?? writeAddress;
            ushort last = 0;

            for (int attempt = 1; attempt <= retries; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                // 1) Write
                await modbus.WriteSingleRegisterAsync(writeAddress, targetValue, ct);

                // 2) Confirm loop (read-back possibly from a DIFFERENT address)
                long deadline = Environment.TickCount64 + timeoutMs;

                while (Environment.TickCount64 < deadline)
                {
                    ct.ThrowIfCancellationRequested();

                    var rb = await modbus.ReadHoldingRegistersAsync(rAddr, 1, ct);
                    last = rb[0];

                    if (last == targetValue)
                        return (true, last);

                    await Task.Delay(pollDelayMs, ct);
                }

                if (attempt < retries)
                    await Task.Delay(retryDelayMs, ct);
            }

            return (false, last);
        }



        public static async Task<(bool confirmed, ushort[] lastRead)> WriteRegistersAndConfirmAsync(
            IModbusClientService modbus,
            ushort writeAddress,
            ushort[] writeValues,
            ushort readAddress,
            ushort readCount,
            Func<ushort[], bool> isConfirmed,
            int retries = 3,
            int timeoutMs = 1500,
            int pollDelayMs = 80,
            int retryDelayMs = 200,
            CancellationToken ct = default)
        {
            if (modbus is null) throw new ArgumentNullException(nameof(modbus));
            if (writeValues is null) throw new ArgumentNullException(nameof(writeValues));
            if (readCount == 0) throw new ArgumentOutOfRangeException(nameof(readCount));

            ushort[] last = new ushort[readCount];

            for (int attempt = 1; attempt <= retries; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                await modbus.WriteMultipleRegistersAsync(writeAddress, writeValues, ct);

                long deadline = Environment.TickCount64 + timeoutMs;

                while (Environment.TickCount64 < deadline)
                {
                    ct.ThrowIfCancellationRequested();

                    var rb = await modbus.ReadHoldingRegistersAsync(readAddress, readCount, ct);
                    last = rb;

                    if (isConfirmed(rb))
                        return (true, rb);

                    await Task.Delay(pollDelayMs, ct);
                }

                if (attempt < retries)
                    await Task.Delay(retryDelayMs, ct);
            }

            return (false, last);
        }

    }
}
