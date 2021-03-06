using CrossX.Graphics;
using CrossX.Graphics2D;
using XxIoC;
using System;
using System.Drawing;
using System.Numerics;

namespace CrossX.Forms.UiHosts
{
    public class ScaledUiHost : IUiHost
    {
        public class Parameters
        {
            public Func<Vector2, float> CalculateScale;
            public TextureFilter DesiredTextureFilter = TextureFilter.Linear;
            public TextureFilter ScalingFilter = TextureFilter.Linear;
            public int AaSamples = 1;
        }

        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;

        public ITransform2D Transform2D { get; }
        
        private readonly Func<Vector2, float> calculateScale;

        public float ScaleToPixel { get; private set; } = 1;

        private Size currentTargetSize;
        private readonly int aaSamples;

        private RenderTarget renderTarget;
        private SpriteBatch spriteBatch;
        private TextureFilter filter;

        public TextureFilter DesiredTextureFilter { get; }

        public ScaledUiHost(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, Parameters parameters)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;

            DesiredTextureFilter = parameters.DesiredTextureFilter;
            Transform2D = new Transform2D(graphicsDevice);

            aaSamples = Math.Max(1, Math.Min(6, parameters.AaSamples));
            filter = parameters.ScalingFilter;

            calculateScale = parameters.CalculateScale;
            CalculateScale();

            spriteBatch = objectFactory.Create<SpriteBatch>();
        }

        private void CalculateScale()
        {
            var scale = calculateScale(new Vector2(graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height));
            ScaleToPixel = scale * aaSamples;
        }

        public Rectangle TargetRect
        {
            get
            {
                return new Rectangle(0, 0, (int)Math.Ceiling(currentTargetSize.Width / ScaleToPixel), (int)Math.Ceiling(currentTargetSize.Height / ScaleToPixel));
            }
        }

        public Matrix4x4 WindowToCanvasTransform => Matrix4x4.Identity;

        public void Update()
        {
            CalculateScale();

            var targetSize = new Size(graphicsDevice.Size.Width * aaSamples, graphicsDevice.Size.Height * aaSamples);

            if(targetSize != currentTargetSize)
            {
                currentTargetSize = targetSize;
                PrepareRenderTarget();
            }
        }

        private void PrepareRenderTarget()
        {
            if (aaSamples == 1) return;

            renderTarget?.Dispose();

            renderTarget = objectFactory.Create<RenderTarget>(new RenderTargetCreationOptions
            {
                Content = RenderTargetContent.Both,
                Width = currentTargetSize.Width,
                Height = currentTargetSize.Height
            });
        }

        public void BeginDraw()
        {
            if (renderTarget != null) graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color4.Black);

            Transform2D.Push(Matrix4x4.CreateScale(new Vector3(ScaleToPixel, ScaleToPixel, 1)));
        }

        public void EndDraw()
        {
            Transform2D.Clear();
            graphicsDevice.SetRenderTarget(null);

            if (renderTarget != null)
            {
                graphicsDevice.Clear(Color4.Black);
                spriteBatch.TextureFilter = filter;
                spriteBatch.DrawImage(renderTarget, new RectangleF(0,0,graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height), null, Color4.White);
                spriteBatch.Flush();
            }

            graphicsDevice.Present();
        }
    }
}
