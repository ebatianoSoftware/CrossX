using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        private bool elipsis;
        public bool Elipsis { get => elipsis; set => SetPropertyAndRedraw(ref elipsis, value); }

        private string elipsisText;

        public Label(IUIServices service) : base(service)
        {
            HorizontalAlignment = Alignment.Start;
            VerticalAlignment = Alignment.Start;
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);
            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(elipsisText ?? Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), ForegroundColor * opacity, FontMeasure);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(Text):
                case nameof(ActualWidth):

                    RecalculateElipsis();

                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        private void RecalculateElipsis()
        {
            if (!Elipsis)
            {
                elipsisText = null;
                return;
            }

            var maxWidth = ActualWidth - TextPadding.Width;

            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);

            string text = Text;
            var width = font.MeasureText(text, FontMeasure.Strict).Width;

            int cut = 0;
            while(width > maxWidth)
            {
                cut++;
                text = Text.Substring(0, Text.Length / 2 - cut) + "..." + Text.Substring(Text.Length / 2 + cut);
                width = font.MeasureText(text, FontMeasure.Strict).Width;
            }

            elipsisText = text;
        }
    }
}
