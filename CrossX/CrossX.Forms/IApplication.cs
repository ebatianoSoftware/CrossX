using System;

namespace CrossX.Forms
{
    public interface IApplication
    {
        event Action<TimeSpan> BeforeUpdate;
        event Action<TimeSpan> AfterUpdate;

        TimeSpan TotalTime { get; }
        TimeSpan DeltaTime { get; }

        void LoadStyles(string name);
    }
}
