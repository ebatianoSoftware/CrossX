using CrossX.WindowsForms;
using Example.Core;
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
    }
}
