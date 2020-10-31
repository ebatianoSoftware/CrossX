using CrossX.Audio;
using CrossX.DxCommon.Audio;
using CrossX.DxCommon.Graphics;
using CrossX.DxCommon.Graphics.Shaders;
using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using CrossX.IoC;

namespace CrossX.UWP
{
    internal static class RegisterServicesAndTypes
    {
        public static ScopeBuilder RegisterUwpServices(this ScopeBuilder builder)
        {
            return builder
                .WithType<SoundSettings>().As<ISoundSettings>().AsSingleton()
                .WithType<DxAudioManager>().AsSelf().AsSingleton();
        }

        public static ScopeBuilder RegisterUwpTypes(this ScopeBuilder builder)
        {
            return builder
                    .WithType<DxTexture>().As<Texture2D>()
                    .WithType<DxVertexBuffer>().As<VertexBuffer>()
                    .WithType<DxBasicShader>().As<BasicShader>()
                    .WithType<DxRenderTarget>().As<RenderTarget>()
                    .WithType<DxSound>().As<Sound>()
                    .WithType<DxAudioEmitter>().As<AudioEmitter>()
                    .WithType<DxAudioListener>().As<AudioListener>();
        }
    }
}
