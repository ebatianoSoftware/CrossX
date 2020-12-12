using CrossX.Async;
using CrossX.Core;
using CrossX.Data;
using CrossX.Diagnostics;
using CrossX.DxCommon;
using CrossX.DxCommon.Graphics;
using CrossX.Graphics;
using CrossX.Input;
using CrossX.IO;
using CrossX.Windows.Input;
using CrossX.WindowsDx.IO;
using CrossX.WindowsDx.Media;
using S2IoC;
using SharpDX.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
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

    public class AppRunner<TApp> : IPlatform, IDisposable where TApp : class, IApp
    {
        private readonly RenderForm renderForm;
        private readonly GraphicsMode graphicsMode;
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
        private int addX;
        private int addY;

        public AppRunner(string windowTitle, GraphicsMode graphicsMode, IServicesProvider servicesProvider = null)
        {
            DpiHelper.SetDpiAwareness();

            renderForm = new RenderForm(windowTitle);
            isActive = true;

            renderForm.AppActivated += OnActivated;
            renderForm.Activated += OnActivated;

            renderForm.AppDeactivated += OnDeactivated;
            renderForm.Deactivate += OnDeactivated;

            renderForm.KeyDown += KeyDown;
            renderForm.MinimizeBox = true;
            renderForm.MaximizeBox = false;
            renderForm.ShowInTaskbar = true;

            renderForm.MouseLeave += (o, e) => Cursor.Show();
            renderForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            
            var clientSize = renderForm.ClientSize;
            var windowSize = renderForm.Size;

            addX = windowSize.Width - clientSize.Width;
            addY = windowSize.Height - clientSize.Height;

            var maxSize = Screen.PrimaryScreen.WorkingArea.Size;
            maxSize.Width -= addX;
            maxSize.Height -= addY;

            graphicsMode.WindowSize = new Size(
                Math.Min(graphicsMode.WindowSize.Width, maxSize.Width),
                Math.Min(graphicsMode.WindowSize.Height, maxSize.Height)
                );

            var size = graphicsMode.WindowSize;
            renderForm.Size = new Size(addX + size.Width, addY + size.Height);

            targetWindow = new TargetWindow(renderForm);
            this.graphicsMode = graphicsMode;
            this.servicesProvider = Initialize(targetWindow, false, servicesProvider);
            dxGraphicsDevice.SetFullscreen(graphicsMode.Fullscreen);
            renderForm.IsFullscreen = graphicsMode.Fullscreen;
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            graphicsMode.Fullscreen = false;
            dxGraphicsDevice.SetFullscreen(false);
            renderForm.IsFullscreen = false;
            renderForm.Size = new Size(addX + graphicsMode.WindowSize.Width, addY + graphicsMode.WindowSize.Height);
            renderForm.ShowInTaskbar = true;
            renderForm.Show();
            isActive = false;
        }

        private void OnActivated(object sender, EventArgs e)
        {
            isActive = true;
        }

        private void KeyDown(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.F4)
            {
                graphicsMode.Fullscreen = !graphicsMode.Fullscreen;
                dxGraphicsDevice.SetFullscreen(graphicsMode.Fullscreen);
                renderForm.IsFullscreen = graphicsMode.Fullscreen;
                renderForm.ShowInTaskbar = true;
                renderForm.Show();
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
                    Monitor.Wait(locker, Math.Max(1, 100));
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
                .WithInstance(this).As<IPlatform>()
                .WithType<GdiImagesLoader>().As<IImageLoader>().As<IRawLoader<RawImage>>().AsSingleton()
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

        public void CloseApp()
        {
            renderForm.Close();
            Environment.Exit(0);
        }
    }
}
