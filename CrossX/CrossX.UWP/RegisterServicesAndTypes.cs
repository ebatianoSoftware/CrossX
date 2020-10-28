using CrossX.Graphics;
using CrossX.IoC;
using CrossX.UWP.Graphics;

namespace CrossX.UWP
{
    internal static class RegisterServicesAndTypes
    {
        public static ScopeBuilder RegisterUwpTypes(this ScopeBuilder builder)
        {
            return builder
                    .WithType<DxTexture2D>().As<Texture2D>();
        }
    }
}
