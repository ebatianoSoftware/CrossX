using System.Globalization;
using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"[-]?([0-9]*[.])?[0-9]+(px|%|\*)?")]
    public struct Length
    {
        public enum Type
        {
            Auto,
            Units,
            Pixels,
            Percent,
            Star
        }

        public static readonly Length Auto = new Length(0, Type.Auto);
        public static readonly Length Zero = new Length(0, Type.Units);

        public static Length Parse(string text)
        {
            switch(text)
            {
                case nameof(Auto):
                    return Auto;

                case nameof(Zero):
                    return Zero;
            }

            text = text.Trim();

            Type type = Type.Units;
            if(text.EndsWith("px"))
            {
                text = text.Trim('p', 'x');
                type = Type.Pixels;
            }
            else if (text.EndsWith("%"))
            {
                text = text.Trim('%');
                type = Type.Percent;
            }
            else if (text.EndsWith("*"))
            {
                text = text.Trim('*');
                if(string.IsNullOrWhiteSpace(text))
                {
                    text = "1";
                }
                type = Type.Star;
            }

            var value = float.Parse(text, CultureInfo.InvariantCulture);
            return new Length(value, type);
        }

        private readonly float value;
        private readonly Type type;

        public bool IsAuto => type == Type.Auto;

        public Length(float value, Type type = Type.Units)
        {
            this.value = value;
            this.type = type;
        }

        public float Calculate(float size = 0, float oneStar = 0)
        {
            switch(type)
            {
                case Type.Percent:
                    return value / 100f * size;

                case Type.Pixels:
                    return value / UiUnit.PixelsPerUnit;

                case Type.Star:
                    return value * oneStar;
            }
            return value;
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
            return other.value == value &&
                   other.type == type;
        }

        public static implicit operator Length(float units) => new Length(units);
        public static implicit operator Length(int units) => new Length(units);
        public static implicit operator Length(double units) => new Length((float)units);
        public static bool operator ==(Length l1, Length l2) => l1.Equals(l2);
        public static bool operator !=(Length l1, Length l2) => !l1.Equals(l2);
    }
}
