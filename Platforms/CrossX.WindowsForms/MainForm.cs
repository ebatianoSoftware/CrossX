using CrossX.Abstractions.IoC;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Core;
using CrossX.Framework.IoC;
using CrossX.Skia;
using SkiaSharp.Views.Desktop;
using System;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    internal partial class MainForm : Form, INativeWindow
    {
        private readonly ISkiaCanvas skiaCanvas;
        private readonly ICoreApplication app;
        private readonly ISystemDispatcher systemDispatcher;
        private readonly MainLoop mainLoop;

        public Size MinSize 
        { 
            set
            {
               systemDispatcher.BeginInvoke(() =>
               {
                   var diff = Size - ClientSize;
                   MinimumSize = new System.Drawing.Size(value.Width + diff.Width, value.Height + diff.Height);
               });
            }
        }

        public Size MaxSize 
        { 
            set
            {
                systemDispatcher.BeginInvoke(() =>
                {
                    var diff = Size - ClientSize;
                    base.MaximumSize = new System.Drawing.Size(value.Width + diff.Width, value.Height + diff.Height);
                });
            }
        }

        Size INativeWindow.Size
        {
            set
            {
                systemDispatcher.BeginInvoke(() =>
                {
                    ClientSize = new System.Drawing.Size(value.Width, value.Height);
                });
            }
        }

        bool INativeWindow.CanResize
        {
            set
            {
                systemDispatcher.BeginInvoke(() =>
                {
                    FormBorderStyle = value ? FormBorderStyle.Sizable : FormBorderStyle.Fixed3D;
                    
                });
            }
        }

        bool INativeWindow.CanMaximize
        {
            set
            {
                systemDispatcher.BeginInvoke(() =>
                {
                    MaximizeBox = value;
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
    }
}
