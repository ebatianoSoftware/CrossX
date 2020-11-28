namespace CrossX.Forms
{
    public interface INavigation
    {
        void Navigate<TViewModel>(NavigationParameters parameters = null, params object[] args) where TViewModel : FormsViewModel;
        void NavigatePopup<TViewModel>(NavigationParameters parameters = null, params object[] args) where TViewModel : FormsViewModel;
        void NavigateBack(NavigationParameters parameters = null);
        void FinishPopup(string closeEvent = null);
        void Clear(string closeEvent = null);
        bool IsTop(FormsViewModel viewModel);
    }
}
