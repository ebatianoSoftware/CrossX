using CrossX.Framework.Graphics;
using CrossX.Framework.Styles;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        public Label(IUIServices service) : base(service)
        {
            HorizontalAlignment = Alignment.Start;
            VerticalAlignment = Alignment.Start;

            ApplyDefaultStyle();
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity );
            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), ForegroundColor * opacity, FontMeasure);
        }

        protected override void ApplyDefaultStyle()
        {
            
            if (Services.AppValues.GetValue(ThemeValueKey.SystemForegroundColor) is Color fgColor) ForegroundColor = fgColor;

            if (Services.AppValues.GetValue(ThemeValueKey.SystemTextFontFamily) is string fontFamily) FontFamily = fontFamily;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemTextFontSize) is Length fontSize) FontSize = fontSize;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemTextFontWeight) is FontWeight fontWeight) FontWeight = fontWeight;
        }
    }
}
