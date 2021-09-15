using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Async
{
    internal class Dispatcher : IDispatcher
    {
        ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private ConcurrentQueue<AutoResetEvent> events = new ConcurrentQueue<AutoResetEvent>();

        public virtual void BeginInvoke(Action action)
        {
            queue.Enqueue(action);
        }

        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            T result = default;

            if(!events.TryDequeue(out var evnt))
            {
                evnt = new AutoResetEvent(false);
            }

            BeginInvoke(() =>
            {
                result = func();
                evnt.Set();
            });

            return Task.Run(() =>
            {
                evnt.WaitOne();
                events.Enqueue(evnt);
                return result;
            });
        }

        public void Process()
        {
            while(queue.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }
    }
}
