using S2IoC;
using System;

namespace CrossX.Forms
{
    public interface IFormsAppContext: IDisposable
    {
        void Load();
        void Run();
    }
}
