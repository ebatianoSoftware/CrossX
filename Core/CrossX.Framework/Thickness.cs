namespace CrossX.Framework
{
    public struct Thickness
    {
        public static readonly Thickness Zero = new Thickness { Left = Length.Zero, Right = Length.Zero, Top = Length.Zero, Bottom = Length.Zero };

        public Length Left;
        public Length Top;
        public Length Right;
        public Length Bottom;

        public Thickness(Length left, Length top, Length right, Length bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
