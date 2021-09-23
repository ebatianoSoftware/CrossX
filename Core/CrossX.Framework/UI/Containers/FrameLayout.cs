using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Containers
{
    public class FrameLayout : ViewContainer
    {
        public FrameLayout(IRedrawService redrawService) : base(redrawService)
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
    }
}
