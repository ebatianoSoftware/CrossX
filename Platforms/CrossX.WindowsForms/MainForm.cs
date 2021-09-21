using CrossX.Abstractions.IoC;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Core;
using CrossX.Framework.IoC;
using CrossX.Framework.UI.Global;
using CrossX.Skia;
using SkiaSharp.Views.Desktop;
using System;
using System.Windows.Forms;
using DrawingSize = System.Drawing.Size;
using DrawingRectangle = System.Drawing.Rectangle;

namespace CrossX.WindowsForms
{
    internal partial class MainForm : Form, INativeWindow
    {
        private readonly ISkiaCanvas skiaCanvas;
        private readonly ICoreApplication app;
        private readonly ISystemDispatcher systemDispatcher;
        private readonly MainLoop mainLoop;
        private FormBorderStyle lastFormBorderStyle = FormBorderStyle.Sizable;
        private DrawingRectangle lastNormalPosition;
        private FormWindowState lastNotFullscreenWindowState;

        Size INativeWindow.MinSize
        {
            set => systemDispatcher.BeginInvoke(() =>
                {
                    var diff = Size - ClientSize;
                    MinimumSize = new DrawingSize(value.Width + diff.Width, value.Height + diff.Height);
                });
        }

        Size INativeWindow.MaxSize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     var diff = Size - ClientSize;
                     base.MaximumSize = new DrawingSize(value.Width + diff.Width, value.Height + diff.Height);
                 });
        }

        Size INativeWindow.Size
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     if(State == Framework.UI.Global.WindowState.Normal)
                     {
                         ClientSize = new DrawingSize(value.Width, value.Height);
                     }
                     lastNormalPosition = Bounds;
                 });
        }

        bool INativeWindow.CanResize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     FormBorderStyle = value ? FormBorderStyle.Sizable : FormBorderStyle.Fixed3D;
                     lastFormBorderStyle = FormBorderStyle;
                 });
        }

        bool INativeWindow.CanMaximize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     MaximizeBox = value;
                 });
        }

        public WindowState State
        {
            get
            {
                if(FormBorderStyle == FormBorderStyle.None)
                {
                    return Framework.UI.Global.WindowState.Fullscreen;
                }

                if (WindowState == FormWindowState.Maximized)
                {
                    return Framework.UI.Global.WindowState.Maximized;
                }
                return Framework.UI.Global.WindowState.Normal;
            }

            set
            {
                
                systemDispatcher.BeginInvoke(() =>
                {
                    switch (value)
                    {
                        case Framework.UI.Global.WindowState.Fullscreen:
                            EnterFullscreen();
                            break;

                        case Framework.UI.Global.WindowState.Maximized:
                            LeaveFullscreen();
                            WindowState = FormWindowState.Maximized;
                            break;

                        case Framework.UI.Global.WindowState:
                            LeaveFullscreen();
                            WindowState = FormWindowState.Normal;
                            break;
                    }
                });
            }
        }

        public MainForm(ICoreApplication app, IServicesProvider servicesProvider = null)
        {
            Hide();
            InitializeComponent();
            Hide();

            var scopeBuilder = new ScopeBuilder(servicesProvider);
            scopeBuilder.WithSkia();
            scopeBuilder.WithInstance(this).As<INativeWindow>();

            float dpi = 96;
            using (var gr = CreateGraphics())
            {
                dpi = gr.DpiX;
            }

            mainLoop = new MainLoop(app, skglControl.Invalidate, scopeBuilder, dpi, true);

            var services = scopeBuilder.Build();
            var factory = services.GetService<IObjectFactory>();
            systemDispatcher = services.GetService<ISystemDispatcher>();
            skiaCanvas = factory.Create<ISkiaCanvas>();

            skglControl.PaintSurface += SkglControl_PaintSurface;
            app.Initialize(services);
            this.app = app;

            mainLoop.Initialize();
            Show();
            BringToFront();
            skglControl.Invalidate();
            skglControl.KeyDown += SkglControl_KeyDown;
        }

        private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs args)
        {
            skiaCanvas.Prepare(args.Surface.Canvas, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
            mainLoop.OnPaintSurface(skiaCanvas.Canvas);
        }

        protected override void OnHandleDestroyed(EventArgs args)
        {
            base.OnHandleDestroyed(args);
            mainLoop.Finish();
            skiaCanvas.Canvas.Dispose();
        }

        private void SkglControl_KeyDown(object sender, KeyEventArgs args)
        {
            OnKeyDown(args);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if(args.KeyCode == Keys.F11)
            {
                switch(State)
                {
                    case Framework.UI.Global.WindowState.Normal:
                        State = Framework.UI.Global.WindowState.Maximized;
                        break;

                    case Framework.UI.Global.WindowState.Maximized:
                        State = Framework.UI.Global.WindowState.Fullscreen;
                        break;
                    case Framework.UI.Global.WindowState.Fullscreen:
                        State = Framework.UI.Global.WindowState.Normal;
                        break;
                }
            }
        }

        private void EnterFullscreen()
        {
            if (WindowState == FormWindowState.Normal && FormBorderStyle != FormBorderStyle.None)
            {
                lastNormalPosition = Bounds;
            }

            if(FormBorderStyle != FormBorderStyle.None)
            {
                lastNotFullscreenWindowState = WindowState;
            }

            lastFormBorderStyle = FormBorderStyle;
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.FromHandle(Handle).Bounds;
        }

        private void LeaveFullscreen()
        {
            FormBorderStyle = lastFormBorderStyle;
            Bounds = lastNormalPosition;
        }
    }
}
