using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SputterNewApp.Core.Models;

namespace SputterNewApp.Services
{
    public sealed class TargetUsageService
    {
        private readonly SemaphoreSlim _gate = new(1, 1);

        public static decimal CalculateWhForOneSecond(int powerW)
        {
            if (powerW <= 0) return 0m;
            return powerW / 3600m;
        }

        public async Task<decimal> AddUsageAsync(string targetName, decimal currentUsedWh, int powerW)
        {
            if (powerW <= 0) return currentUsedWh;

            decimal added = CalculateWhForOneSecond(powerW);
            decimal nextTotal = currentUsedWh + added;

            var normalized = targetName?.Trim();
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                await _gate.WaitAsync().ConfigureAwait(false);
                try
                {
                    var targets = await AppDataManager.LoadTargetsAsync().ConfigureAwait(false);
                    var item = targets.FirstOrDefault(t =>
                        string.Equals(t.Name?.Trim(), normalized, StringComparison.CurrentCultureIgnoreCase));

                    if (item != null)
                    {
                        item.UsedWh = nextTotal;
                        await AppDataManager.SaveTargetsAsync(targets).ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    _gate.Release();
                }
            }

            return nextTotal;
        }
    }
}
