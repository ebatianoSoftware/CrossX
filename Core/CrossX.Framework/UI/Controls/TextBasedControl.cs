using System;

namespace CrossX.Framework.UI.Controls
{
    public abstract class TextBasedControl: View
    {
        private string text;
        private string fontFamily;
        private Length fontSize = new Length(10);
        private FontWeight fontWeight = FontWeight.Normal;
        private bool fontItalic;
        private Alignment horizontalTextAlign = Alignment.Center;
        private Alignment verticalTextAlign = Alignment.Center;
        private Color foregroundColor = Color.Black;
        private FontMeasure fontMeasure = FontMeasure.Extended;
        private Thickness textPadding;
        private Color foregroundColorDisabled;

        public FontMeasure FontMeasure 
        {
            get => fontMeasure; 
            set => SetPropertyAndRecalcLayout(ref fontMeasure, value);
        }

        public Thickness TextPadding
        {
            get => textPadding;
            set => SetPropertyAndRecalcLayout(ref textPadding, value);
        }
        public virtual string Text 
        { 
            get => text;
            set => SetPropertyAndRecalcLayout(ref text, value);
        }

        public string FontFamily { get => fontFamily; set => SetPropertyAndRecalcLayout(ref fontFamily, value); }
        public Length FontSize { get => fontSize; set => SetPropertyAndRecalcLayout(ref fontSize, value); }
        public FontWeight FontWeight { get => fontWeight; set => SetPropertyAndRecalcLayout(ref fontWeight, value); }
        public bool FontItalic { get => fontItalic; set => SetPropertyAndRecalcLayout(ref fontItalic, value); }

        public Alignment HorizontalTextAlignment { get => horizontalTextAlign; set => SetPropertyAndRedraw(ref horizontalTextAlign, value); }
        public Alignment VerticalTextAlignment { get => verticalTextAlign; set => SetPropertyAndRedraw(ref verticalTextAlign, value); }

        public Color ForegroundColor { get => foregroundColor; set => SetPropertyAndRedraw(ref foregroundColor, value); }
        public Color ForegroundColorDisabled { get => foregroundColorDisabled; set => SetPropertyAndRedraw(ref foregroundColorDisabled, value); }

        public TextBasedControl(IUIServices services) : base(services)
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

            return new SizeF(autoWidth ? sizeAuto.Width : size.Width, autoHeight ? sizeAuto.Height : size.Height);
        }
    }
}
