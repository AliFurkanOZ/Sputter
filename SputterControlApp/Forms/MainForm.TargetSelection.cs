using SputterControlApp.Core.Models;

namespace SputterControlApp
{
    public partial class MainForm : Form
    {
        private void LoadSavedTargetSelections()
        {
            var settings = Properties.Settings.Default;

            txtDC_Target.Text = settings.LastDcTargetName ?? "";
            txtRf1_Target.Text = settings.LastRf1TargetName ?? "";
            txtRf2_Target.Text = settings.LastRf2TargetName ?? "";
        }

        private async Task LoadTargetLifeValuesFromTargetsAsync()
        {
            var targets = await AppDataManager.LoadTargetsAsync();

            ApplyTargetLifeFromStore(targets, txtDC_Target, txtDC_TargetLife);
            ApplyTargetLifeFromStore(targets, txtRf1_Target, txtRf1_TargetLife);
            ApplyTargetLifeFromStore(targets, txtRf2_Target, txtRf2_TargetLife);
        }
        private static void ApplyTargetLifeFromStore(
            List<TargetItem> targets,
            TextBox targetNameTextBox,
            TextBox targetLifeTextBox)
        {
            var targetName = targetNameTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(targetName))
            {
                targetLifeTextBox.Text = "0.00";
                return;
            }

            var target = targets.FirstOrDefault(t =>
                string.Equals(t.Name?.Trim(), targetName, StringComparison.CurrentCultureIgnoreCase));

            targetLifeTextBox.Text = target != null
                ? target.UsedWh.ToString("0.00")
                : "0.00";
        }




        private void WireTargetSelectionButtons()
        {
            btnSelectDcTarget.Click -= btnSelectDcTarget_Click;
            btnSelectDcTarget.Click += btnSelectDcTarget_Click;

            btnSelectRf1Target.Click -= btnSelectRf1Target_Click;
            btnSelectRf1Target.Click += btnSelectRf1Target_Click;

            btnSelectRf2Target.Click -= btnSelectRf2Target_Click;
            btnSelectRf2Target.Click += btnSelectRf2Target_Click;
        }

        private async void btnSelectDcTarget_Click(object? sender, EventArgs e)
        {
            if (!CanChangeTarget(btnSelectDcTarget, "DC"))
                return;

            await ShowTargetSelectionAsync(txtDC_Target, txtDC_TargetLife);
        }

        private async void btnSelectRf1Target_Click(object? sender, EventArgs e)
        {
            if (!CanChangeTarget(btnSelectRf1Target, "RF1"))
                return;

            await ShowTargetSelectionAsync(txtRf1_Target, txtRf1_TargetLife);
        }

        private async void btnSelectRf2Target_Click(object? sender, EventArgs e)
        {
            if (!CanChangeTarget(btnSelectRf2Target, "RF2"))
                return;

            await ShowTargetSelectionAsync(txtRf2_Target, txtRf2_TargetLife);
        }

        private async Task ShowTargetSelectionAsync(TextBox targetTextBox, TextBox lifeTextBox)
        {
            try
            {
                var unavailableTargetNames = GetUnavailableTargetNames(targetTextBox);
                var selectedTarget = await TargetSelectForm.SelectTargetAsync(
                    this,
                    targetTextBox.Text,
                    ParseTargetLife(lifeTextBox.Text),
                    unavailableTargetNames);

                if (selectedTarget == null)
                    return;

                if (IsTargetAssignedElsewhere(targetTextBox, selectedTarget.Name))
                {
                    MessageBox.Show(
                        $"'{selectedTarget.Name}' zaten diger target alaninda secili.",
                        "Target Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ApplySelectedTarget(targetTextBox, lifeTextBox, selectedTarget);
                if (ReferenceEquals(targetTextBox, txtDC_Target))
                {
                    SyncDcUsedWhFromUi();
                }
                else if (ReferenceEquals(targetTextBox, txtRf1_Target))
                {
                    SyncRf1UsedWhFromUi();
                }
                else if (ReferenceEquals(targetTextBox, txtRf2_Target))
                {
                    SyncRf2UsedWhFromUi();
                }

                SaveCurrentTargetSelections();
            }
            catch (Exception ex)
            {
                HandleUiException(ex);
            }
        }

        private static void ApplySelectedTarget(TextBox targetTextBox, TextBox lifeTextBox, TargetItem selectedTarget)
        {
            targetTextBox.Text = selectedTarget.Name;
            lifeTextBox.Text = selectedTarget.UsedWh.ToString("0.00");
        }

        private bool CanChangeTarget(Button button, string sourceName)
        {
            if (button.Enabled)
                return true;

            MessageBox.Show(
                $"{sourceName} calisirken target degistirilemez.",
                "Target Selection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        private void UpdateTargetSelectionLocks()
        {
            btnSelectDcTarget.Enabled = !IsSourceRunning(btnControlDC);
            btnSelectRf1Target.Enabled = !IsSourceRunning(btnControlRf1);
            btnSelectRf2Target.Enabled = !IsSourceRunning(btnControlRf2);
        }

        private static bool IsSourceRunning(Button sourceButton)
            => sourceButton.Tag is bool state && state;

        private string[] GetUnavailableTargetNames(TextBox activeTargetTextBox)
        {
            return GetAllTargetTextBoxes()
                .Where(tb => !ReferenceEquals(tb, activeTargetTextBox))
                .Select(tb => NormalizeTargetName(tb.Text))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }

        private bool IsTargetAssignedElsewhere(TextBox activeTargetTextBox, string? selectedTargetName)
        {
            var normalized = NormalizeTargetName(selectedTargetName);
            if (string.IsNullOrWhiteSpace(normalized))
                return false;

            return GetAllTargetTextBoxes()
                .Where(tb => !ReferenceEquals(tb, activeTargetTextBox))
                .Select(tb => NormalizeTargetName(tb.Text))
                .Any(name => string.Equals(name, normalized, StringComparison.Ordinal));
        }

        private IEnumerable<TextBox> GetAllTargetTextBoxes()
        {
            yield return txtDC_Target;
            yield return txtRf1_Target;
            yield return txtRf2_Target;
        }

        private static string NormalizeTargetName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return string.Join(" ", value.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                .ToUpperInvariant();
        }

        private void SaveCurrentTargetSelections()
        {
            var settings = Properties.Settings.Default;

            settings.LastDcTargetName = txtDC_Target.Text?.Trim() ?? "";
            settings.LastRf1TargetName = txtRf1_Target.Text?.Trim() ?? "";
            settings.LastRf2TargetName = txtRf2_Target.Text?.Trim() ?? "";
            settings.Save();

        }

        private static decimal? ParseTargetLife(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (decimal.TryParse(text, out var value))
                return value;

            if (decimal.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
                return value;

            return null;
        }
    }
}
