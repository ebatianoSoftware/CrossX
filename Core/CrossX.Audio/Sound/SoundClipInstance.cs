using System;

namespace CrossX.Audio.Sound
{
    public abstract class SoundClipInstance: IDisposable
    {
        public abstract float Volume { get; set; }
        public abstract float Pan { get; set; }

        public bool Loop { get; set; }

        public void Dispose() => Dispose(true);

        public abstract void Play();
        public abstract void Stop();

        public abstract void Pause();
        public abstract void Resume();

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
