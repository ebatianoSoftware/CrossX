using System;

namespace CrossX.Audio
{
    public interface ISoundSettings
    {
        double Volume { get; set; }
        event Action ParametersChanged;
    }
}
