using System;

namespace CrossX.Framework.UI.Controls
{
    public abstract class TextBasedControl: View
    {
        private string text;
        private string fontFamily;
        private float fontSize = 10;
        private FontWeight fontWeight = FontWeight.Normal;
        private bool fontItalic;
        private Alignment horizontalTextAlign = Alignment.Center;
        private Alignment verticalTextAlign = Alignment.Center;
        private Color foregroundColor = Color.Black;
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
        public string Text 
        { 
            get => text;
            set
            {
                if(SetProperty(ref text, value))
                {
                    Parent?.InvalidateLayout();
                    Services.RedrawService.RequestRedraw();
                }
            }
        }
        public string FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }
        public FontWeight FontWeight { get => fontWeight; set => SetProperty(ref fontWeight, value); }
        public bool FontItalic { get => fontItalic; set => SetProperty(ref fontItalic, value); }

        public Alignment HorizontalTextAlignment { get => horizontalTextAlign; set => SetProperty(ref horizontalTextAlign, value); }
        public Alignment VerticalTextAlignment { get => verticalTextAlign; set => SetProperty(ref verticalTextAlign, value); }

        public Color ForegroundColor { get => foregroundColor; set => SetProperty(ref foregroundColor, value); }

        public TextBasedControl(IUIServices services) : base(services)
        {
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;
            var autoHeight = Height.IsAuto && VerticalAlignment != Alignment.Stretch;

            var font = Services.FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var sizeAuto = font.MeasureText(Text, FontMeasure);


            sizeAuto.Width += TextPadding.Width;
            sizeAuto.Height += TextPadding.Height;

            return new SizeF(autoWidth ? sizeAuto.Width : Math.Max(size.Width, sizeAuto.Width), autoHeight ? sizeAuto.Height : Math.Max(size.Height, sizeAuto.Height));
        }
    }
}
