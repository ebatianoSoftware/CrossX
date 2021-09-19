using CrossX.Abstractions.IoC;
using CrossX.Framework.Core;
using CrossX.Framework.IoC;
using CrossX.Skia;
using SkiaSharp.Views.Desktop;
using System;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    internal partial class MainForm : Form
    {
        private readonly ISkiaCanvas skiaCanvas;
        private readonly ICoreApplication app;

        private readonly MainLoop mainLoop;

        public MainForm(ICoreApplication app, IServicesProvider servicesProvider = null)
        {
            InitializeComponent();

            var scopeBuilder = new ScopeBuilder(servicesProvider);
            scopeBuilder.WithSkia();

            mainLoop = new MainLoop(app, skglControl.Invalidate, scopeBuilder, true);

            var services = scopeBuilder.Build();
            var factory = services.GetService<IObjectFactory>();
            skiaCanvas = factory.Create<ISkiaCanvas>();

            skglControl.PaintSurface += SkglControl_PaintSurface;
            app.Initialize(services);
            this.app = app;

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
