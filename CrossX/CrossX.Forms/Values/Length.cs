namespace CrossX.Forms.Values
{
    public struct Length
    {
        public static readonly Length Auto = new Length(true);
        public static readonly Length Zero = new Length();

        public bool IsAuto { get; }
        public float Percent { get; }
        public float Value { get; }

        public Length(bool auto) : this()
        {
            IsAuto = auto;
        }

        public Length(float percent, float value): this()
        {
            Percent = percent;
            Value = value;
        }

        public float Calculate(float size) => Value + Percent * size;
    }
}
