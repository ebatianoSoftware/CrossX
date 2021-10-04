using System.Threading;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    public static class XxRunner
    {
        public static void Run(Framework.Core.Application application)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            using (var mainForm = new MainForm(application))
            {
                mainForm.Show();
                while (!mainForm.IsDisposed)
                {
                    mainForm.MainLoop.ProcessSystemDispatcher();
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
        }
    }
}
