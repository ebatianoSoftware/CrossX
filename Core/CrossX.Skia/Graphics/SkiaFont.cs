using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;

namespace CrossX.Skia.Graphics
{
    internal class SkiaFont: Font
    {
        public SKFont SKFont { get; }
        public SKPaint SKPaint { get; }
        public SkiaFont(SKFont font)
        {
            SKFont = font;
            
            SKPaint = new SKPaint(font)
            {
                IsStroke = false,
                IsAntialias = true,
                SubpixelText = true,
                HintingLevel = SKPaintHinting.Full,
                //IsAutohinted = true,
                //LcdRenderText = true,
                FilterQuality = SKFilterQuality.High
            };

            FamilyName = font.Typeface.FamilyName;
        }

        public override SizeF MeasureText(string text, FontMeasure measure)
        {
            var rect = new SKRect();
            var width = SKPaint.MeasureText(text, ref rect);
            var height = SKFont.Size;

            switch(measure)
            {
                case FontMeasure.Strict:
                    SKPaint.MeasureText("X", ref rect);
                    height = rect.Height;
                    break;

                case FontMeasure.Extended:
                    height = SKFont.Size + SKFont.Metrics.Descent;
                    break;
            }
            
            return new SizeF(width, height);
        }
    }
}
