using System.Collections.Generic;

namespace CrossX.Audio
{
    internal class SoundInstancesPool
    {
        private readonly Queue<SoundInstance> soundInstances = new Queue<SoundInstance>();
        private readonly Sound sound;

        public SoundInstancesPool(Sound sound)
        {
            this.sound = sound;
        }

        public SoundInstance Get()
        {
            SoundInstance instance = null;
            if(soundInstances.Count == 0)
            {
                instance = sound.NewInstance();
                instance.Finished += () => OnFinished(instance);
            }
            else
            {
                instance = soundInstances.Dequeue();
            }
            
            return instance;
        }

        private void OnFinished(SoundInstance instance)
        {
            instance.Stop();
            soundInstances.Enqueue(instance);
        }
    }
}