using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SputterControlApp.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var renderPreview = e.Args.Length == 2 && e.Args[0] == "--render-preview";
        using var bindingLog = new StringWriter();
        using var bindingListener = new TextWriterTraceListener(bindingLog);
        if (renderPreview)
        {
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            PresentationTraceSources.DataBindingSource.Listeners.Add(bindingListener);
        }
        var window = new MainWindow();
        MainWindow = window;
        // Off-screen visual verification of the actual WPF views; no device services.
        if (renderPreview)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Directory.CreateDirectory(e.Args[1]);
            foreach (var page in new[] { "Process", "Sputter", "Graph", "QCM", "Log", "Debug" })
            {
                window.SelectPage(page);
                // 1920×1080 panel minus the window title bar.
                Render((FrameworkElement)window.Content, Path.Combine(e.Args[1], page + ".png"), 1920, 1050);
                if (page == "Process") VerifyMfcAlignment((FrameworkElement)window.Content);
            }
            foreach (var page in new[] { "Process", "QCM", "Sputter", "Log" })
            {
                window.SelectPage(page);
                Render((FrameworkElement)window.Content, Path.Combine(e.Args[1], page + "-1366.png"), 1366, 738);
                if (page == "Process") VerifyMfcAlignment((FrameworkElement)window.Content);
            }
            foreach (var (name, dialog, width, height) in new (string, Window, int, int)[]
            {
                ("Connection", new Views.ConnectionWindow(), 544, 481),
                ("ManualRead", new Views.ManualReadWindow(), 704, 561),
                ("Targets", new Views.TargetWindow(), 664, 441),
                ("Material", new Views.EditorWindow("Material"), 544, 481),
                ("Layer", new Views.EditorWindow("Layer"), 544, 481),
                ("CoDep", new Views.EditorWindow("CoDep"), 544, 481)
            })
            {
                Render((FrameworkElement)dialog.Content, Path.Combine(e.Args[1], name + ".png"), width, height);
            }
            VerifyValveIndicator(e.Args[1]);
            File.WriteAllText(Path.Combine(e.Args[1], "readability-checks.txt"),
                "PASS: RF1/RF2 MFC heading alignment at 1920 and 1366 px.\n" +
                "PASS: valve state transitions unknown / open / closed / unknown, including accessible labels.\n");
            bindingListener.Flush();
            File.WriteAllText(Path.Combine(e.Args[1], "binding-errors.txt"), bindingLog.ToString());
            PresentationTraceSources.DataBindingSource.Listeners.Remove(bindingListener);
            Shutdown(bindingLog.GetStringBuilder().Length == 0 ? 0 : 1);
            return;
        }
        window.Show();
    }

    private static void VerifyMfcAlignment(FrameworkElement view)
    {
        var cards = Descendants(view).OfType<Controls.SourceCard>()
            .Where(card => card.DataContext is Models.SourceState { IsDc: false }).ToArray();
        if (cards.Length != 2) throw new InvalidOperationException("Both RF cards must be visible.");
        var positions = cards.Select(card => ((FrameworkElement)card.FindName("MfcSection"))
            .TransformToAncestor(view).Transform(new Point()).Y).ToArray();
        if (Math.Abs(positions[0] - positions[1]) > 1)
            throw new InvalidOperationException("RF MFC headings are not aligned.");
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            yield return child;
            foreach (var descendant in Descendants(child)) yield return descendant;
        }
    }

    private static void VerifyValveIndicator(string output)
    {
        var state = new Models.ValveState();
        var indicator = new Controls.ValveIndicator { DataContext = state };
        indicator.SetBinding(Controls.ValveIndicator.IsOpenProperty, new System.Windows.Data.Binding("IsOpen"));
        foreach (var (value, label, file) in new (bool?, string, string)[]
        {
            (null, "BİLİNMİYOR", "unknown"), (true, "AÇIK", "open"),
            (false, "KAPALI", "closed"), (null, "BİLİNMİYOR", "disconnected")
        })
        {
            state.IsOpen = value;
            Render(indicator, Path.Combine(output, "Valve-" + file + ".png"), 150, 40);
            if (((System.Windows.Controls.TextBlock)indicator.FindName("StateText")).Text != label ||
                System.Windows.Automation.AutomationProperties.GetName(indicator) != label)
                throw new InvalidOperationException("Incorrect valve state label: " + file);
        }
    }

    private static void Render(FrameworkElement view, string file, int width, int height)
    {
        // Settle viewport bindings and DataGrid column widths before capturing.
        for (var pass = 0; pass < 4; pass++)
        {
            view.Measure(new Size(width, height));
            view.Arrange(new Rect(0, 0, width, height));
            view.UpdateLayout();
            view.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ContextIdle);
        }
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        var background = new DrawingVisual();
        using (var drawing = background.RenderOpen())
            drawing.DrawRectangle((Brush)Current.FindResource("ScreenBrush"), null, new Rect(0, 0, width, height));
        bitmap.Render(background);
        bitmap.Render(view);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(file);
        encoder.Save(stream);
    }
}
