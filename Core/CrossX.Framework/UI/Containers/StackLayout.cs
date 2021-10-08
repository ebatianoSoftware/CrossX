using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework.UI.Containers
{
    public class StackLayout : ViewContainer
    {
        private Length spacing;
        private Orientation orientation = Orientation.Vertical;

        public Orientation Orientation 
        { 
            get => orientation; 
            set => SetPropertyAndRecalcLayout(ref orientation, value); 
        }

        public Length Spacing
        {
            get => spacing;
            set => SetPropertyAndRecalcLayout(ref spacing, value);
        }

        public StackLayout(IUIServices services) : base(services)
        {
            HorizontalAlignment = Alignment.Start;
            VerticalAlignment = Alignment.Start;
        }

        protected override void RecalculateLayout()
        {
            base.RecalculateLayout();

            var bounds = Bounds.Deflate(Padding);
            var offset = bounds.TopLeft - Bounds.TopLeft;
            var spacing = Spacing.Calculate();

            if (orientation == Orientation.Horizontal)
            {
                var mySize = new SizeF(0, bounds.Height);
                var positionX = offset.X;
                for (var idx = 0; idx < Children.Count; ++idx)
                {
                    var child = Children[idx];
                    if (!child.DisplayVisible) continue;

                    var size = child.CalculateSize(mySize);
                    var position = child.CalculatePosition(size, mySize) + offset;

                    child.Bounds = new RectangleF(positionX + child.Margin.Left.Calculate(), position.Y, size.Width, size.Height);
                    positionX += size.Width + child.Margin.Width + spacing;
                }
            }
            else
            {
                var mySize = new SizeF(bounds.Width, 0);
                var positionY = offset.Y;
                for (var idx = 0; idx < Children.Count; ++idx)
                {
                    var child = Children[idx];
                    if (!child.DisplayVisible) continue;

                    var size = child.CalculateSize(mySize);
                    var position = child.CalculatePosition(size, mySize) + offset;

                    child.Bounds = new RectangleF(position.X, positionY + child.Margin.Top.Calculate(), size.Width, size.Height);
                    positionY += size.Height + child.Margin.Height + spacing;
                }
            }
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            if (Orientation == Orientation.Horizontal)
            {
                if (Height.IsAuto && VerticalAlignment != Alignment.Stretch) size.Height = 0;
                size = CalculateWidth(size);
            }
            else
            {
                if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch) size.Width = 0;
                size = CalculateHeight(size);
            }
            return size;
        }

        private SizeF CalculateWidth(SizeF size)
        {
            var width = Padding.Width;
            var height = size.Height;

            var spacing = Spacing.Calculate();
            var mySize = new SizeF(0, size.Height - Padding.Height);

            int visibleChildren = 0;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];

                if (child.DisplayVisible)
                {
                    visibleChildren++;
                    var childSize = child.CalculateSize(mySize);
                    width += childSize.Width;
                    width += child.Margin.Width;

                    height = Math.Max(height, childSize.Height + child.Margin.Height + Padding.Height);
                }
            }

            width += Math.Max(0, visibleChildren - 1) * spacing;
            return new SizeF(width, height);
        }

        private SizeF CalculateHeight(SizeF size)
        {
            var height = Padding.Height;
            var width = size.Width;

            var spacing = Spacing.Calculate();
            var mySize = new SizeF(size.Width - Padding.Width, 0);

            int visibleChildren = 0;

            for(var idx =0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];

                if(child.DisplayVisible)
                {
                    visibleChildren++;
                    var childSize = child.CalculateSize(mySize);
                    height += childSize.Height;
                    height += child.Margin.Height;

                    width = Math.Max(width, childSize.Width + child.Margin.Width + Padding.Width);
                }
            }

            height += Math.Max(0, visibleChildren - 1) * spacing;
            return new SizeF(width, height);
        }

        public override void InvalidateLayout()
        {
            Parent?.InvalidateLayout();
            base.InvalidateLayout();
        }
    }
}
