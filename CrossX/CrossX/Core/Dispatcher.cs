using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CrossX.Core
{
    public class Dispatcher : IDispatcher
    {
        ConcurrentQueue<Action> queuedActions = new ConcurrentQueue<Action>();

        public void BeginInvoke(Action action)
        {
            queuedActions.Enqueue(action);
        }

        public void Process()
        {
            while(queuedActions.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
