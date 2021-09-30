using CrossX.Framework.Graphics;
using Xx;

namespace CrossX.Framework.Drawables
{
    [XxSchemaExport]
    public abstract class Drawable
    {
        public abstract void Draw(Canvas canvas, RectangleF rectangle, Color color);
    }
}
