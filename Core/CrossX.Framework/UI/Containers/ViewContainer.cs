using CrossX.Framework.Graphics;
using System.Collections.Generic;
using Xx;
using Xx.Toolkit;

namespace CrossX.Framework.UI.Containers
{
    [XxSchemaExport(XxChildrenMode.Multiple)]
    public abstract class ViewContainer : View, IElementsContainer
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

        public ViewContainer()
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
            base.OnUpdate(time);
            if (layoutInvalid)
            {
                RecalculateLayout();
            }
        }

        public override void RecalculateLayout()
        {
            base.RecalculateLayout();
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
    }
}
