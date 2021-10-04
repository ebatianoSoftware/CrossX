using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CrossX.Framework.UI.Containers
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
                if (args.NewItems != null)
                {
                    foreach (var item in args.NewItems)
                    {
                        if (item is View view)
                        {
                            view.Parent = owner;
                        }
                    }
                }
            }

            if(args.Action == NotifyCollectionChangedAction.Reset || args.Action == NotifyCollectionChangedAction.Replace)
            {
                if (args.OldItems != null)
                {
                    foreach (var item in args.OldItems)
                    {
                        if (item is IDisposable disp)
                        {
                            disp.Dispose();
                        }
                    }
                }
            }

            owner.InvalidateLayout();
            base.OnCollectionChanged(args);
        }
    }
}
