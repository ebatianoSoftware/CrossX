using CrossX.Framework.Graphics;

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
        protected readonly IFontManager FontManager;

        public string Text 
        { 
            get => text;
            set
            {
                if(SetProperty(ref text, value))
                {
                    Parent?.InvalidateLayout();
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

        public TextBasedControl(IFontManager fontManager)
        {
            FontManager = fontManager;
        }
    }
}
