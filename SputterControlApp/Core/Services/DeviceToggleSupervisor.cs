using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public sealed class DeviceToggleSupervisor : IDisposable
{
    private readonly Control _uiOwner;
    private readonly Func<Action, bool> _uiPost; // UI thread'e post

    private readonly Button _button;
    private readonly Func<CancellationToken, Task<short>> _readStatusAsync;

    private readonly short _onValue;
    private readonly short _offValue;

    private readonly Action<bool?> _applyUi;     // UI çiz (Tag set dahil)
    private readonly Action<bool>? _onConfirmedOn;
    private readonly Action<bool>? _onConfirmedOff;

    private readonly Action? _triggerRefresh;    // optional (btnRefresh.PerformClick)

    private CancellationTokenSource? _watchCts;
    private bool? _confirmedState; // true/false

    public DeviceToggleSupervisor(
        Control uiOwner,
        Button button,
        Func<CancellationToken, Task<short>> readStatusAsync,
        short offValue,
        short onValue,
        Action<bool?> applyUi,
        Action<bool>? onConfirmedOn = null,
        Action<bool>? onConfirmedOff = null,
        Action? triggerRefresh = null)
    {
        _uiOwner = uiOwner ?? throw new ArgumentNullException(nameof(uiOwner));
        _button = button ?? throw new ArgumentNullException(nameof(button));
        _readStatusAsync = readStatusAsync ?? throw new ArgumentNullException(nameof(readStatusAsync));
        _offValue = offValue;
        _onValue = onValue;
        _applyUi = applyUi ?? throw new ArgumentNullException(nameof(applyUi));
        _onConfirmedOn = onConfirmedOn;
        _onConfirmedOff = onConfirmedOff;
        _triggerRefresh = triggerRefresh;

        _uiPost = (a) =>
        {
            if (_uiOwner.IsDisposed || !_uiOwner.IsHandleCreated) return false;
            if (_uiOwner.InvokeRequired) _uiOwner.BeginInvoke(a);
            else a();
            return true;
        };

        // Hook click (BindOnOffButton zaten bağlı; biz sadece watcher için ekliyoruz)
        _button.Click -= Button_Click;
        _button.Click += Button_Click;
    }

    public void Dispose()
    {
        try { _watchCts?.Cancel(); } catch { }
        _watchCts = null;

        try { _button.Click -= Button_Click; } catch { }
    }

    private async void Button_Click(object? sender, EventArgs e)
    {
        // Click anında mevcut statüyü 1 kez oku -> hedefi tahmin et -> watcher başlat
        short cur;
        try
        {
            cur = await _readStatusAsync(CancellationToken.None);
        }
        catch
        {
            // okuyamazsak "ON" watcher başlatmak genelde daha güvenli (senaryona göre değişebilir)
            StartWatcher(expected: _onValue);
            return;
        }

        if (cur == _offValue) StartWatcher(expected: _onValue); // hedef ON
        else if (cur == _onValue) StartWatcher(expected: _offValue); // hedef OFF
        else StartWatcher(expected: _onValue);
    }

    public void StartOnWatcher() => StartWatcher(_onValue);
    public void StartOffWatcher() => StartWatcher(_offValue);

    private void StartWatcher(short expected)
    {
        try { _watchCts?.Cancel(); } catch { }
        _watchCts = new CancellationTokenSource();
        var ct = _watchCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                // İstersen click sonrası refresh tetikle
                _uiPost(() => _triggerRefresh?.Invoke());

                bool ok = await WaitEqualsAsync(expected, timeoutMs: 10_000, pollMs: 200, ct);
                if (!ok || ct.IsCancellationRequested) return;

                bool confirmed = (expected == _onValue);
                _uiPost(() => OnConfirmed(confirmed));
            }
            catch (OperationCanceledException) { }
            catch { /* istersen log */ }
        }, ct);
    }

    private async Task<bool> WaitEqualsAsync(short expected, int timeoutMs, int pollMs, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                var v = await _readStatusAsync(ct);
                if (v == expected) return true;
            }
            catch
            {
                // sessiz geç
            }

            await Task.Delay(pollMs, ct);
        }
        return false;
    }

    private void OnConfirmed(bool state)
    {
        if (_confirmedState == state) return;
        _confirmedState = state;

        // UI çiz
        _applyUi(state);

        // Aksiyonlar
        if (state)
            _onConfirmedOn?.Invoke(true);
        else
            _onConfirmedOff?.Invoke(false);
    }

    public void NotifyConfirmedState(bool state, bool applyUi = false)
    {
        _confirmedState = state;
        if (applyUi) _applyUi(state);
    }


    // Force OFF: UI'dan bağımsız olarak PLC'ye OFF yaz, sonra durumu oku ve UI uygula QCM için. 
    public async Task ForceOffAsync()
    {
        try
        {
            // mevcut durumu oku
            var current = await _readStatusAsync(CancellationToken.None);

            if (current == _offValue)
                return; // zaten OFF

            // UI thread üzerinden buton click tetikle
            if (_uiOwner.InvokeRequired)
            {
                _uiOwner.BeginInvoke(new Action(() => _button.PerformClick()));
            }
            else
            {
                _button.PerformClick();
            }

            // confirm gelmesi için küçük bekleme
            await Task.Delay(200);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ForceOffAsync error: {ex.Message}");
        }
    }



}
