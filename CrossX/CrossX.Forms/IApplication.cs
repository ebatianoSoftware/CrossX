using System;

namespace CrossX.Forms
{
    public interface IApplication
    {
        event Action<TimeSpan> BeforeUpdate;
        event Action<TimeSpan> AfterUpdate;

        void LoadStyles(string name);
    }
}
