using CrossX.Framework.Graphics;
using SkiaSharp;

namespace CrossX.Skia
{
    public interface ISkiaCanvas
    {
        Canvas Canvas { get; }
        void Prepare(SKCanvas canvas, int width, int height);
    }
}
