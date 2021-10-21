using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class Font : Disposable
    {
        public string FamilyName { get; protected set; }
        public abstract SizeF MeasureText(string text, FontMeasure measure);
        public abstract int BreakText(string text, float position);
    }
}
