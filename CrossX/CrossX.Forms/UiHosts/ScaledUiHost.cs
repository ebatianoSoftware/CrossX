using CrossX.Graphics;
using CrossX.Graphics2D;
using System;
using System.Drawing;

namespace CrossX.Forms.UiHosts
{
    public class ScaledUiHost: IUiHost
    {
        public class Parameters
        {
            public Func<Vector2, float> CalculateScale;
        }

        private readonly IGraphicsDevice graphicsDevice;
        public ITransform2D Transform2D { get; }
        
        private readonly Func<Vector2, float> calculateScale;

        private float Scale { get; set; } = 1;

        public ScaledUiHost(IGraphicsDevice graphicsDevice, Parameters parameters)
        {
            this.graphicsDevice = graphicsDevice;
            Transform2D = new Transform2D(graphicsDevice);

            calculateScale = parameters.CalculateScale;
            CalculateScale();
        }

        private void CalculateScale()
        {
            var scale = calculateScale(new Vector2(graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height));
            Scale = scale;
        }

        public Rectangle TargetRect
        {
            get
            {
                return new Rectangle(0, 0, (int)Math.Ceiling(graphicsDevice.Size.Width / Scale), (int)Math.Ceiling(graphicsDevice.Size.Height / Scale));
            }
        }

        public void Update()
        {
            CalculateScale();
        }

        public void BeginDraw()
        {
            graphicsDevice.Clear(Color4.Black);
            Transform2D.Push(Matrix.CreateScale(new Vector3(Scale, Scale, 1)));
        }

        public void EndDraw()
        {
            Transform2D.Clear();
            graphicsDevice.Present();
        }

        public Vector2 ScreenToUiUnits(Vector2 screenPoint)
        {
            var size = graphicsDevice.Size;

            var scaleX = (float)TargetRect.Width / (float)size.Width;
            var scaleY = (float)TargetRect.Height / (float)size.Height;

            var point = screenPoint;

            point.X *= scaleX;
            point.Y *= scaleY;

            return point;
        }
    }
}
