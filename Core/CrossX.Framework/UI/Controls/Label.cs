using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        private FontMeasure fontMeasure = FontMeasure.Extended;
        private Thickness textPadding;

        public FontMeasure FontMeasure { get => fontMeasure; set => SetProperty(ref fontMeasure, value); }

        public Thickness TextPadding
        {
            get => textPadding;
            set
            {
                if (SetProperty(ref textPadding, value))
                {
                    Parent?.InvalidateLayout();
                }
            }
        }

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

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;
            var autoHeight = Height.IsAuto && VerticalAlignment != Alignment.Stretch;

            var font = FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var sizeAuto = font.MeasureText(Text, FontMeasure);


            sizeAuto.Width += TextPadding.Width;
            sizeAuto.Height += TextPadding.Height;

            return new SizeF(autoWidth ? sizeAuto.Width : Math.Max(size.Width, sizeAuto.Width), autoHeight ? sizeAuto.Height : Math.Max(size.Height, sizeAuto.Height));
        }
    }
}
