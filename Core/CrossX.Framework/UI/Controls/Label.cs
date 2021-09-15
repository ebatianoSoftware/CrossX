using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{
    public class Label : View
    {
        private string text;
        private string fontFamily;
        private float fontSize = 10;
        private FontWeight fontWeight = FontWeight.Normal;
        private bool italic;
        private TextAlign textAlign = TextAlign.Center | TextAlign.Middle;
        private Color textColor = Color.Black;
        private readonly IFontManager fontManager;

        public string Text { get => text; set => SetProperty(ref text, value); }
        public string FontFamily { get => fontFamily; set => SetProperty(ref fontFamily, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        public FontWeight FontWeight { get => fontWeight; set => SetProperty(ref fontWeight, value); }
        public bool Italic { get => italic; set => SetProperty(ref italic, value); }

        public TextAlign TextAlign { get => textAlign; set => SetProperty(ref textAlign, value); }

        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        public Label(IFontManager fontManager)
        {
            this.fontManager = fontManager;
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);
            var font = fontManager.FindFont(FontFamily, FontSize, FontWeight, Italic);
            canvas.DrawText(Text, font, ScreenBounds, TextAlign, TextColor);
        }
    }
}
