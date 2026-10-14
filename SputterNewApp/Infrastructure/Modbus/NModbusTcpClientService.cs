using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using SputterNewApp.Core.Exceptions;
using SputterNewApp.Core.Models;
using SputterNewApp.Core.Services;

namespace SputterNewApp.Infrastructure.Modbus
{
    public sealed class NModbusTcpClientService : IModbusClientService
    {
        private TcpClient? _tcpClient;
        private IModbusMaster? _master;
        private readonly object _sync = new();

        private readonly SemaphoreSlim _ioLock = new(1, 1);
        private readonly SemaphoreSlim _reconnectLock = new(1, 1);

        public bool IsConnected
        {
            get
            {
                lock (_sync)
                {
                    return CurrentSettings != null && _tcpClient != null && _master != null && IsSocketAlive(_tcpClient);
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
            const int timeoutMs = 2500;

            var connectTask = tcp.ConnectAsync(settings.IpAddress, settings.Port);
            var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs, ct)).ConfigureAwait(false);

            if (completed != connectTask)
            {
                try { tcp.Close(); } catch { }
                throw new TimeoutException($"PLC bağlantı zaman aşımı ({timeoutMs} ms): {settings.IpAddress}:{settings.Port}");
            }

            await connectTask.ConfigureAwait(false);
            ConfigureTcpClient(tcp);

            var factory = new ModbusFactory();
            var master = factory.CreateMaster(tcp);
            ConfigureTransport(master);

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

                try
                {
                    _tcpClient?.GetStream()?.Close();
                }
                catch { }

                try
                {
                    _tcpClient?.Close();
                }
                catch { }

                try
                {
                    _master?.Dispose();
                }
                catch { }

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
                    settings = CurrentSettings ?? throw new ModbusConnectionLostException("Modbus bağlantısı aktif değil.");
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
                    settings = CurrentSettings ?? throw new ModbusConnectionLostException("Modbus bağlantısı aktif değil.");
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
            IModbusMaster master;
            try
            {
                master = await EnsureConnectedAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsConnectionException(ex))
            {
                MarkDisconnected();
                throw new ModbusConnectionLostException("PLC bağlantısı kurulamadı veya koptu.", ex);
            }

            // Task.Run içindeki lambdayı try-catch içine alarak debugger'ın 'User-Unhandled' kırmasını engelliyoruz
            var initialExec = await Task.Run(() =>
            {
                try
                {
                    return (Success: true, Result: action(master), Error: (Exception?)null);
                }
                catch (Exception ex)
                {
                    return (Success: false, Result: default!, Error: ex);
                }
            }, ct).ConfigureAwait(false);

            if (initialExec.Success)
            {
                return initialExec.Result;
            }

            var err = initialExec.Error!;
            if (IsConnectionException(err))
            {
                MarkDisconnected();

                // İlk hata sonrası 1 kez yeniden bağlanmayı dene
                try
                {
                    await Task.Delay(150, ct).ConfigureAwait(false);
                    master = await EnsureConnectedAsync(ct, forceReconnect: true).ConfigureAwait(false);
                    
                    var retryExec = await Task.Run(() =>
                    {
                        try
                        {
                            return (Success: true, Result: action(master), Error: (Exception?)null);
                        }
                        catch (Exception rex)
                        {
                            return (Success: false, Result: default!, Error: rex);
                        }
                    }, ct).ConfigureAwait(false);

                    if (retryExec.Success)
                    {
                        return retryExec.Result;
                    }

                    MarkDisconnected();
                    throw new ModbusConnectionLostException("PLC bağlantısı koptu ve yeniden bağlanılamadı.", retryExec.Error);
                }
                catch (ModbusConnectionLostException)
                {
                    throw;
                }
                catch (Exception retryEx)
                {
                    MarkDisconnected();
                    throw new ModbusConnectionLostException($"PLC bağlantısı koptu: {retryEx.Message}", retryEx);
                }
            }

            throw err;
        }

        private async Task<IModbusMaster> EnsureConnectedAsync(CancellationToken ct, bool forceReconnect = false)
        {
            await _reconnectLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ConnectionSettings? settings;
                IModbusMaster? currentMaster;
                TcpClient? currentTcp;

                lock (_sync)
                {
                    settings = CurrentSettings;
                    currentMaster = _master;
                    currentTcp = _tcpClient;
                }

                if (settings is null)
                {
                    throw new InvalidOperationException("Modbus bağlantı ayarları yok.");
                }

                if (!forceReconnect && currentMaster is not null && IsSocketAlive(currentTcp))
                {
                    return currentMaster;
                }

                // Eski soketi temizle
                try { currentTcp?.Close(); } catch { }
                try { currentMaster?.Dispose(); } catch { }

                var tcp = new TcpClient();
                const int timeoutMs = 4000;

                var connectTask = tcp.ConnectAsync(settings.IpAddress, settings.Port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs, ct)).ConfigureAwait(false);

                if (completed != connectTask)
                {
                    try { tcp.Close(); } catch { }
                    throw new TimeoutException($"PLC yeniden bağlantı zaman aşımı ({timeoutMs} ms).");
                }

                await connectTask.ConfigureAwait(false);
                ConfigureTcpClient(tcp);

                var factory = new ModbusFactory();
                var master = factory.CreateMaster(tcp);
                ConfigureTransport(master);

                lock (_sync)
                {
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
                try { _master?.Dispose(); } catch { }
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
                || ex is ModbusConnectionLostException
                || ex.InnerException is SocketException
                || ex.InnerException is IOException
                || ex.InnerException is TimeoutException
                || ex.InnerException is ModbusConnectionLostException
                || (ex.Message != null && ex.Message.Contains("0 bytes returned", StringComparison.OrdinalIgnoreCase));
        }

        private static void ConfigureTcpClient(TcpClient tcp)
        {
            tcp.NoDelay = true;
            tcp.ReceiveTimeout = 5000;
            tcp.SendTimeout = 5000;

            try
            {
                tcp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            }
            catch { }
        }

        private static void ConfigureTransport(IModbusMaster master)
        {
            try
            {
                if (master.Transport != null)
                {
                    master.Transport.ReadTimeout = 3500;
                    master.Transport.WriteTimeout = 3500;
                    // Modbus TCP'de ölü soket üzerinde paket tekrarlanmamalı;
                    // yeniden bağlantı taze soketle ExecuteWithReconnectAsync katmanında yönetilir.
                    master.Transport.Retries = 0;
                }
            }
            catch { }
        }
    }
}
