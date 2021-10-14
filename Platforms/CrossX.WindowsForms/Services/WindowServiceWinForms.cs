using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;

namespace CrossX.WindowsForms.Services
{
    internal class WindowServiceWinForms : WindowService
    {
        public WindowHost MainWindowHost { get; private set; }

        private readonly List<WindowHost> windows = new List<WindowHost>();
        private readonly IDispatcher dispatcher;
        private readonly ISequencer sequencer;

        public IReadOnlyList<WindowHost> Windows => windows;

        public WindowServiceWinForms(IXxFileParser xxFileParser, IViewLocator viewLocator, IDispatcher dispatcher,
            ISequencer sequencer, AppColorTable appColorTable) : base(xxFileParser, viewLocator)
        {
            this.dispatcher = dispatcher;
            this.sequencer = sequencer;
        }

        private IEnumerable<Sequence> ShowWindow(WindowHost host)
        {
            host.Opacity = 0;
            host.Show();

            yield return Sequence.WaitForNextFrame();
            host.Opacity = 1;
        }

        protected override INativeWindow CreateNativeWindow(Window window, CreateWindowMode createMode)
        {
            var host = new WindowHost(window, objectFactory);

            switch (createMode)
            {
                case CreateWindowMode.MainWindow:
                    var oldMain = MainWindow;
                    MainWindowHost = host;
                    MainWindowHost.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    MainWindow = window;
                    oldMain?.Close();
                    GC.Collect(2);
                    break;

                case CreateWindowMode.Global:
                    if (MainWindowHost == null)
                    {
                        MainWindowHost = host;
                        MainWindow = window;
                        host.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    }
                    break;

                case CreateWindowMode.Modal:
                    host.ShowInTaskbar = false;
                    host.MinimizeBox = false;
                    host.ShowIcon = false;
                    MainWindowHost.AddModal(host);
                    break;
            }

            host.Disposed += Host_Disposed;
            windows.Add(host);

            sequencer.Run(ShowWindow(host));
            return host;
        }

        private void Host_Disposed(object sender, EventArgs _)
        {
            if (sender is WindowHost wh)
            {
                wh.Disposed -= Host_Disposed;
                dispatcher.EnqueueAction(() =>
                {
                    windows.Remove(wh);
                    if (MainWindowHost == wh)
                    {
                        MainWindowHost = null;
                        MainWindow = null;
                    }
                    GC.Collect(2);
                });
            }
        }
    }
}
