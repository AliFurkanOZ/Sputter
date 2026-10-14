using SputterControlApp.Core.Models;
using System.Globalization;
using System.Threading;

namespace SputterControlApp
{
    public partial class MainForm
    {
        private decimal _dcUsedWhLive;
        private bool _dcUsedWhInitialized;

        private decimal _rf1UsedWhLive;
        private bool _rf1UsedWhInitialized;

        private decimal _rf2UsedWhLive;
        private bool _rf2UsedWhInitialized;

        private readonly SemaphoreSlim _targetUsageSaveGate = new(1, 1);

        private static decimal ReadUsedWh(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            if (decimal.TryParse(text, out var value))
                return value;

            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return value;

            return 0m;
        }

        private static decimal CalculateWhForOneSecond(int powerW)
        {
            if (powerW <= 0)
                return 0m;

            return powerW / 3600m;
        }

        private async Task AddDcUsageToTargetLifeAsync(int powerW)
        {
            if (!_dcUsedWhInitialized)
                SyncDcUsedWhFromUi();

            await AddUsageToTargetLifeAsync(
                powerW,
                txtDC_Target,
                txtDC_TargetLife,
                currentValue: _dcUsedWhLive,
                setCurrentValue: value => _dcUsedWhLive = value);
        }

        private async Task AddRf1UsageToTargetLifeAsync(int powerW)
        {
            if (!_rf1UsedWhInitialized)
                SyncRf1UsedWhFromUi();

            await AddUsageToTargetLifeAsync(
                powerW,
                txtRf1_Target,
                txtRf1_TargetLife,
                currentValue: _rf1UsedWhLive,
                setCurrentValue: value => _rf1UsedWhLive = value);
        }

        private async Task AddRf2UsageToTargetLifeAsync(int powerW)
        {
            if (!_rf2UsedWhInitialized)
                SyncRf2UsedWhFromUi();

            await AddUsageToTargetLifeAsync(
                powerW,
                txtRf2_Target,
                txtRf2_TargetLife,
                currentValue: _rf2UsedWhLive,
                setCurrentValue: value => _rf2UsedWhLive = value);
        }

        private async Task AddUsageToTargetLifeAsync(
            int powerW,
            TextBox targetTextBox,
            TextBox targetLifeTextBox,
            decimal currentValue,
            Action<decimal> setCurrentValue)
        {
            if (powerW <= 0)
                return;

            decimal nextValue = currentValue + CalculateWhForOneSecond(powerW);
            setCurrentValue(nextValue);
            targetLifeTextBox.Text = nextValue.ToString("0.00");

            await SaveUsedWhToTargetsAsync(targetTextBox.Text, nextValue);
        }

        private void SyncDcUsedWhFromUi()
        {
            _dcUsedWhLive = ReadUsedWh(txtDC_TargetLife.Text);
            _dcUsedWhInitialized = true;
        }

        private void SyncRf1UsedWhFromUi()
        {
            _rf1UsedWhLive = ReadUsedWh(txtRf1_TargetLife.Text);
            _rf1UsedWhInitialized = true;
        }

        private void SyncRf2UsedWhFromUi()
        {
            _rf2UsedWhLive = ReadUsedWh(txtRf2_TargetLife.Text);
            _rf2UsedWhInitialized = true;
        }

        private async Task SaveUsedWhToTargetsAsync(string? targetName, decimal usedWh)
        {
            var normalizedTargetName = targetName?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTargetName))
                return;

            await _targetUsageSaveGate.WaitAsync();
            try
            {
                try
                {
                    var targets = await AppDataManager.LoadTargetsAsync();

                    var target = targets.FirstOrDefault(t =>
                        string.Equals(t.Name?.Trim(), normalizedTargetName, StringComparison.CurrentCultureIgnoreCase));

                    if (target == null)
                        return;

                    target.UsedWh = usedWh;
                    await AppDataManager.SaveTargetsAsync(targets);
                }
                catch (Exception ex)
                {
                    UiLog($"[TargetUsage] save error: {ex.Message}");
                }
            }
            finally
            {
                _targetUsageSaveGate.Release();
            }
        }
    }
}
