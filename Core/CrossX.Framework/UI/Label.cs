using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework.UI
{
    public class Label : View
    {
        private string text;
        private string fontFamily;
        private float fontSize = 10;
        private FontWeight fontWeight = FontWeight.Normal;
        private bool italic;
        private Alignment horizontalTextAlign = Alignment.Center;
        private Alignment verticalTextAlign = Alignment.Center;
        private Color textColor = Color.Black;
        private FontMeasure fontMeasure = FontMeasure.Extended;
        private readonly IFontManager fontManager;

        public string Text { get => text; set => SetProperty(ref text, value); }
        public string FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        public FontWeight FontWeight { get => fontWeight; set => SetProperty(ref fontWeight, value); }
        public bool Italic { get => italic; set => SetProperty(ref italic, value); }

        public Alignment HorizontalTextAlignment { get => horizontalTextAlign; set => SetProperty(ref horizontalTextAlign, value); }
        public Alignment VerticalTextAlignment { get => verticalTextAlign; set => SetProperty(ref verticalTextAlign, value); }

        public FontMeasure FontMeasure { get => fontMeasure; set => SetProperty(ref fontMeasure, value); }

        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        public Label(IFontManager fontManager)
        {
            this.fontManager = fontManager;
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);
            var font = fontManager.FindFont(FontFamily, FontSize, FontWeight, Italic);
            canvas.DrawText(Text, font, ScreenBounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), TextColor, FontMeasure);
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;
            var autoHeight = Height.IsAuto && VerticalAlignment != Alignment.Stretch;

            var font = fontManager.FindFont(FontFamily, FontSize, FontWeight, Italic);
            var sizeAuto = font.MeasureText(Text, FontMeasure);

            return new SizeF(autoWidth ? sizeAuto.Width : Math.Max(size.Width, sizeAuto.Width), autoHeight ? sizeAuto.Height : Math.Max(size.Height, sizeAuto.Height));
        }
    }
}
