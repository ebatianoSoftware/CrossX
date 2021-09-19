using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Framework.Navigation
{
    internal class NavigationRequest : EventArgs
    {
        private List<Task> navigationTasks;
        public IEnumerable<Task> NavigationTasks => navigationTasks;

        public object ViewModel { get; }

        public NavigationRequest(object viewModel) => ViewModel = viewModel;

        public void AddNavigationTask(Task task)
        {
            if (navigationTasks == null) navigationTasks = new List<Task>();
            navigationTasks.Add(task);
        }
    }
}
