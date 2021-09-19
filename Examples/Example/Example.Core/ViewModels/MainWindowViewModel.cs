using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public MainWindowViewModel(IObjectFactory objectFactory) : base(objectFactory)
        {
        }

        protected override object InitializeStrartupViewModel(IObjectFactory objectFactory)
        {
            return objectFactory.Create<MainPageViewModel>();
        }
    }
}
