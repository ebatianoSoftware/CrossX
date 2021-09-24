﻿using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{
    public class Label : TextBasedControl
    {
        public Label(IUIServices service) : base(service)
        {
            HorizontalAlignment = Alignment.Start;
            VerticalAlignment = Alignment.Start;
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);
            var font = Services.FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), ForegroundColor, FontMeasure);
        }
    }
}
