﻿using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;

namespace CrossX.WindowsForms
{
    internal class WindowServiceWinForms: WindowService
    {
        public WindowHost MainWindowHost { get; private set; }

        private readonly List<WindowHost> windows = new List<WindowHost>();
        private readonly IObjectFactory objectFactory;
        private readonly IDispatcher dispatcher;

        public IReadOnlyList<WindowHost> Windows => windows;

        public WindowServiceWinForms(IObjectFactory objectFactory, IXxFileParser xxFileParser, IViewLocator viewLocator, IDispatcher dispatcher) : base(objectFactory, xxFileParser, viewLocator)
        {
            this.objectFactory = objectFactory;
            this.dispatcher = dispatcher;
        }

        public override void ShowWindow(Window window, CreateWindowMode createMode)
        {
            var host = new WindowHost(window, objectFactory, createMode);

            switch(createMode)
            {
                case CreateWindowMode.MainWindow:
                    MainWindowHost = host;
                    MainWindow = window;
                    break;

                case CreateWindowMode.Global:
                    if (MainWindowHost == null)
                    {
                        MainWindowHost = host;
                        MainWindow = window;
                    }
                    break;

                case CreateWindowMode.ChildToMain:
                    host.ShowInTaskbar = false;
                    host.MinimizeBox = false;
                    host.ShowIcon = false;
                    MainWindowHost.AddChild(host);
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

            host.Show();
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
