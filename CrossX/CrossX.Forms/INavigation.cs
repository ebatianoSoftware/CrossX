namespace CrossX.Forms
{
    public interface INavigation
    {
        void Navigate<TViewModel>(params object[] args) where TViewModel : FormsViewModel;
        void NavigatePopup<TViewModel>(params object[] args) where TViewModel : FormsViewModel;
        void NavigateBack();
        void FinishPopup();
    }
}
