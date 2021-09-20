using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using System.Threading.Tasks;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public MainWindowViewModel(IObjectFactory objectFactory) : base(objectFactory)
        {
        }

        protected override Task InitializeFirstPage()
        {
            return Navigation.Navigate<MainPageViewModel>(out var viewModel);
        }
    }
}
