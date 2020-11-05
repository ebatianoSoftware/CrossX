using System;

namespace CrossX.Audio
{
    public abstract class Sound: IDisposable
    {
        public abstract TimeSpan Duration { get; }
        public abstract void Play();
        public abstract SoundInstance NewInstance();
        public abstract SoundInstance NewInstance(AudioEmitter audioEmitter, AudioListener audioListener);
        public abstract void Dispose();
    }
}
