using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CrossX.Framework.UI
{
    public class ChildrenCollection : ObservableCollection<View>
    {
        private readonly ViewContainer owner;

        public ChildrenCollection(ViewContainer owner)
        {
            this.owner = owner;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in args.NewItems)
                {
                    if (item is View view)
                    {
                        view.Parent = owner;
                    }
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove || args.Action == NotifyCollectionChangedAction.Reset || args.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in args.OldItems)
                {
                    if (item is View view)
                    {
                        view.Parent = owner;
                    }
                }
            }
            owner.InvalidateLayout();
            base.OnCollectionChanged(args);
        }
    }
}
