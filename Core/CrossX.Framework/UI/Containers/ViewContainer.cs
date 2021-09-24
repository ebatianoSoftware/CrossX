using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework.UI.Containers
{
    [XxSchemaExport(XxChildrenMode.Multiple)]
    public abstract class ViewContainer : View, IElementsContainer, IViewParent
    {
        private bool layoutInvalid;
        private Thickness padding;

        public ChildrenCollection Children { get; }

        public Thickness Padding
        {
            get => padding;
            set
            {
                if (SetProperty(ref padding, value))
                {
                    InvalidateLayout();
                }
            }
        }

        public ViewContainer(IUIServices services) : base(services)
        {
            Children = new ChildrenCollection(this);
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                child.Render(canvas);
            }
        }


        public void InvalidateLayout() => layoutInvalid = true;

        protected override void OnUpdate(float time)
        {
            if (layoutInvalid)
            {
                RecalculateLayout();
                Services.RedrawService.RequestRedraw();
            }

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                child.Update(time);
            }

            base.OnUpdate(time);
        }

        protected override void RecalculateLayout()
        {
            layoutInvalid = false;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach(var element in elements)
            {
                if(element is View view)
                {
                    Children.Add(view);
                }
            }
            InvalidateLayout();
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            for (var idx = Children.Count-1; idx >=0; --idx)
            {
                if (Children[idx].PreviewGesture(gesture)) return true;
            }
            return false;
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            for (var idx = Children.Count - 1; idx >= 0; --idx)
            {
                if (Children[idx].ProcessGesture(gesture)) return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                Children[idx].Dispose();
            }
        }
    }
}
