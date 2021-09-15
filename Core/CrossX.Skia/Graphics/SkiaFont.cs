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
                IsAntialias = true
            };
        }
    }
}
