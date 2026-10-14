using System.Windows;
using System.Windows.Controls;

namespace SputterControlApp.Wpf.Controls;

public partial class ValueReadout : UserControl
{
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(ValueReadout), new PropertyMetadata(""));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(ValueReadout), new PropertyMetadata("0"));
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(ValueReadout), new PropertyMetadata(""));
    public static readonly DependencyProperty NoteProperty = DependencyProperty.Register(nameof(Note), typeof(string), typeof(ValueReadout), new PropertyMetadata("", NoteChanged));
    public static readonly DependencyProperty ValueSizeProperty = DependencyProperty.Register(nameof(ValueSize), typeof(double), typeof(ValueReadout), new PropertyMetadata(24.0));
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Unit { get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value); }
    public string Note { get => (string)GetValue(NoteProperty); set => SetValue(NoteProperty, value); }
    public double ValueSize { get => (double)GetValue(ValueSizeProperty); set => SetValue(ValueSizeProperty, value); }
    public ValueReadout() { InitializeComponent(); NoteText.Visibility = Visibility.Collapsed; }
    private static void NoteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var readout = (ValueReadout)d;
        if (readout.NoteText != null) readout.NoteText.Visibility = string.IsNullOrEmpty((string)e.NewValue) ? Visibility.Collapsed : Visibility.Visible;
    }
}
