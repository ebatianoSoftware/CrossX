using System;

namespace CrossX.Core
{
    public interface IDispatcher
    {
        void BeginInvoke(Action action);
    }
}
