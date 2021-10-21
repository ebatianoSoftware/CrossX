using System;

namespace CrossX.Abstractions.Windows
{
    public interface IModalContext<TResult>
    {
        event Action<TResult> CloseWithResult;
    }
}
