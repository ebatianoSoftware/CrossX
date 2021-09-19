using CrossX.Abstractions.Mvvm;
using CrossX.Abstractions.Navigation;

namespace Example.Core.ViewModels
{
    internal class MainPageViewModel : NavigatedViewModel
    {
        public MainPageViewModel(INavigation navigation) : base(navigation)
        {
        }
    }
}