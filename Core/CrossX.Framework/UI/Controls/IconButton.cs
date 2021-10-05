using System;

namespace CrossX.Framework.UI.Controls
{
    public class IconButton : Button
    {
        private Length iconSize = new Length(24);
        private string iconText;
        private string iconFontFace = "FluentSystemIcons-Filled";
        private Length spacing;
        private FontMeasure iconFontMeasure = FontMeasure.Strict;

        public Length IconSize { get => iconSize; set => SetPropertyAndRecalcLayout(ref iconSize, value); }
        public string IconText { get => iconText; set => SetPropertyAndRecalcLayout(ref iconText, value); }
        public string IconFontFace { get => iconFontFace; set => SetPropertyAndRecalcLayout(ref iconFontFace, value); }
        public FontMeasure IconFontMeasure { get => iconFontMeasure; set => SetPropertyAndRecalcLayout(ref iconFontMeasure, value); }
        public Length Spacing { get => spacing; set => SetPropertyAndRecalcLayout(ref spacing, value); }

        public IconButton(IUIServices services) : base(services)
        {
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;
            var autoHeight = Height.IsAuto && VerticalAlignment != Alignment.Stretch;

            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var sizeAuto = font.MeasureText(Text, FontMeasure);

            sizeAuto.Width += TextPadding.Width;
            sizeAuto.Height += TextPadding.Height;

            if (!string.IsNullOrEmpty(iconText) && IconSize != Length.Zero && !string.IsNullOrEmpty(IconFontFace))
            {
                var spacing = Spacing.Calculate();
                var iconFont = Services.FontManager.FindFont(IconFontFace, IconSize.Calculate(), FontWeight.Normal, false);
                var iconSize = iconFont.MeasureText(IconText, FontMeasure.Strict);

                switch (HorizontalTextAlignment)
                {
                    case Alignment.Start:
                    case Alignment.End:
                        sizeAuto.Width += spacing + iconSize.Width;
                        sizeAuto.Height = Math.Max(sizeAuto.Height, iconSize.Height + TextPadding.Height);
                        break;
                }

                switch (VerticalTextAlignment)
                {
                    case Alignment.Start:
                    case Alignment.End:
                        sizeAuto.Height += spacing + iconSize.Height;
                        sizeAuto.Width = Math.Max(sizeAuto.Width, iconSize.Width + TextPadding.Width);
                        break;
                }
            }

            return new SizeF(autoWidth ? sizeAuto.Width : Math.Max(size.Width, sizeAuto.Width), autoHeight ? sizeAuto.Height : Math.Max(size.Height, sizeAuto.Height));
        }

        protected override void RenderButton(Graphics.Canvas canvas, Color foregroundColor, Color backgroundColor, float opacity)
        {
            base.RenderButton(canvas, foregroundColor, backgroundColor, opacity);

            var font = Services.FontManager.FindFont(IconFontFace, IconSize.Calculate(), FontWeight.Normal, false);

            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(IconText, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment.Oposite(), VerticalTextAlignment.Oposite()), foregroundColor * opacity, IconFontMeasure);
        }
    }
}
