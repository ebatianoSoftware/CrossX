using XxSchema.Contracts;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"[A-Za-z][A-Za-z0-9-_]*")]
    public struct Length
    {
        public static readonly Length Auto = new Length(rest: -1);
        public static readonly Length Zero = new Length();

        private readonly float units;
        private readonly float pixels;
        private readonly float percent;
        private readonly int rest;

        public bool IsAuto => rest < 0;

        public float Calculate(float onePixelInUnit, float size = 0, float oneRest = 0)
        {
            return rest * oneRest + percent * size + units + pixels * onePixelInUnit;
        }

        public override bool Equals(object other)
        {
            if (other is Length len)
            {
                return Equals(len);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool Equals(Length other)
        {
            return other.units == units &&
                   other.pixels == pixels &&
                   other.percent == percent &&
                   other.rest == rest;
        }

        public Length(float units = 0, float pixels = 0, float percent = 0, int rest = 0)
        {
            this.units = units;
            this.pixels = pixels;
            this.percent = percent;
            this.rest = rest;
        }

        public static implicit operator Length(float units) => new Length(units: units);
        public static implicit operator Length(int units) => new Length(units: units);
        public static implicit operator Length(double units) => new Length(units: (float)units);

        public static bool operator ==(Length l1, Length l2) => l1.Equals(l2);
        public static bool operator !=(Length l1, Length l2) => !l1.Equals(l2);
    }
}
