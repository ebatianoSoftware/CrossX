using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;

namespace CrossX.Skia.Graphics
{
    internal class SkiaEffectsFactory : IEffectsFactory
    {
        public IEffect CreateBlurEffect(SizeF blurSize)
        {
            return new SkiaEffect(SKImageFilter.CreateBlur(blurSize.Width, blurSize.Height, SKShaderTileMode.Decal));
        }
    }

    internal class SkiaEffect: IEffect
    {
        public SKImageFilter Filter { get; }

        public SkiaEffect(SKImageFilter imageFilter)
        {
            Filter = imageFilter;
        }

        public void Dispose()
        {
            Filter.Dispose();
        }
    }
}
