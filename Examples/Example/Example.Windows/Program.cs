using CrossX.WindowsForms;
using Example.Core;
using System;
using System.Threading.Tasks;

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
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            var host = new ApplicationHost();
            host.Run(new App());
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            Console.WriteLine(args.Exception);
        }
    }
}
