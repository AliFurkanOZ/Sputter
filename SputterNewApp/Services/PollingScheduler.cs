using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SputterNewApp.Services
{
    public sealed class PollingScheduler : IDisposable
    {
        private readonly Func<Task>? _job250ms;
        private readonly Func<Task>? _job500ms;
        private readonly Func<Task>? _job1000ms;

        private CancellationTokenSource? _cts;
        private Task? _runnerTask;
        private int _busy;

        public bool IsRunning => _cts != null && !_cts.IsCancellationRequested;

        public PollingScheduler(
            Func<Task>? job250ms,
            Func<Task>? job500ms,
            Func<Task>? job1000ms)
        {
            _job250ms = job250ms;
            _job500ms = job500ms;
            _job1000ms = job1000ms;
        }

        public void Start()
        {
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            _runnerTask = Task.Run(() => LoopAsync(token), token);
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
            }
            catch { }

            _cts = null;
        }

        private async Task LoopAsync(CancellationToken ct)
        {
            long t250 = Environment.TickCount64;
            long t500 = Environment.TickCount64;
            long t1000 = Environment.TickCount64;

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!await timer.WaitForNextTickAsync(ct))
                        break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (Interlocked.Exchange(ref _busy, 1) == 1)
                    continue;

                try
                {
                    long now = Environment.TickCount64;

                    if (_job250ms != null && now - t250 >= 250)
                    {
                        t250 = now;
                        await ExecuteSafeAsync(_job250ms, ct);
                    }

                    if (_job500ms != null && now - t500 >= 500)
                    {
                        t500 = now;
                        await ExecuteSafeAsync(_job500ms, ct);
                    }

                    if (_job1000ms != null && now - t1000 >= 1000)
                    {
                        t1000 = now;
                        await ExecuteSafeAsync(_job1000ms, ct);
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _busy, 0);
                }
            }
        }

        private static async Task ExecuteSafeAsync(Func<Task> job, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return;
            try
            {
                await job().ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PollingScheduler] Task error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
