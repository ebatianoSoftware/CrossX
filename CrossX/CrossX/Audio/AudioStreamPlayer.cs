using System;

namespace CrossX.Audio
{
    public abstract class AudioStreamPlayer
    {
        public abstract event Action Finished;
        public abstract float Volume { get; set; }
        public abstract void Play(bool loop);
        public abstract void Pause();
        public abstract void Reset();
    }
}
