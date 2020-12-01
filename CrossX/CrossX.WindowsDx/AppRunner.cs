using CrossX.Async;
using CrossX.Core;
using CrossX.Diagnostics;
using CrossX.DxCommon;
using CrossX.DxCommon.Graphics;
using CrossX.Graphics;
using CrossX.Input;
using CrossX.IO;
using CrossX.IoC;
using CrossX.Windows.Input;
using CrossX.WindowsDx.IO;
using SharpDX.Windows;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CrossX.WindowsDx
{
    static class DpiHelper
    {
        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }


        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        public static void SetDpiAwareness()
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
                }
            }
            catch (EntryPointNotFoundException)//this exception occures if OS does not implement this API, just ignore it.
            {
            }
        }
    }

    public class AppRunner<TApp>: IDisposable where TApp: class, IApp
    {
        private readonly RenderForm renderForm;
        private IServicesProvider servicesProvider;
        private object locker = new object();
        private IApp app;

        private bool isActive;
        private DxGraphicsDevice dxGraphicsDevice;
        private Win32Mouse win32Mouse;
        private Win32Keyboard win32Keyboard;
        private Win32Gamepads win32GamePads;

        private Stopwatch stopWatch;
        private TimeSpan lastTime;

        private float[] fpses = new float[20];
        private int fpsIndex = 0;

        private AppStats appStats = new AppStats();
        private Dispatcher dispatcher = new Dispatcher();
        private TargetWindow targetWindow;
        
        public AppRunner(string windowTitle, GraphicsMode graphicsMode, IServicesProvider servicesProvider = null)
        {
            DpiHelper.SetDpiAwareness();

            renderForm = new RenderForm(windowTitle);
            isActive = true;

            //renderForm.AppActivated += OnActivated;
            //renderForm.Activated += OnActivated;

            //renderForm.AppDeactivated += OnDeactivated;
            //renderForm.Deactivate += OnDeactivated;

            renderForm.KeyDown += KeyDown;
            renderForm.MinimizeBox = true;
            renderForm.MaximizeBox = false;
            renderForm.ShowInTaskbar = true;

            renderForm.MouseLeave += (o, e) => Cursor.Show();

            renderForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            
            var clientSize = renderForm.ClientSize;
            var windowSize = renderForm.Size;

            var addX = windowSize.Width - clientSize.Width;
            var addY = windowSize.Height - clientSize.Height;

            renderForm.Size = new System.Drawing.Size(addX + graphicsMode.Width, addY + graphicsMode.Height);

            targetWindow = new TargetWindow(renderForm);
            this.servicesProvider = Initialize(targetWindow, graphicsMode.Fullscreen, servicesProvider);
            
        }

        private void KeyDown(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.F4)
            {
                targetWindow.SetFullscreen(targetWindow.IsFullscreen);
            }
        }

        public void Dispose()
        {
            dxGraphicsDevice.Dispose();
        }

        public void Run(object appParameters = null)
        {
            app = servicesProvider.GetService<IObjectFactory>().Create<TApp>(appParameters);
            app.LoadContent();

            stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            lastTime = stopWatch.Elapsed;
            RenderLoop.Run(renderForm, Tick);
        }

        private void Tick()
        {
            dispatcher.Process();
            win32GamePads.Update();

            var current = stopWatch.Elapsed;
            var ellapsed = current - lastTime;
            lastTime = current;

            fpses[fpsIndex] = 1.0f / (float)Math.Max(0.000000001, ellapsed.TotalSeconds);
            fpsIndex = (fpsIndex + 1) % 20;

            float fps = 0.0f;
            for (var idx = 0; idx < 20; ++idx)
            {
                fps += fpses[idx];
            }
            fps /= 20.0f;
            appStats.Fps = fps;

            app.Update(ellapsed);

            win32Keyboard.Update();
            win32Mouse.Update();

            dxGraphicsDevice.BeginRender();
            app.Draw(ellapsed);
            
            if (!isActive)
            {
                lock (locker)
                {
                    Monitor.Wait(locker, Math.Max(1, 10));
                }
            }
        }

        private IServicesProvider Initialize(TargetWindow targetWindow, bool fullscreen, IServicesProvider servicesProvider)
        {
            var scopeBuilder = new ScopeBuilder();
            if (servicesProvider != null)
            {
                scopeBuilder.WithParent(servicesProvider);
            }

            scopeBuilder
                .RegisterDirectXServices()
                .RegisterDirectXTypes();

            dxGraphicsDevice = new DxGraphicsDevice(appStats);
            dxGraphicsDevice.Initialize(targetWindow, fullscreen);

            var renderStates = new RenderStates(dxGraphicsDevice);
            renderStates.Initialize();

            win32Mouse = new Win32Mouse(renderForm);
            win32Keyboard = new Win32Keyboard(renderForm);
            win32GamePads = new Win32Gamepads();

            var featuresFlags = new FeaturesFlags();
            featuresFlags.Add("WINDOWS");
            featuresFlags.Add("DESKTOP");

            scopeBuilder
                .WithInstance(dxGraphicsDevice).As<IGraphicsDevice>().As<DxGraphicsDevice>()
                .WithInstance(win32GamePads).As<IGamePads>()
                .WithInstance(win32Keyboard).As<IKeyboard>()
                .WithInstance(win32Mouse).As<IMouse>().As<ITouchPanel>()
                .WithInstance(featuresFlags).As<IFeaturesFlags>()
                .WithInstance(dispatcher).As<IDispatcher>()
                .WithInstance(appStats).As<IAppStats>()
                .WithType<Storage>().As<IStorage>().AsSingleton();


            return scopeBuilder.Build();
        }
    }
}
