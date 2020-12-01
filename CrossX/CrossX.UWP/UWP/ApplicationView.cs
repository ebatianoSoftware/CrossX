using CrossX.Async;
using CrossX.Core;
using CrossX.Diagnostics;
using CrossX.DxCommon.Graphics;
using CrossX.Graphics;
using CrossX.Input;
using CrossX.IO;
using CrossX.IoC;
using CrossX.UWP.Graphics;
using CrossX.UWP.IO;
using CrossX.WindowsUniversal.Input;
using System;
using System.Diagnostics;
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
        private UwpGamePads gamePads;
        private UwpKeyboard keyboard;
        private UwpMouse mouse;
        private UwpTouchPanel touchPanel;
        private DxGraphicsDevice graphicsDevice;
        private Dispatcher dispatcher = new Dispatcher();

        private AppStats AppStats { get; } = new AppStats();

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
            graphicsDevice = new DxGraphicsDevice(AppStats);
            graphicsDevice.Initialize(new UwpWindow(window), false);

            gamePads = new UwpGamePads();
            keyboard = new UwpKeyboard(window);
            mouse = new UwpMouse(window);
            touchPanel = new UwpTouchPanel(window);

            var featuresFlags = new FeaturesFlags();
            featuresFlags.Add("UWP");
            
            if(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Xbox"))
            {
                featuresFlags.Add("XBOX");
                featuresFlags.Add("CONSOLE");
            }
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Contains("Windows"))
            {
                featuresFlags.Add("WINDOWS");
                featuresFlags.Add("DESKTOP");
            }

            scopeBuilder
                .WithInstance(graphicsDevice).As<IGraphicsDevice>().As<DxGraphicsDevice>()
                .WithInstance(gamePads).As<IGamePads>()
                .WithInstance(touchPanel).As<ITouchPanel>()
                .WithInstance(keyboard).As<IKeyboard>()
                .WithInstance(mouse).As<IMouse>()
                .WithInstance(featuresFlags).As<IFeaturesFlags>()
                .WithInstance(dispatcher).As<IDispatcher>()
                .WithInstance(AppStats).As<IAppStats>()
                .WithType<Storage>().As<IStorage>().AsSingleton();

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

            var fpses = new float[20];
            var fpsIndex = 0;

            while (true)
            {
                CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

                dispatcher.Process();

                var current = stopWatch.Elapsed;
                var ellapsed = current - lastTime;
                lastTime = current;

                fpses[fpsIndex] = 1.0f / (float)Math.Max(0.000000001, ellapsed.TotalSeconds);
                fpsIndex = (fpsIndex + 1) % 20;

                float fps = 0.0f;
                for(var idx =0; idx < 20; ++idx)
                {
                    fps += fpses[idx];
                }
                fps /= 20.0f;
                AppStats.Fps = fps;

                gamePads.Update();

                app.Update(ellapsed);

                graphicsDevice.BeginRender();
                app.Draw(ellapsed);

                keyboard.Update();
                mouse.Update();
                touchPanel.Update();
            }

            //stopWatch.Stop();
        }

        public void Uninitialize()
        {
            
        }
    }
}
