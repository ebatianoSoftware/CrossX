namespace CrossX.Framework.UI
{
    public class FrameLayout : ViewContainer
    {
        public override void RecalculateLayout()
        {
            base.RecalculateLayout();

            var onePixelUnit = 1;
            var bounds = Bounds.Inflate(Padding, onePixelUnit);

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
