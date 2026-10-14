using System;
using System.Windows.Forms;

namespace SputterControlApp
{
    internal static class Program
    {
        /*
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new ConnectForm());

        }*/

        private static Mutex? _mutex;

        [STAThread]
        static void Main()
        {
            const string appMutexName = "SputterControlApp_SINGLE_INSTANCE";

            bool createdNew;
            _mutex = new Mutex(true, appMutexName, out createdNew);

            if (!createdNew)
            {
                // Uygulama zaten açık
                MessageBox.Show(
                    "Uygulama zaten çalışıyor.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConnectForm());

            _mutex.ReleaseMutex();
        }
    }
}
