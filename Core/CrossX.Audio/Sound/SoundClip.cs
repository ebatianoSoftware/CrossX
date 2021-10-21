using System;

namespace CrossX.Audio.Sound
{
    public abstract class SoundClip: IDisposable
    {
        public abstract SoundClipInstance CreateInstance();

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
