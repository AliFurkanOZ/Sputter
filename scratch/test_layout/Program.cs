using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestLayout
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new Application();
            app.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/SputterNewApp;component/Styles/Theme.xaml", UriKind.Absolute)
            });

            var win = new SputterNewApp.MainWindow();
            win.Width = 1920;
            win.Height = 1080;
            win.WindowState = WindowState.Normal;
            win.Show();
            win.UpdateLayout();

            Console.WriteLine("=== WINDOW MEASUREMENTS ===");
            Console.WriteLine($"Window RenderSize: {win.ActualWidth} x {win.ActualHeight}");

            var pages = win.FindName("Pages") as TabControl;
            if (pages != null)
            {
                var tabItem = pages.Items[0] as TabItem;
                if (tabItem != null && tabItem.Content is Grid processGrid)
                {
                    Console.WriteLine("\n=== PROCESS TAB GRID ===");
                    Console.WriteLine($"Process Grid ActualHeight: {processGrid.ActualHeight:F1}");
                    Console.WriteLine($"Process Grid Row 0 (Cards): {processGrid.RowDefinitions[0].ActualHeight:F1}");
                    Console.WriteLine($"Process Grid Row 1 (Info):  {processGrid.RowDefinitions[1].ActualHeight:F1}");

                    Console.WriteLine("\n=== MFC CONTROL ALIGNMENT CHECK ===");
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(processGrid); i++)
                    {
                        var child = VisualTreeHelper.GetChild(processGrid, i) as FrameworkElement;
                        if (child is SputterNewApp.Controls.SourceCard card)
                        {
                            var header = card.DataContext?.GetType().GetProperty("Name")?.GetValue(card.DataContext) ?? "Source";
                            var pt = card.TranslatePoint(new Point(0, 0), win);
                            Console.WriteLine($"Card [{header}] Pos: ({pt.X:F1}, {pt.Y:F1}) Size: ({card.ActualWidth:F1} x {card.ActualHeight:F1})");
                            
                            // Find MFC TextBlock, Argon row, Oxygen row
                            FindMfcDetails(card, win, header.ToString() ?? "");
                        }
                    }

                    Console.WriteLine("\n=== ROW 1 (BOTTOM) CARDS CHECK ===");
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(processGrid); i++)
                    {
                        var child = VisualTreeHelper.GetChild(processGrid, i) as FrameworkElement;
                        if (child is GroupBox gb && (gb.Header?.ToString()?.Contains("Info") == true))
                        {
                            var pt = gb.TranslatePoint(new Point(0, 0), win);
                            Console.WriteLine($"GroupBox [{gb.Header}] Pos: ({pt.X:F1}, {pt.Y:F1}) Size: ({gb.ActualWidth:F1} x {gb.ActualHeight:F1}) Bottom: {pt.Y + gb.ActualHeight:F1}");
                        }
                    }

                    // Test Sputter Tab
                    win.SelectPage("Sputter");
                    win.UpdateLayout();
                    Console.WriteLine("\n=== SPUTTER TAB CHECK ===");
                    Console.WriteLine($"Sputter tab selected successfully. SelectedIndex: {pages.SelectedIndex}");
                }
            }

            win.Close();
        }

        static void FindMfcDetails(DependencyObject parent, UIElement root, string cardName)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBlock tb && tb.Text == "MFC Control")
                {
                    var pt = tb.TranslatePoint(new Point(0, 0), root);
                    Console.WriteLine($"   [{cardName}] 'MFC Control' Header Y: {pt.Y:F1}");
                }
                if (child is TextBlock tbAr && tbAr.Text == "Ar")
                {
                    var pt = tbAr.TranslatePoint(new Point(0, 0), root);
                    Console.WriteLine($"   [{cardName}] 'Ar' Row Y: {pt.Y:F1}");
                }
                if (child is TextBlock tbO2 && tbO2.Text == "O₂")
                {
                    var pt = tbO2.TranslatePoint(new Point(0, 0), root);
                    Console.WriteLine($"   [{cardName}] 'O₂' Row Y: {pt.Y:F1}");
                }
                FindMfcDetails(child, root, cardName);
            }
        }
    }
}
