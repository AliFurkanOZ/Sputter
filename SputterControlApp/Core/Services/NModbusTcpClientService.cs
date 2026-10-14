using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using SputterControlApp.Core.Exceptions;
using SputterControlApp.Core.Models;

namespace SputterControlApp.Core.Services
{
    public sealed class NModbusTcpClientService : IModbusClientService
    {
        private TcpClient? _tcpClient;
        private IModbusMaster? _master;
        private readonly object _sync = new();

        private readonly SemaphoreSlim _ioLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _reconnectLock = new SemaphoreSlim(1, 1);

        public bool IsConnected
        {
            get
            {
                lock (_sync)
                {
                    // Uygulama genelinde birçok yerde IsConnected guard'ı var.
                    // Fiziksel socket düştüğünde burada false dönmek, read/write çağrılarını
                    // tamamen durdurup otomatik reconnect akışını tetiklenemez hale getiriyordu.
                    // Bu nedenle "aktif bir bağlantı profili var mı" durumunu döndürüyoruz.
                    return CurrentSettings != null;
                }
            }
        }

        public ConnectionSettings? CurrentSettings { get; private set; }

        public async Task ConnectAsync(ConnectionSettings settings, CancellationToken ct = default)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrWhiteSpace(settings.IpAddress)) throw new ArgumentException("IP boş olamaz.");

            Disconnect();

            var tcp = new TcpClient();
            const int timeoutMs = 2000;

            var connectTask = tcp.ConnectAsync(settings.IpAddress, settings.Port);
            var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs, ct)).ConfigureAwait(false);

            if (completed != connectTask)
            {
                try { tcp.Close(); } catch { }
                throw new TimeoutException($"Bağlantı zaman aşımı ({timeoutMs} ms).");
            }

            await connectTask.ConfigureAwait(false);
            ConfigureTcpClient(tcp);

            var factory = new ModbusFactory();
            var master = factory.CreateMaster(tcp);

            lock (_sync)
            {
                _tcpClient = tcp;
                _master = master;
                CurrentSettings = settings;
            }
        }

        public void Disconnect()
        {
            lock (_sync)
            {
                CurrentSettings = null;

                try { _tcpClient?.Close(); } catch { }

                _master = null;
                _tcpClient = null;
            }
        }

        private async Task<T> WithIoLockAsync<T>(
            Func<IModbusMaster, ConnectionSettings, T> func,
            CancellationToken ct,
            ushort startAddressForError,
            ushort qtyForError)
        {
            await _ioLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ConnectionSettings settings;
                lock (_sync)
                {
                    settings = CurrentSettings ?? throw new InvalidOperationException("Bağlantı ayarları yok.");
                }

                return await ExecuteWithReconnectAsync(
                    master =>
                    {
                        try
                        {
                            return func(master, settings);
                        }
                        catch (SlaveException ex)
                        {
                            throw new ModbusAddressException(startAddressForError, qtyForError, ex);
                        }
                    },
                    ct).ConfigureAwait(false);
            }
            finally
            {
                _ioLock.Release();
            }
        }

        private async Task WithIoLockAsync(
            Action<IModbusMaster, ConnectionSettings> action,
            CancellationToken ct,
            ushort startAddressForError,
            ushort qtyForError)
        {
            await _ioLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ConnectionSettings settings;
                lock (_sync)
                {
                    settings = CurrentSettings ?? throw new InvalidOperationException("Bağlantı ayarları yok.");
                }

                await ExecuteWithReconnectAsync(
                    master =>
                    {
                        try
                        {
                            action(master, settings);
                            return true;
                        }
                        catch (SlaveException ex)
                        {
                            throw new ModbusAddressException(startAddressForError, qtyForError, ex);
                        }
                    },
                    ct).ConfigureAwait(false);
            }
            finally
            {
                _ioLock.Release();
            }
        }

        public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints, CancellationToken ct = default)
            => WithIoLockAsync(
                (master, settings) => master.ReadHoldingRegisters(settings.UnitId, startAddress, numberOfPoints),
                ct,
                startAddress,
                numberOfPoints);

        public Task WriteSingleRegisterAsync(ushort address, ushort value, CancellationToken ct = default)
            => WithIoLockAsync(
                (master, settings) => master.WriteSingleRegister(settings.UnitId, address, value),
                ct,
                address,
                1);

        public Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] data, CancellationToken ct = default)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            return WithIoLockAsync(
                (master, settings) => master.WriteMultipleRegisters(settings.UnitId, startAddress, data),
                ct,
                startAddress,
                (ushort)Math.Min(data.Length, ushort.MaxValue));
        }

        public void Dispose()
        {
            Disconnect();
            _ioLock.Dispose();
            _reconnectLock.Dispose();
        }

        private async Task<T> ExecuteWithReconnectAsync<T>(Func<IModbusMaster, T> action, CancellationToken ct)
        {
            var master = await EnsureConnectedAsync(ct).ConfigureAwait(false);
            try
            {
                return await Task.Run(() => action(master), ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsConnectionException(ex))
            {
                MarkDisconnected();
                master = await EnsureConnectedAsync(ct, forceReconnect: true).ConfigureAwait(false);
                return await Task.Run(() => action(master), ct).ConfigureAwait(false);
            }
        }

        private async Task<IModbusMaster> EnsureConnectedAsync(CancellationToken ct, bool forceReconnect = false)
        {
            await _reconnectLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ConnectionSettings? settings;
                IModbusMaster? currentMaster;

                lock (_sync)
                {
                    settings = CurrentSettings;
                    currentMaster = _master;
                }

                if (settings is null)
                {
                    throw new InvalidOperationException("Bağlantı ayarları yok.");
                }

                if (!forceReconnect && currentMaster is not null && IsSocketAlive(_tcpClient))
                {
                    return currentMaster;
                }

                var tcp = new TcpClient();
                const int timeoutMs = 2000;

                var connectTask = tcp.ConnectAsync(settings.IpAddress, settings.Port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs, ct)).ConfigureAwait(false);

                if (completed != connectTask)
                {
                    try { tcp.Close(); } catch { }
                    throw new TimeoutException($"Bağlantı zaman aşımı ({timeoutMs} ms).");
                }

                await connectTask.ConfigureAwait(false);
                ConfigureTcpClient(tcp);

                var factory = new ModbusFactory();
                var master = factory.CreateMaster(tcp);

                lock (_sync)
                {
                    try { _tcpClient?.Close(); } catch { }
                    _tcpClient = tcp;
                    _master = master;
                }

                return master;
            }
            finally
            {
                _reconnectLock.Release();
            }
        }

        private static bool IsSocketAlive(TcpClient? client)
        {
            if (client?.Client is null) return false;

            var socket = client.Client;
            if (!socket.Connected) return false;

            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        private void MarkDisconnected()
        {
            lock (_sync)
            {
                try { _tcpClient?.Close(); } catch { }
                _tcpClient = null;
                _master = null;
            }
        }

        private static bool IsConnectionException(Exception ex)
        {
            return ex is SocketException
                || ex is IOException
                || ex is ObjectDisposedException
                || ex is TimeoutException
                || ex.InnerException is SocketException
                || ex.InnerException is IOException
                || ex.InnerException is TimeoutException;
        }

        private static void ConfigureTcpClient(TcpClient tcp)
        {
            tcp.NoDelay = true;
            tcp.ReceiveTimeout = 3000;
            tcp.SendTimeout = 3000;

            try
            {
                tcp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            }
            catch
            {
                // platform desteklemiyorsa sessizce geç
            }
        }
    }
}
