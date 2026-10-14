using System.Windows;
using System.Windows.Media;

namespace SputterNewApp.Controls;

// ISA-101 analog indicator: gray track, darker target band, pointer at the value.
// Lets the operator see "is this OK?" without reading the number.
public sealed class RangeBar : FrameworkElement
{
    private static readonly Brush TrackBrush = Frozen(Color.FromRgb(0xD3, 0xD7, 0xDB));
    private static readonly Brush BandBrush = Frozen(Color.FromRgb(0x9A, 0xA3, 0xAC));
    private static readonly Brush PointerBrush = Frozen(Color.FromRgb(0x1C, 0x21, 0x26));
    private static readonly Pen TrackPen = FrozenPen(Color.FromRgb(0xA3, 0xAA, 0xB1));

    public static readonly DependencyProperty MinimumProperty = Register(nameof(Minimum), 0.0);
    public static readonly DependencyProperty MaximumProperty = Register(nameof(Maximum), 100.0);
    public static readonly DependencyProperty ValueProperty = Register(nameof(Value), 0.0);
    public static readonly DependencyProperty BandLowProperty = Register(nameof(BandLow), double.NaN);
    public static readonly DependencyProperty BandHighProperty = Register(nameof(BandHigh), double.NaN);
    public static readonly DependencyProperty IsLogarithmicProperty = DependencyProperty.Register(nameof(IsLogarithmic), typeof(bool), typeof(RangeBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double BandLow { get => (double)GetValue(BandLowProperty); set => SetValue(BandLowProperty, value); }
    public double BandHigh { get => (double)GetValue(BandHighProperty); set => SetValue(BandHighProperty, value); }
    public bool IsLogarithmic { get => (bool)GetValue(IsLogarithmicProperty); set => SetValue(IsLogarithmicProperty, value); }

    public RangeBar() { Height = 14; SnapsToDevicePixels = true; }

    protected override void OnRender(DrawingContext dc)
    {
        var w = ActualWidth;
        if (w < 20) return;
        const double trackTop = 8, trackHeight = 6;
        dc.DrawRectangle(TrackBrush, TrackPen, new Rect(0.5, trackTop + 0.5, w - 1, trackHeight));
        if (!double.IsNaN(BandLow) && !double.IsNaN(BandHigh))
        {
            double x1 = X(BandLow, w), x2 = X(BandHigh, w);
            dc.DrawRectangle(BandBrush, null, new Rect(x1, trackTop + 1, Math.Max(0, x2 - x1), trackHeight - 1));
        }
        var x = X(Value, w);
        var pointer = new StreamGeometry();
        using (var ctx = pointer.Open())
        {
            ctx.BeginFigure(new Point(x - 5, 0), true, true);
            ctx.LineTo(new Point(x + 5, 0), true, false);
            ctx.LineTo(new Point(x, 7), true, false);
        }
        pointer.Freeze();
        dc.DrawGeometry(PointerBrush, null, pointer);
        dc.DrawRectangle(PointerBrush, null, new Rect(x - 1, 6, 2, trackHeight + 3));
    }

    private double X(double value, double width)
    {
        double min = Minimum, max = Maximum, v = value;
        if (IsLogarithmic)
        {
            min = Math.Log10(Math.Max(min, 1e-12)); max = Math.Log10(Math.Max(max, 1e-12)); v = Math.Log10(Math.Max(v, 1e-12));
        }
        var t = max > min ? (v - min) / (max - min) : 0;
        return 1 + Math.Clamp(t, 0, 1) * (width - 2);
    }

    private static DependencyProperty Register(string name, double fallback) => DependencyProperty.Register(name, typeof(double), typeof(RangeBar), new FrameworkPropertyMetadata(fallback, FrameworkPropertyMetadataOptions.AffectsRender));
    private static Brush Frozen(Color color) { var brush = new SolidColorBrush(color); brush.Freeze(); return brush; }
    private static Pen FrozenPen(Color color) { var pen = new Pen(Frozen(color), 1); pen.Freeze(); return pen; }
}
