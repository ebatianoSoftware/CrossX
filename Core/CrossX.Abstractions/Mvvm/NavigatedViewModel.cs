using CrossX.Abstractions.Navigation;

namespace CrossX.Abstractions.Mvvm
{
    public abstract class NavigatedViewModel: BindingContext
    {
        protected INavigation Navigation { get; }
        public NavigatedViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }
    }
}
