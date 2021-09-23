using System;

namespace CrossX.Framework.Async
{
    internal class SystemDispatcher: Dispatcher, ISystemDispatcher
    {
        private readonly Action onNewTask;

        public SystemDispatcher(Action onNewTask = null)
        {
            this.onNewTask = onNewTask;
        }

        public override void BeginInvoke(Action action)
        {
            if(!InvokeOnDispatcherThread(action))
            {
                EnqueueAction(action);
                onNewTask?.Invoke();
            }
        }
    }
}
