using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Navigation;
using CrossX.Framework.IoC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Framework.Navigation
{
    internal class NavigationImpl : INavigationController
    {
        private ConcurrentStack<object> viewModels = new ConcurrentStack<object>();

        private readonly IObjectFactory objectFactory;
        private readonly IDispatcher dispatcher;

        public IServicesProvider Services { get; }

        public event EventHandler<NavigationRequest> NavigationRequested;
        public event Action<object> NavigatedTo;

        public NavigationImpl(IServicesProvider servicesProvider, IDispatcher dispatcher)
        {
            Services = new ScopeBuilder(servicesProvider)
                .WithInstance(this).As<INavigation>()
                .Build();

            objectFactory = Services.GetService<IObjectFactory>();
            this.dispatcher = dispatcher;
        }

        public Task Navigate<TViewModel>(out TViewModel createdInstance, params object[] parameters)
        {
            createdInstance = objectFactory.Create<TViewModel>(parameters);
            return Navigate(createdInstance);
        }

        public Task Navigate(object viewModel)
        {
            viewModels.Push(viewModel);
            return NavigateIntenal(viewModel);
        }

        public Task NavigateBack()
        {
            if (!viewModels.TryPop(out var oldViewModel)) throw new Exception();
            if (!viewModels.TryPeek(out var newViewModel)) throw new Exception();

            var task = NavigateIntenal(newViewModel);

            return task.ContinueWith(t =>
            {
                if (oldViewModel is IDisposable disposable)
                {
                    dispatcher.BeginInvoke( ()=> disposable.Dispose());
                }
            });
        }

        public Task NavigateBackTo<TViewModel>()
        {
            var oldDisposables = new List<IDisposable>();
            object newViewModel = null;

            while (true)
            {
                if (!viewModels.TryPeek(out newViewModel)) throw new Exception();

                if(newViewModel.GetType() == typeof(TViewModel))
                {
                    break;
                }
                if (!viewModels.TryPop(out var oldViewModel)) throw new Exception();

                if(oldViewModel is IDisposable disposable)
                {
                    oldDisposables.Add(disposable);
                }
            }

            var task = NavigateIntenal(newViewModel);
            return task.ContinueWith(t =>
            {
                foreach(var disposable in oldDisposables)
                {
                    dispatcher.BeginInvoke(() => disposable.Dispose());
                }
            });
        }

        private async Task NavigateIntenal(object viewModel)
        {
            var request = new NavigationRequest(viewModel);
            NavigationRequested?.Invoke(this, request);

            if (request.NavigationTasks != null)
            {
                await Task.WhenAll(request.NavigationTasks);
            }

            NavigatedTo?.Invoke(viewModel);
        }
    }
}
