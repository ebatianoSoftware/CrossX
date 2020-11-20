using CrossX.Graphics;
using CrossX.Graphics2D;
using System.Drawing;

namespace CrossX.Forms.UiHosts
{
    public class TransparentUiHost : IUiHost
    {
        private readonly IGraphicsDevice graphicsDevice;

        public TransparentUiHost(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            Transform2D = new Transform2D(graphicsDevice);
        }

        public Rectangle TargetRect => new Rectangle(0, 0, graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height);

        public ITransform2D Transform2D { get; }

        public void BeginDraw()
        {
            graphicsDevice.Clear(Color4.Black);
        }

        public void EndDraw()
        {
            Transform2D.Clear();
            graphicsDevice.Present();
        }

        public Vector2 ScreenToUiUnits(Vector2 screenPoint)
        {
            return screenPoint;
        }
    }
}
