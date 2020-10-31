using System.Collections.Generic;

namespace CrossX.Async
{
    public interface ICoroutineService
    {
        void Run(Coroutine coroutine);
        void Run(IEnumerable<Coroutine> coroutine);
    }
}
