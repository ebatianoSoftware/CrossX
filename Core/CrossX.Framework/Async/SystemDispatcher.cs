using System;

namespace CrossX.Framework.Async
{
    internal class SystemDispatcher: Dispatcher, ISystemDispatcher
    {
        private readonly Action onNewTask;

        public SystemDispatcher(Action onNewTask)
        {
            this.onNewTask = onNewTask;
        }

        public override void BeginInvoke(Action action)
        {
            base.BeginInvoke(action);
            onNewTask?.Invoke();
        }
    }
}
