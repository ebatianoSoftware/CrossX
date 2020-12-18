using CrossX.Graphics;
using CrossX.Graphics2D;
using System;
using System.Drawing;
using System.Numerics;
using XxIoC;

namespace CrossX.Forms.UiHosts
{
    public class TargetUiHost : IUiHost
    {
        public class Parameters
        {
            public int? TargetWidth;
            public int? TargetHeight;
            public TextureFilter DesiredTextureFilter = TextureFilter.Linear;
            public TextureFilter ScalingFilter = TextureFilter.Linear;
        }
        public Rectangle TargetRect { get; private set; }
        public ITransform2D Transform2D { get; }

        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;

        private RenderTarget renderTarget;
        private SpriteBatch spriteBatch;

        public float ScaleToPixel { get; private set; }

        public TextureFilter DesiredTextureFilter => parameters.DesiredTextureFilter;

        private readonly Parameters parameters;
        private Size currentTargetSize;

        private Size targetSize;

        public Matrix4x4 WindowToCanvasTransform => Matrix4x4.CreateScale(
                    (float)currentTargetSize.Width / (float)graphicsDevice.CurrentTargetSize.Width,
                    (float)currentTargetSize.Height / (float)graphicsDevice.CurrentTargetSize.Height, 1);

        public TargetUiHost(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, Parameters parameters)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;

            Transform2D = new Transform2D(graphicsDevice);

            this.parameters = parameters;
            CalculateScale();
            spriteBatch = objectFactory.Create<SpriteBatch>();
        }

        private void CalculateScale()
        {
            Size unitSize = Size.Empty;

            if (parameters.TargetWidth.HasValue)
            {
                ScaleToPixel = (float)Math.Ceiling((double)graphicsDevice.CurrentTargetSize.Width / parameters.TargetWidth.Value);

                unitSize.Width = parameters.TargetWidth.Value * graphicsDevice.CurrentTargetSize.Height / graphicsDevice.CurrentTargetSize.Width;
                unitSize.Width = parameters.TargetWidth.Value;
            }
            else if (parameters.TargetHeight.HasValue)
            {
                ScaleToPixel = (float)Math.Ceiling((double)graphicsDevice.CurrentTargetSize.Height / parameters.TargetHeight.Value);

                unitSize.Width = parameters.TargetHeight.Value * graphicsDevice.CurrentTargetSize.Width / graphicsDevice.CurrentTargetSize.Height;
                unitSize.Height = parameters.TargetHeight.Value;
            }
            else
            {
                ScaleToPixel = 1;
                unitSize = graphicsDevice.CurrentTargetSize;
            }

            TargetRect = new Rectangle(0, 0, unitSize.Width, unitSize.Height);
            targetSize = new Size((int)Math.Ceiling(unitSize.Width * ScaleToPixel), (int)Math.Ceiling(unitSize.Height * ScaleToPixel));
        }

        public void Update()
        {
            CalculateScale();

            if (targetSize != currentTargetSize)
            {
                currentTargetSize = targetSize;
                PrepareRenderTarget();
            }
        }

        private void PrepareRenderTarget()
        {
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

            Transform2D.Push(Matrix4x4.CreateScale(ScaleToPixel, ScaleToPixel, 1));
        }

        public void EndDraw()
        {
            Transform2D.Clear();
            graphicsDevice.SetRenderTarget(null);

            if (renderTarget != null)
            {
                graphicsDevice.Clear(Color4.Black);
                spriteBatch.TextureFilter = parameters.ScalingFilter;
                spriteBatch.DrawImage(renderTarget, new RectangleF(0, 0, graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height), null, Color4.White);
                spriteBatch.Flush();
            }

            graphicsDevice.Present();
        }
    }
}
