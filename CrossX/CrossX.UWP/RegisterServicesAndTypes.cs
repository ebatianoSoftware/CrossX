using CrossX.DxCommon.Graphics;
using CrossX.DxCommon.Graphics.Shaders;
using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using CrossX.IoC;
using CrossX.UWP.Graphics;

namespace CrossX.UWP
{
    internal static class RegisterServicesAndTypes
    {
        public static ScopeBuilder RegisterUwpTypes(this ScopeBuilder builder)
        {
            return builder
                    .WithType<DxTexture>().As<Texture2D>()
                    .WithType<DxVertexBuffer>().As<VertexBuffer>()
                    .WithType<DxBasicShader>().As<BasicShader>()
                    .WithType<DxRenderTarget>().As<RenderTarget>();
        }
    }
}
