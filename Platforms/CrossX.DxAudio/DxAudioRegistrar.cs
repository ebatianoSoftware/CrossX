using CrossX.Abstractions.IoC;
using CrossX.Audio.Sound;
using CrossX.DxAudio.Sound;

namespace CrossX.DxAudio
{
    public static class DxAudioRegistrar
    {
        public static IScopeBuilder WithXAudio(this IScopeBuilder builder)
        {
            return builder
                .WithType<XAudioEngine>().AsSelf().AsSingleton()
                .WithType<DxSoundClip>().As<SoundClip>()
                .WithType<DxSoundClipInstance>().As<SoundClipInstance>();
        }
    }
}
