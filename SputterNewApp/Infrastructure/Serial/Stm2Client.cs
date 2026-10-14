using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SputterNewApp.Infrastructure.Serial
{
    public sealed class Stm2Client : IDisposable
    {
        // STM-2 sabitleri (INFICON dokümantasyonuna göre)
        private const byte STX = 0x02;
        private const byte ADDR = 0x10;
        private const byte CMD_RSP_CMD = 0x80;
        private const byte CR = 0x0D;

        private readonly SerialPort _port;
        private readonly SemaphoreSlim _ioLock = new(1, 1);

        public string PortName => _port.PortName;
        public int BaudRate
        {
            get => _port.BaudRate;
            set => _port.BaudRate = value;
        }

        public bool IsOpen
        {
            get
            {
                try { return _port.IsOpen; }
                catch { return false; }
            }
        }

        public string SerialNumber { get; set; } = "";

        public bool CheckPortExists()
        {
            try
            {
                var ports = SerialPort.GetPortNames();
                return ports.Any(p => string.Equals(p, _port.PortName, StringComparison.OrdinalIgnoreCase));
            }
            catch { return false; }
        }

        public Stm2Client(string portName,
                          int baudRate = 115200,
                          Parity parity = Parity.None,
                          int dataBits = 8,
                          StopBits stopBits = StopBits.One)
        {
            _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500,
                Encoding = Encoding.ASCII,
                NewLine = "\r",
                DtrEnable = true,
                RtsEnable = true
            };
        }

        public void Open()
        {
            if (_port.IsOpen) return;
            _port.Open();
            try
            {
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
            }
            catch { }
        }

        public bool TryOpen()
        {
            try
            {
                if (_port.IsOpen) return true;
                _port.Open();
                try
                {
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                }
                catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (_port.IsOpen)
                    _port.Close();
            }
            catch { }
        }

        public void Dispose()
        {
            try { Close(); } catch { }
            try { _port.Dispose(); } catch { }
            try { _ioLock.Dispose(); } catch { }
        }

        // STM-2 Application Commands
        private const string CMD_READ_THICKNESS = "S";
        private const string CMD_READ_RATE = "T";
        private const string CMD_READ_LIFE = "V";

        private const string CMD_WRITE_DENSITY = "E";
        private const string CMD_WRITE_ZRATIO = "Z";
        private const string CMD_WRITE_TOOLING = "J";

        private const string CMD_ZERO_THICKNESS = "B";
        private const string CMD_READ_FREQUENCY = "U";
        private const string CMD_READ_SN = "@A";

        /// <summary>
        /// STM-2 cihazına ASCII komut paketi gönderir ve CR ile sonlanan yanıtı bekler.
        /// Zaman aşımı durumunda istisna fırlatmaz, güvenli bir şekilde null döner.
        /// </summary>
        public async Task<byte[]?> SendAsciiCommandAsync(string messageAscii,
                                                         int timeoutMs = 800,
                                                         CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(messageAscii))
                return null;

            const int maxAttempts = 2;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                if (ct.IsCancellationRequested) return null;

                try
                {
                    await _ioLock.WaitAsync(ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return null;
                }

                try
                {
                    if (!_port.IsOpen)
                        return null;

                    var tx = BuildFrame(messageAscii);
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    _port.Write(tx, 0, tx.Length);

                    var rx = await ReadUntilCrAsync(timeoutMs, ct).ConfigureAwait(false);
                    if (rx != null && rx.Length > 0)
                        return rx;
                }
                catch (Exception)
                {
                    try
                    {
                        if (_port.IsOpen)
                        {
                            _port.DiscardInBuffer();
                            _port.DiscardOutBuffer();
                        }
                    }
                    catch { }
                }
                finally
                {
                    _ioLock.Release();
                }

                if (attempt < maxAttempts)
                {
                    try
                    {
                        await Task.Delay(30, ct).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public async Task<string> PingAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync("@", timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryPingAsync(int timeoutMs = 600, CancellationToken ct = default)
        {
            var res = await PingAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public async Task<string> GetSNAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync(CMD_READ_SN, timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryGetSNAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var res = await GetSNAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public static byte[] BuildFrame(string messageAscii)
        {
            var msgBytes = Encoding.ASCII.GetBytes(messageAscii);

            int sum = ADDR + CMD_RSP_CMD;
            for (int i = 0; i < msgBytes.Length; i++)
                sum = (sum + msgBytes[i]) & 0xFF;

            int x = (sum >> 4) & 0x0F;
            int y = sum & 0x0F;

            byte cksum1 = (byte)(0x30 + x);
            byte cksum2 = (byte)(0x30 + y);

            var frame = new byte[1 + 1 + 1 + msgBytes.Length + 2 + 1];
            int k = 0;
            frame[k++] = STX;
            frame[k++] = ADDR;
            frame[k++] = CMD_RSP_CMD;
            Buffer.BlockCopy(msgBytes, 0, frame, k, msgBytes.Length);
            k += msgBytes.Length;
            frame[k++] = cksum1;
            frame[k++] = cksum2;
            frame[k++] = CR;

            return frame;
        }

        public static bool TryParseAsciiMessage(byte[]? rxFrame, out string asciiMessage)
        {
            asciiMessage = string.Empty;
            if (rxFrame == null || rxFrame.Length < 7) return false;

            int stxIdx = Array.IndexOf(rxFrame, STX);
            if (stxIdx < 0) return false;

            int crIdx = Array.LastIndexOf(rxFrame, CR);
            if (crIdx <= stxIdx + 5) return false;

            int msgStart = stxIdx + 3;
            int msgLen = crIdx - 2 - msgStart;
            if (msgLen <= 0) return false;

            asciiMessage = Encoding.ASCII.GetString(rxFrame, msgStart, msgLen);
            return true;
        }

        /// <summary>
        /// Seri porttan CR baytı gelene kadar veriyi okur.
        /// Belirtilen zaman aşımında CR gelmezse TimeoutException fırlatmaz, güvenle null döner.
        /// </summary>
        private async Task<byte[]?> ReadUntilCrAsync(int timeoutMs, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var buf = new List<byte>(64);
                byte[] temp = new byte[64];

                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    if (ct.IsCancellationRequested) return null;

                    try
                    {
                        if (!_port.IsOpen) return null;

                        int bytesToRead = _port.BytesToRead;
                        if (bytesToRead > 0)
                        {
                            int count = _port.Read(temp, 0, Math.Min(bytesToRead, temp.Length));
                            for (int i = 0; i < count; i++)
                            {
                                byte b = temp[i];
                                buf.Add(b);

                                if (buf.Count > 256)
                                    buf.Clear();

                                if (b == CR)
                                    return buf.ToArray();
                            }
                        }
                        else
                        {
                            Thread.Sleep(8);
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                // Zaman aşımı: İstisna fırlatmadan temiz null dön
                return null;
            }, ct).ConfigureAwait(false);
        }

        public async Task<string> GetThicknessAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync(CMD_READ_THICKNESS, timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryGetThicknessAsync(int timeoutMs = 600, CancellationToken ct = default)
        {
            var res = await GetThicknessAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public async Task<string> GetRateAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync(CMD_READ_RATE, timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryGetRateAsync(int timeoutMs = 600, CancellationToken ct = default)
        {
            var res = await GetRateAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public async Task<string> GetFrequencyAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync(CMD_READ_FREQUENCY, timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryGetFrequencyAsync(int timeoutMs = 600, CancellationToken ct = default)
        {
            var res = await GetFrequencyAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public async Task<string> GetLifeAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var rx = await SendAsciiCommandAsync(CMD_READ_LIFE, timeoutMs, ct).ConfigureAwait(false);
            return (rx != null && TryParseAsciiMessage(rx, out var msg)) ? msg : "";
        }

        public async Task<string?> TryGetLifeAsync(int timeoutMs = 800, CancellationToken ct = default)
        {
            var res = await GetLifeAsync(timeoutMs, ct).ConfigureAwait(false);
            return string.IsNullOrEmpty(res) ? null : res;
        }

        public async Task WriteDensityAsync(float density, int timeoutMs = 1200, CancellationToken ct = default)
        {
            string cmd = $"{CMD_WRITE_DENSITY} {density:0.###}";
            await SendAsciiCommandAsync(cmd, timeoutMs, ct).ConfigureAwait(false);
        }

        public async Task WriteZRatioAsync(float zRatio, int timeoutMs = 1200, CancellationToken ct = default)
        {
            string cmd = $"{CMD_WRITE_ZRATIO} {zRatio:0.###}";
            await SendAsciiCommandAsync(cmd, timeoutMs, ct).ConfigureAwait(false);
        }

        public async Task WriteToolingAsync(float tooling, int timeoutMs = 1200, CancellationToken ct = default)
        {
            string cmd = $"{CMD_WRITE_TOOLING} {tooling:0.###}";
            await SendAsciiCommandAsync(cmd, timeoutMs, ct).ConfigureAwait(false);
        }

        public async Task ZeroThicknessAsync(int timeoutMs = 1200, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return;

            try
            {
                await _ioLock.WaitAsync(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                if (!_port.IsOpen) return;

                var tx = BuildFrame(CMD_ZERO_THICKNESS);
                _port.DiscardInBuffer();
                _port.Write(tx, 0, tx.Length);
                await Task.Delay(50, ct).ConfigureAwait(false);
            }
            catch { }
            finally
            {
                _ioLock.Release();
            }
        }

        public void ZeroThickness_NoWait()
        {
            try
            {
                if (!_port.IsOpen) return;

                var tx = BuildFrame(CMD_ZERO_THICKNESS);
                _port.DiscardInBuffer();
                _port.Write(tx, 0, tx.Length);
            }
            catch { }
        }
    }
}
