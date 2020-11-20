using CrossX.Forms.Values;
using CrossX.Graphics2D.Text;
using System;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public sealed class Label : Control
    {
        public string Font { get => font; set => SetProperty(ref font, value); }
        public FontStyle FontStyle { get => fontStyle; set => SetProperty(ref fontStyle, value); }
        public TextAlignment TextAlignment { get => textAlignment; set => SetProperty(ref textAlignment, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }
        public Color4 TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        private TextObject textObject;
        private string font = "";
        private float fontSize = 12;
        private TextAlignment textAlignment;
        private Color4 textColor = Color4.White;
        private FontStyle fontStyle = FontStyle.Regular;
        private readonly IFontsContainer fontsContainer;
        private readonly IUiHost uiHost;
        private TextSource text;

        private bool shouldUpdateText = true;
        private float scaleToPixel = 1;

        public Label(IControlParent parent, IFontsContainer fontsContainer, IControlServices services, IUiHost uiHost) : base(parent, services)
        {
            this.fontsContainer = fontsContainer;
            this.uiHost = uiHost;
            VerticalAlignment = Alignment.Start;
            HorizontalAlignment = Alignment.Start;
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case nameof(Text):
                case nameof(ActualWidth):
                case nameof(ActualHeight):
                case nameof(TextAlignment):
                case nameof(Font):
                case nameof(FontSize):
                case nameof(FontStyle):
                    shouldUpdateText = true;
                    break;
            }
        }

        private void UpdateText()
        {
            var fontObj = fontsContainer.Find(font, fontSize * scaleToPixel, fontStyle);
            if (textObject == null)
            {
                textObject = TextObjectFactory.Instance.CreateText(fontObj, new TextSource("@"), fontSize);
            }

            int maxWidth = (int)ActualWidth;
            if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
            {
                maxWidth = 0;
            }

            TextObjectFactory.Instance.UpdateText(textObject, text, fontSize, maxWidth, textAlignment, fontObj);
            Parent.InvalidateLayout();
        }

        public override Vector2 CalculateSize(RectangleF clientArea, bool includeMargins)
        {
            var size = base.CalculateSize(clientArea, includeMargins);

            if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
            {
                size.X = textObject?.Size.X ?? size.X;
            }

            if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
            {
                size.Y = textObject?.Size.Y ?? size.Y;
            }

            return size;
        }

        public override void BeforeUpdate()
        {
            if(uiHost.ScaleToPixel != scaleToPixel)
            {
                shouldUpdateText = true;
                scaleToPixel = uiHost.ScaleToPixel;
            }

            if (shouldUpdateText)
            {
                UpdateText();
                shouldUpdateText = false;
            }
            base.BeforeUpdate();
        }

        protected override void OnDraw(TimeSpan frameTime, Color4 tintColor)
        {
            base.OnDraw(frameTime, tintColor);

            if (textObject == null) UpdateText();

            Services.SpriteBatch.TextureFilter = Graphics.TextureFilter.Anisotropic;
            Services.SpriteBatch.DrawText(textObject, new Vector2(ActualX, ActualY), textColor * tintColor);
        }
    }
}
