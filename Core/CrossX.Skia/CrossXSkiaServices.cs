﻿using CrossX.Abstractions.IoC;
using CrossX.Framework.Graphics;
using CrossX.Skia.Graphics;

namespace CrossX.Skia
{
    public static class CrossXSkiaServices
    {
        public static IScopeBuilder WithSkia(this IScopeBuilder builder)
        {
            return builder
                .WithType<SkiaImage>().As<Image>()
                .WithType<SkiaCanvas>().As<Canvas>().As<ISkiaCanvas>()
                .WithType<SkiaVertexBuffer>().As<VertexBuffer>()
                .WithType<SkiaFont>().As<Font>()
                .WithType<SkiaFontManager>().As<IFontManager>().AsSingleton()
                .WithType<SkiaEffectsFactory>().As<IEffectsFactory>().AsSingleton();
        }
    }
}
