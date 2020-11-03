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
                .WithType<DxAudioManager>().AsSelf().AsSingleton()
                .WithType<ShadersRepository>().As<IShadersRepository>().AsSingleton();
        }

        public static ScopeBuilder RegisterUwpTypes(this ScopeBuilder builder)
        {
            
            return builder
                    .WithType<DxTexture>().As<Texture2D>()
                    .WithType<DxVertexBuffer>().As<VertexBuffer>()
                    .WithType<DxIndexBuffer2>().As<IndexBuffer2>()
                    .WithType<DxIndexBuffer4>().As<IndexBuffer4>()
                    .WithType(typeof(DxPixelShader)).As(typeof(PixelShader))
                    .WithType(typeof(DxVertexShader)).As(typeof(VertexShader))
                    .WithType<DxRenderTarget>().As<RenderTarget>()
                    .WithType<DxSound>().As<Sound>()
                    .WithType<DxAudioEmitter>().As<AudioEmitter>()
                    .WithType<DxAudioListener>().As<AudioListener>()
                    .WithType<DxAudioStreamPlayer>().As<AudioStreamPlayer>();
        }
    }
}
