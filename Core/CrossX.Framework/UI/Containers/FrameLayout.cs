using System;
using System.Numerics;

namespace CrossX.Framework.UI.Containers
{
    public class FrameLayout : ViewContainer
    {
        public FrameLayout(IUIServices services) : base(services)
        {
        }

        protected override void RecalculateLayout()
        {
            base.RecalculateLayout();

            var bounds = Bounds.Deflate(Padding);

            var offset = bounds.TopLeft - Bounds.TopLeft;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                var size = child.CalculateSize(bounds.Size);
                var position = child.CalculatePosition(size, bounds.Size) + offset;
                child.Bounds = new RectangleF(position, size);
            }
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            var bounds = Bounds.Deflate(Padding);
            var offset = bounds.TopLeft - Bounds.TopLeft;

            if ((!Width.IsAuto || HorizontalAlignment == Alignment.Stretch) &&
                (!Height.IsAuto || VerticalAlignment == Alignment.Stretch)) return size;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                var childSize = child.CalculateSize(Vector2.Zero);
                var position = child.CalculatePosition(childSize, Vector2.Zero) + offset;

                if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
                {
                    size.Width = Math.Max(size.Width, position.X + childSize.Width + child.Margin.Right.Calculate());
                }

                if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
                {
                    size.Height = Math.Max(size.Height, position.X + childSize.Height + child.Margin.Bottom.Calculate());
                }
            }

            return size;
        }
    }
}
