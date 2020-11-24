using System;

namespace CrossX.Audio
{    
    internal class SoundSettings : ISoundSettings
    {
        private float soundVolume = 1;
        private float musicVolume = 1;

        public float SoundVolume 
        { 
            get => soundVolume; 
            set
            {
                soundVolume = value;
                ParametersChanged?.Invoke();
            }
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                ParametersChanged?.Invoke();
            }
        }

        public event Action ParametersChanged;
    }
}
