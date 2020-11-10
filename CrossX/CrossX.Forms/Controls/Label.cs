using CrossX.Forms.Values;
using CrossX.Graphics2D.Text;
using System;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public sealed class Label : Control
    {
        private TextSource text;
        private bool shouldUpdateText = true;

        public string Font { get => font; set => SetProperty(ref font, value); }
        public FontStyle FontStyle { get => fontStyle; set => SetProperty(ref fontStyle, value); }
        public TextAlignment TextAlignment { get => textAlignment; set => SetProperty(ref textAlignment, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }
        public Color4 TextColor { get => textColor; set => SetProperty(ref textColor, value); }
        public Color4 Background { get => background; set => SetProperty(ref background, value); }

        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        private TextObject textObject;
        private string font = "";
        private float fontSize = 12;
        private TextAlignment textAlignment;
        private Color4 textColor = Color4.White;
        private Color4 background;
        private FontStyle fontStyle = FontStyle.Regular;
        private readonly IFontsContainer fontsContainer;

        public Label(IControlParent parent, IFontsContainer fontsContainer) : base(parent)
        {
            this.fontsContainer = fontsContainer;
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
            var fontObj = fontsContainer.Find(font, fontSize, fontStyle);
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

            if (includeMargins)
            {
                clientArea = ClientAreaWithMargin(clientArea);
            }

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
            if (shouldUpdateText)
            {
                UpdateText();
                shouldUpdateText = false;
            }
            base.BeforeUpdate();
        }

        public override void Draw(TimeSpan frameTime)
        {
            if (background.A > 0)
            {
                Parent.PrimitiveBatch.DrawRect(new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight), background);
            }

            Parent.SpriteBatch.TextureFilter = Graphics.TextureFilter.Anisotropic;
            Parent.SpriteBatch.DrawText(textObject, new Vector2(ActualX, ActualY), textColor);
        }
    }
}
