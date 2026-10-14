using System;
using System.Threading;
using System.Threading.Tasks;
using SputterNewApp.Core.Models;

namespace SputterNewApp.Core.Services
{
    public interface IModbusClientService : IDisposable
    {
        bool IsConnected { get; }

        ConnectionSettings? CurrentSettings { get; }

        // Connect/Disconnect
        Task ConnectAsync(ConnectionSettings settings, CancellationToken ct = default);
        void Disconnect();

        // Read/Write API
        Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints, CancellationToken ct = default);
        Task WriteSingleRegisterAsync(ushort address, ushort value, CancellationToken ct = default);
        Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] data, CancellationToken ct = default);
    }
}
