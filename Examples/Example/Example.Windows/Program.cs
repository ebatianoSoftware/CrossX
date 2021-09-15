using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Core;
using CrossX.WindowsForms;
using System;

namespace Example.Windows
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XxRunner.Run(new App());   
        }

        private class App : Application
        {
            protected override void StartApp()
            {
                Canvas.Clear(Color.CornflowerBlue);
            }
        }
    }
}
