using System.Windows;
using System.Windows.Controls;

namespace SputterNewApp.Controls;

// Read-only UI status. Null must never be presented as a confirmed closed valve.
public partial class ValveIndicator : UserControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool?), typeof(ValveIndicator), new PropertyMetadata(null));

    public bool? IsOpen
    {
        get => (bool?)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public ValveIndicator() => InitializeComponent();
}
