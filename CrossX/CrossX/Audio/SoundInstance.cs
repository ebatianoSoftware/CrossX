using System;

namespace CrossX.Audio
{
    public abstract class SoundInstance: IDisposable
    {
        public abstract bool IsLooped { get; set; }
        public abstract double Pitch { get; set; }
        public abstract double Volume { get; set; }

        public virtual SoundState State { get; protected set; }
        public abstract void Play();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Stop();

        public abstract event Action Finished;

        public abstract void Dispose();
    }
}
