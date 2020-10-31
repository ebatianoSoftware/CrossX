using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Async
{
    public static class AsyncExtensions
    {
        public static Coroutine AsCoroutine(this Task task)
        {
            return Coroutine.Agregate(GetCoroutine(task, 0));
        }

        public static Coroutine AsCoroutine(this IEnumerable<Coroutine> coroutines)
        {
            return Coroutine.Agregate(coroutines);
        }

        public static Coroutine AsCoroutine(this Task task, int checkIntervalInMs)
        {
            return Coroutine.Agregate(GetCoroutine(task, checkIntervalInMs));
        }

        private static IEnumerable<Coroutine> GetCoroutine(Task task, int ms)
        {
            while (!task.IsCompleted)
            {
                var seconds = (double)ms / 1000.0;
                yield return ms == 0 ? Coroutine.WaitForNextFrame() : Coroutine.WaitForSeconds(seconds);
            }

            task.Wait();
        }
    }
}
