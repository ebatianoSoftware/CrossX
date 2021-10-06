using CrossX.Abstractions.Contracts;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrossX.Abstractions.Mvvm
{
    public abstract class NavigationFrameViewModel: BindingContext, IExceptionHandler
    {
        public INavigation Navigation { get; }
        public ICommand AttachedToFrameCommand { get; }

        public NavigationFrameViewModel(IObjectFactory objectFactory)
        {
            Navigation = objectFactory.Create<INavigation>();
            AttachedToFrameCommand = new AsyncCommand(this, OnAttachedToViewCommand);
        }

        private Task OnAttachedToViewCommand() => InitializeFirstPage();

        protected abstract Task InitializeFirstPage();

        protected virtual void OnException(Exception ex)
        {
            
        }

        void IExceptionHandler.OnException(Exception ex) => OnException(ex);
    }
}
