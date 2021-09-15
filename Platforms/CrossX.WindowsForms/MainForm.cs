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

        private bool appRunned = false;

        public MainForm(ICoreApplication app, IServicesProvider servicesProvider = null)
        {
            InitializeComponent();

            var scopeBuilder = new ScopeBuilder();

            scopeBuilder
                .WithParent(servicesProvider)
                .WithSkia();

            var services = scopeBuilder.Build();
            var factory = services.GetService<IObjectFactory>();
            skiaCanvas = factory.Create<ISkiaCanvas>();

            skglControl.PaintSurface += SkglControl_PaintSurface;

            app.Initialize(services);
            this.app = app;
        }

        private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs args)
        {
            skiaCanvas.Prepare(args.Surface.Canvas, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
            
            if(!appRunned)
            {
                appRunned = true;
                app.Run(skiaCanvas.Canvas);
            }

            app.Render();
        }

        protected override void OnHandleDestroyed(EventArgs args)
        {
            base.OnHandleDestroyed(args);
            skiaCanvas.Canvas.Dispose();
        }
    }
}
