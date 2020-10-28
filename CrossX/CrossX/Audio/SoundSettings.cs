using System;

namespace CrossX.Audio
{
    internal class SoundSettings : ISoundSettings
    {
        private double volume = 1;

        public double Volume 
        { 
            get => volume; 
            set
            {
                volume = value;
                ParametersChanged?.Invoke();
            }
        }
        public event Action ParametersChanged;
    }
}
