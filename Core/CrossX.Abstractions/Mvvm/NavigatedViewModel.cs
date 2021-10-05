using CrossX.Abstractions.Contracts;
using CrossX.Abstractions.Navigation;
using System;

namespace CrossX.Abstractions.Mvvm
{
    public abstract class NavigatedViewModel: BindingContext, IExceptionHandler
    {
        protected INavigation Navigation { get; }
        public NavigatedViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }

        void IExceptionHandler.OnException(Exception ex)
        {
            OnException(ex);
        }

        protected virtual void OnException(Exception ex) { }
    }
}
