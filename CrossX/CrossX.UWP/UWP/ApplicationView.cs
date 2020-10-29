using CrossX.Core;
using CrossX.DxCommon.Graphics;
using CrossX.Graphics;
using CrossX.IoC;
using CrossX.UWP.Graphics;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace CrossX.UWP.UWP
{
    public class ApplicationView<TApp>: IFrameworkView where TApp: IApp
    {
        private readonly ScopeBuilder scopeBuilder;
        private readonly object appParameters;
        private IServicesProvider serviceProvider;
        private TApp app;

        public ApplicationView(ScopeBuilder scopeBuilder, object appParameters)
        {
            this.scopeBuilder = scopeBuilder;
            this.appParameters = appParameters;

            CoreApplication.Suspending += CoreApplication_Suspending;
        }

        private void CoreApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            
        }

        public void Initialize(CoreApplicationView applicationView)
        {
            applicationView.Activated += (s, a) => CoreWindow.GetForCurrentThread().Activate();
        }

        public void SetWindow(CoreWindow window)
        {
            var graphicsDevice = new DxGraphicsDevice();
            graphicsDevice.Initialize(new UwpWindow(window), false);

            scopeBuilder
                .WithInstance(graphicsDevice).As<IGraphicsDevice>().As<DxGraphicsDevice>();

            serviceProvider = scopeBuilder.Build();
            
            app = serviceProvider.GetService<IObjectFactory>().Create<TApp>(appParameters);
        }

        public void Load(string entryPoint)
        {
            app.LoadContent();
        }

        public void Run()
        {
            var stopWatch = Stopwatch.StartNew();
            var lastTime = stopWatch.Elapsed;

            while (true)
            {
                CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

                var current = stopWatch.Elapsed;
                var ellapsed = current - lastTime;
                lastTime = current;

                app.Update(ellapsed);
                app.Draw(ellapsed);

                Task.Delay(1).Wait();
            }

            //stopWatch.Stop();
        }

        public void Uninitialize()
        {
            
        }
    }
}
