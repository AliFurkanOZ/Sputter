using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SputterControlApp.Wpf.Controls;

// A static drawing for layout review. It intentionally has no live data source.
public sealed class PreviewChart : FrameworkElement
{
    private static readonly Brush PlotBrush = Frozen(Color.FromRgb(0xFF, 0xFF, 0xFF));
    private static readonly Brush LabelBrush = Frozen(Color.FromRgb(0x4F, 0x58, 0x60));
    private static readonly Brush HeaderBrush = Frozen(Color.FromRgb(0x1C, 0x21, 0x26));
    private static readonly Brush BadgeBackgroundBrush = Frozen(Color.FromRgb(0xF4, 0xF5, 0xF7));
    private static readonly Pen FramePen = FrozenPen(Color.FromRgb(0xA3, 0xAA, 0xB1), 1);
    private static readonly Pen GridPen = FrozenPen(Color.FromRgb(0xE1, 0xE4, 0xE7), 1);
    private static readonly Pen BadgePen = FrozenPen(Color.FromRgb(0xC8, 0xD0, 0xD8), 1);

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(string), typeof(PreviewChart),
        new FrameworkPropertyMetadata("Vacuum", FrameworkPropertyMetadataOptions.AffectsRender));

    public string Kind { get => (string)GetValue(KindProperty); set => SetValue(KindProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var w = ActualWidth; var h = ActualHeight;
        if (w < 120 || h < 100) return;

        double left = 86, right = w - 12, top = 26, bottom = h - 26;
        dc.DrawRectangle(PlotBrush, FramePen, new Rect(left, top, right - left, bottom - top));

        var maximum = Kind switch { "Thickness" => 1.5, "Rate" => 2.0, "Temperature" => 200.0, _ => 1.0 };
        var unit = Kind switch { "Thickness" => "kÅ", "Rate" => "Å/s", "Temperature" => "°C", _ => "Torr" };
        var title = Kind switch { "Thickness" => "Thickness", "Rate" => "Rate", "Temperature" => "Temperature", _ => "Vacuum" };

        // Top Y-Axis Unit indicator
        Label(dc, $"▲ Y: {unit}", left, top - 13, alignRight: false, isBold: true, fontSize: 11, brush: HeaderBrush);

        // Y-axis grid & labels with exact units
        for (var i = 0; i <= 5; i++)
        {
            double y = top + (bottom - top) * i / 5;
            if (i > 0 && i < 5) dc.DrawLine(GridPen, new Point(left + 1, y), new Point(right - 1, y));
            var text = Kind switch
            {
                "Vacuum" => $"1E-{i + 2} Torr",
                "Thickness" => $"{(maximum * (5 - i) / 5):0.0} kÅ",
                "Rate" => $"{(maximum * (5 - i) / 5):0.0} Å/s",
                "Temperature" => $"{(maximum * (5 - i) / 5):0} °C",
                _ => $"{maximum * (5 - i) / 5:0.0}"
            };
            Label(dc, text, left - 8, y, alignRight: true, fontSize: 11);
        }

        // X-axis grid & labels
        for (var i = 0; i <= 4; i++)
        {
            double x = left + (right - left) * i / 4;
            if (i > 0 && i < 4) dc.DrawLine(GridPen, new Point(x, top + 1), new Point(x, bottom - 1));
            Label(dc, i == 4 ? "0 s" : $"-{(4 - i) * 30} s", x, bottom + 12, alignRight: i == 4, center: i < 4, fontSize: 11);
        }

        // Curve geometry
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            for (var i = 0; i <= 120; i++)
            {
                var t = i / 120.0;
                double value = Kind switch
                {
                    "Thickness" => .08 + .75 * t,
                    "Rate" => .50 + .045 * Math.Sin(i * .4) + .018 * Math.Cos(i * .7),
                    "Temperature" => .18 + .62 * (1 - Math.Exp(-t * 3)),
                    _ => .85 * Math.Exp(-t * 4) + .04 + .01 * Math.Sin(i * .6)
                };
                var point = new Point(left + (right - left) * t, bottom - (bottom - top) * value);
                if (i == 0) ctx.BeginFigure(point, false, false); else ctx.LineTo(point, true, false);
            }
        }
        geometry.Freeze();

        // Muted, distinguishable trend pens; saturated colours stay reserved for alarms.
        var color = Kind switch
        {
            "Temperature" => Color.FromRgb(0x8A, 0x5A, 0x1F),
            "Thickness" => Color.FromRgb(0x2F, 0x6B, 0x4F),
            _ => Color.FromRgb(0x2D, 0x5B, 0x88)
        };

        dc.PushClip(new RectangleGeometry(new Rect(left, top, right - left, bottom - top)));
        dc.DrawGeometry(null, FrozenPen(color, 2), geometry);
        dc.Pop();

        // Legend Badge in top-right corner of the plot
        double badgeW = 126, badgeH = 20;
        double badgeX = right - badgeW - 8, badgeY = top + 8;
        if (badgeX > left + 10)
        {
            dc.DrawRoundedRectangle(BadgeBackgroundBrush, BadgePen, new Rect(badgeX, badgeY, badgeW, badgeH), 3, 3);
            dc.DrawEllipse(Frozen(color), null, new Point(badgeX + 12, badgeY + badgeH / 2), 4, 4);
            Label(dc, $"{title} ({unit})", badgeX + 22, badgeY + badgeH / 2, isBold: true, fontSize: 11, brush: HeaderBrush);
        }
    }

    private void Label(DrawingContext dc, string text, double x, double y, bool alignRight = false, bool center = false, double fontSize = 12, bool isBold = false, Brush? brush = null)
    {
        var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, isBold ? FontWeights.SemiBold : FontWeights.Normal, FontStretches.Normal);
        var formatted = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, fontSize, brush ?? LabelBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        var dx = alignRight ? -formatted.Width : center ? -formatted.Width / 2 : 0;
        dc.DrawText(formatted, new Point(x + dx, y - formatted.Height / 2));
    }

    private static Brush Frozen(Color color) { var brush = new SolidColorBrush(color); brush.Freeze(); return brush; }
    private static Pen FrozenPen(Color color, double thickness) { var pen = new Pen(Frozen(color), thickness); pen.Freeze(); return pen; }
}
