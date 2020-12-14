using XxIoC;
using System;

namespace CrossX.Forms
{
    public interface IFormsAppContext: IDisposable
    {
        void Load();
        void Run();
    }
}
