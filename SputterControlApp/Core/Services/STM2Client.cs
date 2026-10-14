using System.IO;
using System.IO.Ports;
using System.Text;


public sealed class Stm2Client : IDisposable
{
    // STM-2 sabitleri (docx'e göre)
    private const byte STX = 0x02;
    private const byte ADDR = 0x10;
    private const byte CMD_RSP_CMD = 0x80;
    private const byte CR = 0x0D;

    private readonly SerialPort _port;
    private readonly SemaphoreSlim _ioLock = new(1, 1);

    public string PortName => _port.PortName;
    public bool IsOpen => _port.IsOpen;
    public Stm2Client(string portName,
                      int baudRate = 115200,
                      Parity parity = Parity.None,
                      int dataBits = 8,
                      StopBits stopBits = StopBits.One)
    {
        // Baud/parity/stopbits değerlerini INFICON remote dokümanına göre güncelle.
        _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
        {
            Handshake = Handshake.None,
            ReadTimeout = 500,   // ReadExisting yerine byte okuyacaksan kullanılır
            WriteTimeout = 500,
            Encoding = Encoding.ASCII,
            NewLine = "\r"
        };
    }
    public void Open()
    {
        if (_port.IsOpen) return;
        _port.Open();
        // Buffer temizliği
        _port.DiscardInBuffer();
        _port.DiscardOutBuffer();
    }
    public void Close()
    {
        if (!_port.IsOpen) return;
        _port.Close();
    }

    public void Dispose()
    {
        try { Close(); } catch { /* ignore */ }
        _port.Dispose();
        _ioLock.Dispose();
    }
    // -----------------------------
    // STM-2 Application Commands
    // -----------------------------

    // ⚠️ Komut harfleri cihaz dokümanına göre değişebilir.
    // Burayı tek yerden yönetiyoruz.
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
    /// STM-2'ye ASCII komut gönderir (örn: "@") ve CR ile biten cevabı döndürür.
    /// Cevap: ham frame (STX...CR dahil) byte[].
    /// </summary>
    public async Task<byte[]> SendAsciiCommandAsync(string messageAscii,
                                                    int timeoutMs = 1200,
                                                    CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(messageAscii))
            throw new ArgumentException("messageAscii cannot be null/empty", nameof(messageAscii));

        const int maxAttempts = 3;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            ct.ThrowIfCancellationRequested();

            await _ioLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (!_port.IsOpen)
                    throw new InvalidOperationException("Serial port is not open.");

                // 1) Tx frame oluştur
                var tx = BuildFrame(messageAscii);

                // 2) Rx buffer temizle (önceki kalıntılar karışmasın)
                _port.DiscardInBuffer();

                // 3) Yaz
                _port.Write(tx, 0, tx.Length);

                // 4) Cevabı CR gelene kadar oku
                var rx = await ReadUntilCrAsync(timeoutMs, ct).ConfigureAwait(false);
                return rx;
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
            {
                TryRecoverPort();
            }
            finally
            {
                _ioLock.Release();
            }

            await Task.Delay(120, ct).ConfigureAwait(false);
        }

        throw new TimeoutException($"STM-2 communication failed after retry on {PortName}.");
    }

    private static bool IsTransient(Exception ex)
        => ex is TimeoutException || ex is IOException || ex is InvalidOperationException;

    private void TryRecoverPort()
    {
        try
        {
            if (_port.IsOpen)
                _port.Close();
        }
        catch { }

        try
        {
            _port.Open();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
        }
        catch { }
    }

    /// <summary>
    /// "@ ping" komutu: cevap içindeki ASCII mesajı döndürür (örn "STM-2 A0.3").
    /// </summary>
    public async Task<string> PingAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync("@", timeoutMs, ct).ConfigureAwait(false);
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    public async Task<string> getSNAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync("@A", timeoutMs, ct).ConfigureAwait(false);
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    /// <summary>
    /// STM-2 frame: <STX><ADDR><CMD_RSP><MESSAGE...><CKSUM1><CKSUM2><CR>
    /// Checksum: SUM(ADDR + CMD_RSP + MESSAGE bytes) mod 256 => XY
    /// CKSUM1=0x3X, CKSUM2=0x3Y
    /// </summary>
    public static byte[] BuildFrame(string messageAscii)
    {
        var msgBytes = Encoding.ASCII.GetBytes(messageAscii);

        // checksum SUM = ADDR + CMD_RSP + message bytes
        int sum = ADDR + CMD_RSP_CMD;
        for (int i = 0; i < msgBytes.Length; i++)
            sum = (sum + msgBytes[i]) & 0xFF;

        // XY: high nibble = X, low nibble = Y
        int x = (sum >> 4) & 0x0F;
        int y = sum & 0x0F;

        byte cksum1 = (byte)(0x30 + x); // 0x3X
        byte cksum2 = (byte)(0x30 + y); // 0x3Y

        // Frame length: 1(STX)+1(ADDR)+1(CMD)+N(MSG)+2(CKSUM)+1(CR)
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
    /// <summary>
    /// Rx frame içinden ASCII mesajı çıkarır.
    /// Beklenen: STX(0) ADDR(1) CMD_RSP(2) MESSAGE(3..len-4) CKSUM1(len-3) CKSUM2(len-2) CR(len-1)
    /// </summary>
    public static bool TryParseAsciiMessage(byte[] rxFrame, out string asciiMessage)
    {
        asciiMessage = string.Empty;
        if (rxFrame == null || rxFrame.Length < 7) return false;
        if (rxFrame[0] != STX) return false;
        if (rxFrame[^1] != CR) return false;

        // MESSAGE: 3..(len-4)
        int msgLen = rxFrame.Length - 1 /*CR*/ - 2 /*CKSUM*/ - 3 /*STX ADDR CMD*/;
        if (msgLen <= 0) return false;

        asciiMessage = Encoding.ASCII.GetString(rxFrame, 3, msgLen);
        return true;
    }
    private async Task<byte[]> ReadUntilCrAsync(int timeoutMs, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var buf = new List<byte>(64);

        // ReadByte() senkron; UI thread’de çağırma. Biz zaten async method içinde, Task.Run ile sarıyoruz.
        return await Task.Run(() =>
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                if (sw.ElapsedMilliseconds > timeoutMs)
                    throw new TimeoutException($"STM-2 response timeout on {PortName}.");

                try
                {
                    int b = _port.ReadByte(); // bloklar; ReadTimeout kullanmıyoruz -> kendi timeout'umuz var
                    if (b < 0) continue;

                    buf.Add((byte)b);

                    if (b == CR)
                        return buf.ToArray();
                }
                catch (InvalidOperationException)
                {
                    throw; // port kapandı vb.
                }
                catch (TimeoutException)
                {
                    // SerialPort ReadTimeout tetiklenirse burada döner; biz loop'a devam edip genel timeout'u kullanıyoruz
                }
            }
        }, ct).ConfigureAwait(false);
    }
    // -----------------------------
    // STM-2 Measurement Commands
    // -----------------------------
    public async Task<string> GetThicknessAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync(CMD_READ_THICKNESS, timeoutMs, ct).ConfigureAwait(false); // Thickness
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    public async Task<string> GetRateAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync(CMD_READ_RATE, timeoutMs, ct).ConfigureAwait(false); // Rate
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    public async Task<string> GetFrequencyAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync("U", timeoutMs, ct).ConfigureAwait(false); // Frequency
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    public async Task<string> GetLifeAsync(int timeoutMs = 1200, CancellationToken ct = default)
    {
        var rx = await SendAsciiCommandAsync(CMD_READ_LIFE, timeoutMs, ct).ConfigureAwait(false);
        return TryParseAsciiMessage(rx, out var msg) ? msg : "";
    }

    /// <summary>
    /// Density yaz: örn "D 2.700"
    /// </summary>
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

    /// <summary>
    /// Zero thickness: geri dönüş beklemeden gönder.
    /// </summary>
    public void ZeroThickness_NoWait()
    {
        if (!_port.IsOpen) throw new InvalidOperationException("Serial port is not open.");

        // build + write (no read)
        var tx = BuildFrame(CMD_ZERO_THICKNESS);
        _port.DiscardInBuffer();
        _port.Write(tx, 0, tx.Length);
    }
}
