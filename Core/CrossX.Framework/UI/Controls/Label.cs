using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        private FontMeasure fontMeasure = FontMeasure.Extended;
        public FontMeasure FontMeasure { get => fontMeasure; set => SetProperty(ref fontMeasure, value); }

        public Label(IFontManager fontManager): base(fontManager)
        {
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);
            var font = FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            canvas.DrawText(Text, font, ScreenBounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), TextColor, FontMeasure);
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;
            var autoHeight = Height.IsAuto && VerticalAlignment != Alignment.Stretch;

            var font = FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var sizeAuto = font.MeasureText(Text, FontMeasure);

            return new SizeF(autoWidth ? sizeAuto.Width : Math.Max(size.Width, sizeAuto.Width), autoHeight ? sizeAuto.Height : Math.Max(size.Height, sizeAuto.Height));
        }
    }
}
