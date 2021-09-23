using CrossX.Abstractions.Async;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Async
{
    internal class Dispatcher : IDispatcher
    {
        public Thread DispatcherThread { get; set; }

        ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private ConcurrentQueue<AutoResetEvent> events = new ConcurrentQueue<AutoResetEvent>();

        private AutoResetEvent waitEvent = new AutoResetEvent(false);

        public virtual void BeginInvoke(Action action)
        {
            if(!InvokeOnDispatcherThread(action))
            {
                EnqueueAction(action);
            }
        }

        protected void EnqueueAction(Action action)
        {
            queue.Enqueue(action);
            waitEvent.Set();
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

        protected bool InvokeOnDispatcherThread(Action action)
        {
            if(DispatcherThread != null)
            {
                if(DispatcherThread == Thread.CurrentThread)
                {
                    action();
                    return true;
                }
            }
            return false;
        }

        public void Wait(int time) => waitEvent.WaitOne(time);

        public void Touch() => waitEvent.Set();
    }
}
