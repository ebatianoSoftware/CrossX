using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        public Label(IFontManager fontManager) : base(fontManager)
        {
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);
            var font = FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), ForegroundColor, FontMeasure);
        }
    }
}
