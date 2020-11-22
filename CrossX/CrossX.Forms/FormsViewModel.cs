using System;

namespace CrossX.Forms
{
    public abstract class FormsViewModel: ObservableDataModel, IDisposable
    {
        protected INavigation Navigation { get; private set; }

        internal void SetNavigation(INavigation navigation) => Navigation = navigation;

        internal void CallNavigateFrom() => OnNavigateFrom();
        internal void CallNavigateTo() => OnNavigateTo();
        protected virtual void OnNavigateFrom()
        { 
        }

        protected virtual void OnNavigateTo()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
