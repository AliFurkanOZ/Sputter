using SputterControlApp.Core.Models;

namespace SputterControlApp.Core.Services
{
    public interface IModbusClientService : IDisposable
    {
        bool IsConnected { get; }

        ConnectionSettings? CurrentSettings { get; }

        // Connect/Disconnect
        Task ConnectAsync(ConnectionSettings settings, CancellationToken ct = default);
        void Disconnect();

        // Basit read/write API (sonra genişletiriz)
        Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints, CancellationToken ct = default);
        Task WriteSingleRegisterAsync(ushort address, ushort value, CancellationToken ct = default);
        Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] data, CancellationToken ct = default);
    }
}
