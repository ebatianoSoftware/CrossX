using System;

namespace CrossX.Async
{
    public interface IDispatcher
    {
        void BeginInvoke(Action action);
    }
}
