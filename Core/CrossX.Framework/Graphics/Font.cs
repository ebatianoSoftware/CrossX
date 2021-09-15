using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class Font : Disposable
    {
        public abstract SizeF MeasureText(string text, FontMeasure measure);
    }
}
