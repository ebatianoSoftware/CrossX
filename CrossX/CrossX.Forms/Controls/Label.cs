using CrossX.Graphics2D.Text;
using System;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public sealed class Label : Control
    {
        private TextSource text;
        private bool shouldUpdateText = true;

        public Font Font { get => font; set => SetProperty(ref font, value); }
        public TextAlignment TextAlignment { get => textAlignment; set => SetProperty(ref textAlignment, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        public Color4 TextColor { get => textColor; set => SetProperty(ref textColor, value); }
        public Color4 Background { get => background; set => SetProperty(ref background, value); }

        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        private TextObject textObject;
        private Font font;
        private float fontSize;
        private TextAlignment textAlignment;
        private Color4 textColor;
        private Color4 background;

        public Label(IControlParent parent) : base(parent)
        {
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
                    shouldUpdateText = true;
                    break;
            }
        }

        private void UpdateText()
        {
            if (textObject == null)
            {
                textObject = TextObjectFactory.Instance.CreateText(font, new TextSource(""), fontSize);
            }

            int maxWidth = (int)ActualWidth;
            if(Width.IsAuto)
            {
                maxWidth = 0;
            }

            TextObjectFactory.Instance.UpdateText(textObject, text, fontSize, maxWidth, textAlignment, font);
        }

        public override Vector2 CalculateSize(RectangleF clientArea)
        {
            var size = base.CalculateSize(clientArea);

            if(Width.IsAuto)
            {
                size.X = textObject.Size.X;
            }

            if (Height.IsAuto)
            {
                size.Y = textObject.Size.Y;
            }

            return size;

        }

        public override void BeforeUpdate()
        {
            if (shouldUpdateText)
            {
                UpdateText();
                shouldUpdateText = false;
                ShouldCalculateLayout = true;
            }
            base.BeforeUpdate();
        }

        public override void Draw(TimeSpan frameTime)
        {
            if (background.A > 0)
            {
                Parent.PrimitiveBatch.DrawRect(new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight), background);
            }

            Parent.SpriteBatch.DrawText(textObject, new Vector2(ActualX, ActualY), textColor);
        }
    }
}
