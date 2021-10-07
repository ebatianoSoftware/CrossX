using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Core;
using CrossX.Framework.IoC;
using CrossX.Skia;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FormsApplication = System.Windows.Forms.Application;

namespace CrossX.WindowsForms
{
    public class ApplicationHost
    {
        private readonly Dispatcher dispatcher = new Dispatcher();
        private readonly Sequencer sequencer = new Sequencer();

        public void Run(ICoreApplication application, IServicesProvider servicesProvider = null)
        {
            FormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
            FormsApplication.SetCompatibleTextRenderingDefault(false);

            Sequence.CreateSequenceFunc = SequenceImpl.Create;
            Sequence.NextFrameSequence = SequenceImpl.Create(0, 0, null, null);

            dispatcher.DispatcherThread = Thread.CurrentThread;

            using (var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                UiUnit.PixelsPerUnit = graphics.DpiX / 96f;
            }

            var scopeBuilder = new ScopeBuilder(servicesProvider);
            scopeBuilder.WithSkia()
                        .WithCrossTypes()
                        .WithInstance(dispatcher).As<ISystemDispatcher>().As<IDispatcher>()
                        .WithInstance(sequencer).As<ISequencer>();

            
            var services = scopeBuilder.Build();

            application.AfterInitServices += (s,b) =>
            {
                b.WithType<WindowServiceWinForms>().AsSelf().As<IWindowsService>().AsSingleton();
            };

            services = application.Initialize(services);
            application.Load();

            var windowsService = services.GetService<WindowServiceWinForms>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            TimeSpan lastUpdateTimeSpan = stopwatch.Elapsed;

            while (windowsService.Windows.Count > 0)
            {
                TimeSpan currentTimeSpan = stopwatch.Elapsed;
                TimeSpan timeDelta = currentTimeSpan - lastUpdateTimeSpan;
                lastUpdateTimeSpan = currentTimeSpan;

                dispatcher.Process();
                sequencer.Update(timeDelta);

                for(var idx = 0; idx < windowsService.Windows.Count; ++idx)
                {
                    var hostWindow = windowsService.Windows[idx];

                    hostWindow.Window.Update((float)timeDelta.TotalSeconds);
                    if(hostWindow.Window.IsDirty)
                    {
                        hostWindow.Redraw();
                    }
                }

                FormsApplication.DoEvents();
                Thread.Sleep(1);
            }
        }
    }
}
