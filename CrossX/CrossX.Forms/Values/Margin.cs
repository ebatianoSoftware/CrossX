namespace CrossX.Forms.Values
{
    public struct Margin
    {
        public static readonly Margin Zero = new Margin(0, 0, 0, 0);
        public float Left { get; }
        public float Top { get; }
        public float Right { get; }
        public float Bottom { get; }

        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
