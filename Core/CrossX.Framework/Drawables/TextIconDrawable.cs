using CrossX.Framework.Graphics;
using System.Numerics;

namespace CrossX.Framework.Drawables
{
    public class TextIconDrawable : Drawable
    {
        private readonly IFontManager fontManager;

        public string FontFamily { get; set; }
        public Length FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public string Text { get; set; }

        public Length OffsetX { get; set; }
        public Length OffsetY { get; set; }

        public TextIconDrawable(IFontManager fontManager)
        {
            this.fontManager = fontManager;
        }

        public override void Draw(Canvas canvas, RectangleF rectangle, Color color)
        {
            var font = fontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, false);
            rectangle = rectangle.Deflate(-100, -100);
            rectangle = rectangle.Offset(new Vector2(OffsetX.Calculate(), OffsetY.Calculate()));
            canvas.DrawText(Text, font, rectangle, TextAlign.Center | TextAlign.Middle, color, FontMeasure.Strict);
        }
    }
}
