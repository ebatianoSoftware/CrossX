using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Templates;
using CrossX.Framework.XxTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xx;
using Xx.Definition;

namespace CrossX.Framework.UI.Containers
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(DataTemplateElement), typeof(ContainerTemplateElement))]
    public class ItemsView : View, IElementsContainer, IViewParent
    {
        private readonly XxDefinitionObjectFactory definitionObjectFactory;
        private XxElement dataTemplate;
        private ViewContainer container;
        private IList items;
        private bool layoutInvalid;

        public IList Items
        {
            get => items;
            set
            {
                UpdateItems(value);
            }
        }

        private void UpdateItems(IList value)
        {
            
            if(value is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged -= Items_CollectionChanged;
            }

            container.Children.Clear();
            items = value;

            if(items is INotifyCollectionChanged ncc2)
            {
                ncc2.CollectionChanged += Items_CollectionChanged;
            }
            CreateItems();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch(args.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (args.NewItems == null) return;

                    for(var idx = 0; idx < args.NewItems.Count; ++idx)
                    {
                        CreateItem(idx + args.NewStartingIndex, args.NewItems[idx]);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    container.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new InvalidOperationException();

                case NotifyCollectionChangedAction.Remove:
                    container.Children.RemoveAt(args.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    container.Children.Move(args.OldStartingIndex, args.NewStartingIndex);
                    break;
            }

            InvalidateLayout();
        }

        private void CreateItem(int index, object item)
        {
            var view = definitionObjectFactory.CreateObject<View>(dataTemplate);
            view.DataContext = item;

            container.Children.Insert(index, view);
        }

        private void CreateItems()
        {
            if (Items == null) return;

            for(var idx =0; idx < Items.Count; ++idx)
            {
                CreateItem(idx, Items[idx]);
            }
            InvalidateLayout();
        }

        public ItemsView(XxDefinitionObjectFactory definitionObjectFactory, IUIServices services) : base(services)
        {
            this.definitionObjectFactory = definitionObjectFactory;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            dataTemplate = elements.Where(o => o is DataTemplateElement).Cast<DataTemplateElement>().First().Element;
            var containerTemplate = elements.Where(o => o is ContainerTemplateElement).Cast<ContainerTemplateElement>().First().Element;

            container = definitionObjectFactory.CreateObject<ViewContainer>(containerTemplate);
            container.Parent = this;
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            return container.CalculateSize(parentSize);
        }

        protected override void RecalculateLayout()
        {
            base.RecalculateLayout();
            container.Bounds = new RectangleF(0, 0, ScreenBounds.Width, ScreenBounds.Height);   
        }

        public void InvalidateLayout()
        {
            layoutInvalid = true;
            Parent?.InvalidateLayout();
        }

        protected override void OnUpdate(float time)
        {
            if (layoutInvalid)
            {
                RecalculateLayout();
                Services.RedrawService.RequestRedraw();
            }

            base.OnUpdate(time);
            container.Update(time);
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);
            container.Render(canvas, opacity);
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            return container.PreviewGesture(gesture);
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            return container.ProcessGesture(gesture);
        }
    }
}
