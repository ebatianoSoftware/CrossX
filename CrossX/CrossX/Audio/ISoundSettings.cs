using System;

namespace CrossX.Audio
{
    public interface ISoundSettings
    {
        float SoundVolume { get; set; }
        float MusicVolume { get; set; }
        event Action ParametersChanged;
    }
}
