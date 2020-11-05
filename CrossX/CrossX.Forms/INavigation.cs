namespace CrossX.Forms
{
    public interface INavigation
    {
        void Navigate<TViewModel>(params object[] args);
    }
}
