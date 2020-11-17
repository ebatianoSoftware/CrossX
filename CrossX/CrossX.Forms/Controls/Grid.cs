namespace CrossX.Forms.Controls
{
    public enum GridLengthMode
    {
        Value,
        Star,
        Auto
    }
    public struct GridLength
    {
        public GridLengthMode Mode { get; }
        public float Value { get; }

        public GridLength(GridLengthMode mode, float value)
        {
            Mode = mode;
            Value = value;
        }
    }

    public class Grid : ContainerControl
    {
        public GridLength[] ColumnDefinitions { get; set; } = new GridLength[] { new GridLength(GridLengthMode.Star, 1) };
        public GridLength[] RowDefinitions { get; set; } = new GridLength[] { new GridLength(GridLengthMode.Star, 1) };
        
        public float[] ActualColumnWidth { get; }
        public float[] ActualColumnHeight { get; }

        public Grid(IControlParent parent) : base(parent)
        {
        }

        protected override void CalculateLayout()
        {
            ShouldCalculateLayout = false;
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].CalculateSizeWithMargins(ClientArea, out var size, out var sizeWithMargins);
                var position = children[idx].CalculatePosition(ClientArea, size);
                children[idx].PositionControl(position, size);
            }
        }
    }
}
