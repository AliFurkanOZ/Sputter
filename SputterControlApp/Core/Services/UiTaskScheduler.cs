using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;


namespace SputterControlApp.Core.Services
{
    public sealed class UiTaskScheduler : IDisposable
    {
        private readonly Timer _timer;
        private readonly Func<Task>? _job250ms;
        private readonly Func<Task>? _job500ms;
        private readonly Func<Task>? _job1000ms;

        private long _t250;
        private long _t500;
        private long _t1000;
        private int _busy;

        public UiTaskScheduler(
            Func<Task>? job250ms,
            Func<Task>? job500ms,
            Func<Task>? job1000ms,
            int baseTickMs = 50)
        {
            _job250ms = job250ms;
            _job500ms = job500ms;
            _job1000ms = job1000ms;

            _t250 = Environment.TickCount64;
            _t500 = Environment.TickCount64;
            _t1000 = Environment.TickCount64;

            _timer = new Timer
            {
                Interval = baseTickMs
            };
            _timer.Tick += Timer_Tick;
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        // -------------------------------------------------
        // Timer Tick Handler (UI thread)
        // -------------------------------------------------
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            await TickAsync();
        }
        private async Task TickAsync()
        {
            if (Interlocked.Exchange(ref _busy, 1) == 1)
                return;

            try
            {
                long now = Environment.TickCount64;

                if (_job250ms != null && now - _t250 >= 250)
                {
                    _t250 = now;
                    await _job250ms();
                }

                if (_job500ms != null && now - _t500 >= 500)
                {
                    _t500 = now;
                    await _job500ms();
                }
                if (_job1000ms != null && now - _t1000 >= 1000)
                {
                    _t1000 = now;
                    await _job1000ms();
                }
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
            }
        }

        public void Dispose()
        {
            _timer.Stop();

            //içeride job çalışıyorsa kısa süre bekle
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (Interlocked.CompareExchange(ref _busy, 0, 0) == 1 && sw.ElapsedMilliseconds < 500)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }

            _timer.Dispose();
        }

    }
}
