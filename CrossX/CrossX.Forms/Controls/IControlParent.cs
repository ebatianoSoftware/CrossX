using CrossX.Graphics2D;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public interface IControlParent
    {
        RectangleF ClientArea { get; }
        SpriteBatch SpriteBatch { get; }
        PrimitiveBatch PrimitiveBatch { get; }
    }
}
