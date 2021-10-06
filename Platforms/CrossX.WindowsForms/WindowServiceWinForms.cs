using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;

namespace CrossX.WindowsForms
{
    internal class WindowServiceWinForms: WindowService
    {
        private readonly List<WindowHost> windows = new List<WindowHost>();
        private readonly IObjectFactory objectFactory;
        private readonly IDispatcher dispatcher;

        public IReadOnlyList<WindowHost> Windows => windows;

        public WindowServiceWinForms(IObjectFactory objectFactory, IXxFileParser xxFileParser, IViewLocator viewLocator, IDispatcher dispatcher) : base(objectFactory, xxFileParser, viewLocator)
        {
            this.objectFactory = objectFactory;
            this.dispatcher = dispatcher;
        }

        public override void ShowWindow(Window window) => ShowWindow(window, false);

        private void ShowWindow(Window window, bool modal)
        {
            var host = new WindowHost(window, objectFactory);
            host.Disposed += Host_Disposed;
            windows.Add(host);

            if (modal)
            {
                host.ShowDialog();
            }
            else
            {
                host.Show();
            }
        }

        private void Host_Disposed(object sender, EventArgs _)
        {
            if(sender is WindowHost wh)
            {
                wh.Disposed -= Host_Disposed;
                dispatcher.EnqueueAction(() => windows.Remove(wh));
            }
        }
    }
}
