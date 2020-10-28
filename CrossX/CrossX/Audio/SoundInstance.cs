using System;

namespace CrossX.Audio
{
    public abstract class SoundInstance
    {
        public abstract bool IsLooped { get; set; }
        public abstract double Pitch { get; set; }
        public abstract double Volume { get; set; }

        public abstract SoundState State { get; }
        public abstract void Play();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Stop();

        public abstract event Action Finished;
    }
}
