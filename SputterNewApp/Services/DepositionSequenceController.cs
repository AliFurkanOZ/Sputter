using System;
using System.Threading;
using System.Threading.Tasks;

namespace SputterNewApp.Services
{
    public enum SequencePhase
    {
        Idle,
        Step1,
        Step2
    }

    public sealed class DepositionSequenceController : IDisposable
    {
        private readonly string _sourceName;
        private CancellationTokenSource? _cts;
        private SequencePhase _phase = SequencePhase.Idle;
        private readonly object _sync = new();

        private DateTime _step2StartedAtUtc;
        private TimeSpan _step2Total;

        public event Action<SequencePhase>? PhaseChanged;
        public event Action<TimeSpan>? Step1RemainingChanged;
        public event Action<TimeSpan>? Step2RemainingChanged;
        public event Action<TimeSpan, TimeSpan>? Step1ProgressChanged; // (remaining, total)
        public event Action<TimeSpan, TimeSpan>? Step2ProgressChanged; // (remaining, total)
        public event Func<Task>? SequenceCompletedAsync;
        public event Action? SequenceCompleted;

        public SequencePhase CurrentPhase
        {
            get => _phase;
            private set
            {
                if (_phase != value)
                {
                    _phase = value;
                    PhaseChanged?.Invoke(_phase);
                }
            }
        }

        public bool IsRunning => CurrentPhase != SequencePhase.Idle;

        public DepositionSequenceController(string sourceName)
        {
            _sourceName = sourceName;
        }

        public static TimeSpan ComputeStep1Duration(decimal setW, decimal ramp, decimal dwellMin)
        {
            double r = Math.Max(0.001, (double)ramp);
            double totalSec = (double)dwellMin * 60.0 + (double)setW / r;
            return TimeSpan.FromSeconds(Math.Max(0, totalSec));
        }

        public static TimeSpan ComputeStep2DurationPlanned(decimal setW1, decimal setW2, decimal ramp2, decimal dwell2Min)
        {
            double r2 = Math.Max(0.001, (double)ramp2);
            double delta = Math.Max(0, (double)(setW2 - setW1));
            double totalSec = (double)dwell2Min * 60.0 + delta / r2;
            return TimeSpan.FromSeconds(Math.Max(0, totalSec));
        }

        public static TimeSpan ComputeStep2DurationFromPower(double currentPower, decimal setW2, decimal ramp2, decimal dwell2Min)
        {
            double r2 = Math.Max(0.001, (double)ramp2);
            double delta = Math.Max(0, (double)setW2 - currentPower);
            double totalSec = (double)dwell2Min * 60.0 + delta / r2;
            return TimeSpan.FromSeconds(Math.Max(0, totalSec));
        }

        public void Start(
            Func<TimeSpan> getStep1Duration,
            Func<TimeSpan> getStep2PlannedDuration,
            Func<TimeSpan> getStep2DynamicDuration)
        {
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            var ct = _cts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    // --- STEP 1 ---
                    CurrentPhase = SequencePhase.Step1;
                    var step1Dur = getStep1Duration();
                    await RunCountdownAsync(step1Dur, Step1RemainingChanged, Step1ProgressChanged, ct);

                    // --- STEP 2 ---
                    CurrentPhase = SequencePhase.Step2;
                    lock (_sync)
                    {
                        _step2StartedAtUtc = DateTime.UtcNow;
                        _step2Total = getStep2PlannedDuration();
                    }

                    await RunStep2CountdownDynamicAsync(getStep2DynamicDuration, Step2RemainingChanged, Step2ProgressChanged, ct);

                    ct.ThrowIfCancellationRequested();

                    if (SequenceCompletedAsync != null)
                    {
                        await SequenceCompletedAsync.Invoke();
                    }
                    else
                    {
                        SequenceCompleted?.Invoke();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception) { }
                finally
                {
                    CurrentPhase = SequencePhase.Idle;
                    Step1RemainingChanged?.Invoke(TimeSpan.Zero);
                    Step2RemainingChanged?.Invoke(TimeSpan.Zero);
                }
            }, ct);
        }

        public void Cancel()
        {
            try { _cts?.Cancel(); } catch { }
            _cts = null;
            CurrentPhase = SequencePhase.Idle;
            Step1RemainingChanged?.Invoke(TimeSpan.Zero);
            Step2RemainingChanged?.Invoke(TimeSpan.Zero);
        }

        private async Task RunCountdownAsync(
            TimeSpan duration,
            Action<TimeSpan>? onTick,
            Action<TimeSpan, TimeSpan>? onProgress,
            CancellationToken ct)
        {
            var remaining = duration;
            onTick?.Invoke(remaining);
            onProgress?.Invoke(remaining, duration);

            while (remaining > TimeSpan.Zero)
            {
                await Task.Delay(1000, ct);
                remaining -= TimeSpan.FromSeconds(1);
                if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                onTick?.Invoke(remaining);
                onProgress?.Invoke(remaining, duration);
            }
        }

        private async Task RunStep2CountdownDynamicAsync(
            Func<TimeSpan> getDynamicDuration,
            Action<TimeSpan>? onTick,
            Action<TimeSpan, TimeSpan>? onProgress,
            CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                TimeSpan remaining;
                TimeSpan currentTotal;
                lock (_sync)
                {
                    var newTotal = getDynamicDuration();
                    if (newTotal > _step2Total) _step2Total = newTotal;
                    currentTotal = _step2Total;
                    remaining = _step2Total - (DateTime.UtcNow - _step2StartedAtUtc);
                    if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
                }

                onTick?.Invoke(remaining);
                onProgress?.Invoke(remaining, currentTotal);

                if (remaining <= TimeSpan.Zero)
                    break;

                await Task.Delay(1000, ct);
            }
        }

        public void Dispose()
        {
            Cancel();
        }

        public static string FormatDurationText(TimeSpan t)
        {
            if (t <= TimeSpan.Zero) return "0 sn";
            if (t.TotalSeconds < 60)
            {
                int sec = (int)Math.Round(t.TotalSeconds);
                return $"{sec} sn";
            }
            int m = (int)Math.Floor(t.TotalMinutes);
            int s = t.Seconds;
            if (t.TotalHours >= 1)
            {
                int h = (int)Math.Floor(t.TotalHours);
                m = t.Minutes;
                return s > 0 ? $"{h} sa {m} dk {s} sn" : $"{h} sa {m} dk";
            }
            return s > 0 ? $"{m} dk {s} sn" : $"{m} dk";
        }

        public static string FormatHms(TimeSpan t)
        {
            if (t <= TimeSpan.Zero) return "00:00:00";
            int h = (int)Math.Floor(t.TotalHours);
            return $"{h:00}:{t.Minutes:00}:{t.Seconds:00}";
        }

        public static string FormatStepProgress(TimeSpan total, TimeSpan remaining)
        {
            string totalStr = FormatDurationText(total);
            if (remaining <= TimeSpan.Zero)
                return $"{totalStr} / 00:00:00";
            return $"{totalStr} / {FormatHms(remaining)}";
        }
    }
}
