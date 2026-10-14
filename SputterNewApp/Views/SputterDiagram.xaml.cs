using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SputterNewApp.Models;
using SputterNewApp.Services;

namespace SputterNewApp.Views
{
    public partial class SputterDiagram : UserControl
    {
        public static readonly DependencyProperty IsControlModeProperty =
            DependencyProperty.Register(
                nameof(IsControlMode),
                typeof(bool),
                typeof(SputterDiagram),
                new PropertyMetadata(false));

        public bool IsControlMode
        {
            get => (bool)GetValue(IsControlModeProperty);
            set => SetValue(IsControlModeProperty, value);
        }

        private readonly DoubleAnimation _turbAnim = new(0, 360, new Duration(TimeSpan.FromSeconds(0.6)))
        {
            RepeatBehavior = RepeatBehavior.Forever
        };
        private readonly DoubleAnimation _subAnim = new(0, 360, new Duration(TimeSpan.FromSeconds(2.0)))
        {
            RepeatBehavior = RepeatBehavior.Forever
        };

        private WorkspaceState? _hookedState;

        public SputterDiagram()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) => HookState();
        private void OnUnloaded(object sender, RoutedEventArgs e) => UnhookState();
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => HookState();

        private void HookState()
        {
            var st = State;
            if (st == _hookedState) return;

            UnhookState();

            if (st != null)
            {
                _hookedState = st;
                _hookedState.PropertyChanged += OnStatePropertyChanged;
                UpdateAnimations();
            }
        }

        private void UnhookState()
        {
            if (_hookedState != null)
            {
                _hookedState.PropertyChanged -= OnStatePropertyChanged;
                _hookedState = null;
            }
        }

        private void OnStatePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceState.IsTurboSpinning) ||
                e.PropertyName == nameof(WorkspaceState.IsRotationSpinning))
            {
                Dispatcher.Invoke(UpdateAnimations);
            }
        }

        private void UpdateAnimations()
        {
            if (_hookedState != null)
            {
                if (_hookedState.IsTurboSpinning)
                    TurboRotorTransform?.BeginAnimation(RotateTransform.AngleProperty, _turbAnim);
                else
                    TurboRotorTransform?.BeginAnimation(RotateTransform.AngleProperty, null);

                if (_hookedState.IsRotationSpinning)
                    SubstrateRotorTransform?.BeginAnimation(RotateTransform.AngleProperty, _subAnim);
                else
                    SubstrateRotorTransform?.BeginAnimation(RotateTransform.AngleProperty, null);
            }
        }

        private MainWindow? MainWin => Window.GetWindow(this) as MainWindow ?? (DataContext as MainWindow);
        private SputterServiceCoordinator? Coordinator => MainWin?.Coordinator;
        private WorkspaceState? State => MainWin?.State;

        private void ToggleMode_Click(object sender, RoutedEventArgs e)
        {
            IsControlMode = !IsControlMode;
        }

        private async void Shutter_Click(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlMode) return;
            if (sender is FrameworkElement elem && elem.Tag is string tag && ushort.TryParse(tag, out ushort addr))
            {
                if (Coordinator != null)
                {
                    await Coordinator.ToggleDeviceAsync(addr);
                }
            }
        }

        private async void DeviceClick(object sender, MouseButtonEventArgs e) => await Device_ClickAsync(sender, e);
        private async void Device_Click(object sender, MouseButtonEventArgs e) => await Device_ClickAsync(sender, e);

        private async System.Threading.Tasks.Task Device_ClickAsync(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlMode) return;
            if (sender is FrameworkElement elem && elem.Tag is string tag && ushort.TryParse(tag, out ushort addr))
            {
                if (Coordinator != null)
                {
                    if (addr == 110) // Chamber Vent valfi güvenlik uyarısı
                    {
                        var res = MessageBox.Show(
                            "Hazne Vent Valfini açmak/kapatmak istediğinize emin misiniz? (Vakum bozulabilir)",
                            "Vent Valfi Onayı",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
                        if (res != MessageBoxResult.Yes) return;
                    }

                    await Coordinator.ToggleDeviceAsync(addr);
                }
            }
        }

        private async void Source_Click(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlMode) return;
            if (sender is FrameworkElement elem && elem.Tag is string sourceName)
            {
                if (Coordinator == null || State == null) return;
                var source = sourceName switch
                {
                    "RF1" => State.Rf1,
                    "RF2" => State.Rf2,
                    "DC" => State.Dc,
                    _ => null
                };
                if (source == null) return;

                if (source.IsActive == true)
                {
                    var res = MessageBox.Show(
                        $"{sourceName} kaynağını durdurmak istiyor musunuz?",
                        $"{sourceName} Kapatma Onayı",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        await Coordinator.StopSourceAsync(sourceName);
                    }
                }
                else if (source.IsActive == false)
                {
                    var res = MessageBox.Show(
                        $"{sourceName} kaynağını başlatmak istiyor musunuz?",
                        $"{sourceName} Başlatma Onayı",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        await Coordinator.StartSourceAsync(sourceName);
                    }
                }
            }
        }

        private async void GateValve_Click(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlMode) return;
            if (Coordinator == null || State == null) return;

            bool isCurrentlyOpen = State.GateValve.IsOpen == true;
            short newTarget = isCurrentlyOpen ? (short)0 : (short)100;
            string actionText = isCurrentlyOpen ? "%0 (KAPAT)" : "%100 (AÇ)";

            var res = MessageBox.Show(
                $"Gate Valve hedef konumunu {actionText} yapmak istiyor musunuz?",
                "Gate Valve Kumandası",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                State.GateSet = newTarget;
                await Coordinator.SetGateValvePositionAsync(newTarget);
            }
        }
    }
}
