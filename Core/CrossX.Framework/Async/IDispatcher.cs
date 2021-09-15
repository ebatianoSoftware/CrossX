using System;
using System.Threading.Tasks;

namespace CrossX.Framework.Async
{
    public interface IDispatcher
    {
        void BeginInvoke(Action action);
        Task<T> InvokeAsync<T>(Func<T> func);
    }
}
