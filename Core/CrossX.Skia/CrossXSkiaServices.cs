using CrossX.Abstractions.IoC;
using CrossX.Framework.Graphics;
using CrossX.Skia.Graphics;

namespace CrossX.Skia
{
    public static class CrossXSkiaServices
    {
        public static IScopeBuilder WithSkia<TSkiaFontManager>(this IScopeBuilder builder) where TSkiaFontManager: SkiaFontManager
        {
            return builder
                .WithType<SkiaImage>().As<Image>()
                .WithType<SkiaCanvas>().As<Canvas>().As<ISkiaCanvas>()
                .WithType<SkiaVertexBuffer>().As<VertexBuffer>()
                .WithType<SkiaFont>().As<Font>()
                .WithType<TSkiaFontManager>().As<IFontManager>().AsSingleton()
                .WithType<SkiaEffectsFactory>().As<IEffectsFactory>().AsSingleton();
        }
    }
}
