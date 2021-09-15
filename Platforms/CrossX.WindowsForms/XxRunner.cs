using System;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    public static class XxRunner
    {
        public static void Run(Framework.Core.Application application)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var mainForm = new MainForm(application))
            {
                Application.Run(mainForm);
            }
        }
    }
}
