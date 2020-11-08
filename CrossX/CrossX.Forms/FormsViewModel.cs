namespace CrossX.Forms
{
    public abstract class FormsViewModel: ObservableDataModel
    {
        protected INavigation Navigation { get; private set; }
        internal void SetNavigation(INavigation navigation) => Navigation = navigation;
    }
}
