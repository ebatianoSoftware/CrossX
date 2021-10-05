using CrossX.Abstractions.IoC;
using System;
using System.Threading.Tasks;

namespace CrossX.Abstractions.Navigation
{
    public interface INavigation
    {
        public IServicesProvider Services { get; }
        Task Navigate<TViewModel>(out TViewModel createdInstance, params object[] parameters);
        Task Navigate(object viewModel);
        Task NavigateBack();
        Task NavigateBackTo<TViewModel>();

        event Action<object> NavigatedTo;
    }
}
