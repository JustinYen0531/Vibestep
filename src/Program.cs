using System;
using System.Threading;
using System.Windows.Forms;

namespace Vibestep
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            bool isFirstInstance;
            using (Mutex mutex = new Mutex(true, "Local\\Vibestep.SingleInstance", out isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    MessageBox.Show(
                        "Vibestep 已經在執行中。請查看 Windows 右下角通知區。",
                        "Vibestep",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new VibestepApplicationContext());
                GC.KeepAlive(mutex);
            }
        }
    }
}

