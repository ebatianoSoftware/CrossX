using CrossX.Forms.Values;
using CrossX.Graphics2D.Text;
using System;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public class TextButton : Button
    {
        public string Font { get => font; set => SetProperty(ref font, value); }
        public FontStyle FontStyle { get => fontStyle; set => SetProperty(ref fontStyle, value); }
        public TextAlignment TextAlignment { get => textAlignment; set => SetProperty(ref textAlignment, value); }
        public float FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

        public Color4 FocusedColor { get => focusedColor; set => SetProperty(ref focusedColor, value); }
        public Color4 NormalColor { get => normalColor; set => SetProperty(ref normalColor, value); }
        public Color4 DownColor { get => downColor; set => SetProperty(ref downColor, value); }

        public Color4 DisabledColor { get => disabledColor; set => SetProperty(ref disabledColor, value); }

        private string font = "";
        private float fontSize = 12;
        private TextAlignment textAlignment;
        private FontStyle fontStyle = FontStyle.Regular;
        private readonly IFontsContainer fontsContainer;

        private bool shouldUpdateText = true;

        private TextObject textObject;
        private Color4 focusedColor = Color4.Yellow;
        private Color4 normalColor = Color4.White;
        private Color4 downColor = Color4.LightGray;
        private Color4 disabledColor = Color4.Gray;

        public TextButton(IControlParent parent, IFontsContainer fontsContainer, IControlServices services) : base(parent, services)
        {
            this.fontsContainer = fontsContainer;
            VerticalAlignment = Alignment.Start;
            HorizontalAlignment = Alignment.Start;
            TextAlignment = TextAlignment.Center;
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

                case nameof(DataContext):
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

            TextObjectFactory.Instance.UpdateText(textObject, Text, fontSize, maxWidth, textAlignment, fontObj);
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

            var color = (IsEnabled && CommandEnabled) ? (IsDown ? downColor : (IsFocused ? focusedColor : normalColor)) : disabledColor;

            Services.SpriteBatch.TextureFilter = Graphics.TextureFilter.Anisotropic;
            
            var size = textObject.Size;
            var offsetY = (ActualHeight - size.Y) / 2;

            Services.SpriteBatch.DrawText(textObject, new Vector2(ActualX, ActualY + offsetY), color * tintColor);
        }

        public override void AddChild(Control control)
        {
            throw new InvalidOperationException("TextButton cannot have children");
        }
    }
}
