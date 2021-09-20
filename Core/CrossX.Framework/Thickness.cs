using System;
using System.Linq;
using System.Numerics;
using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern(
        @"[-+]?[0-9]*\.?[0-9]+(px)?|" +
        @"[-+]?[0-9]*\.?[0-9]+(px)?[ ]*,[ ]*[-+]?[0-9]*\.?[0-9]+(px)?|" +
        @"[-+]?[0-9]*\.?[0-9]+(px)?[ ]*,[ ]*[-+]?[0-9]*\.?[0-9]+(px)?[ ]*,[ ]*[-+]?[0-9]*\.?[0-9]+(px)?[ ]*,[ ]*[-+]?[0-9]*\.?[0-9]+(px)?")]
    public struct Thickness
    {
        public static readonly Thickness Zero = new Thickness { Left = Length.Zero, Right = Length.Zero, Top = Length.Zero, Bottom = Length.Zero };
        public static Thickness Parse(string text)
        {
            if (text == nameof(Zero)) return Zero;

            var parts = text.Split(',').Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();

            Length left = Length.Zero;
            Length top = Length.Zero;
            Length right = Length.Zero;
            Length bottom = Length.Zero;

            switch (parts.Length)
            {
                case 1:
                    left = top = right = bottom = Length.Parse(parts[0]);
                    break;

                case 2:
                    left = right = Length.Parse(parts[0]);
                    top = bottom = Length.Parse(parts[1]);
                    break;

                case 4:
                    left = Length.Parse(parts[0]);
                    top = Length.Parse(parts[1]);
                    right = Length.Parse(parts[2]);
                    bottom = Length.Parse(parts[3]);
                    break;

                default:
                    throw new FormatException();
            }

            return new Thickness(left, top, right, bottom);
        }

        public Length Left;
        public Length Top;
        public Length Right;
        public Length Bottom;

        public float Width => Left.Calculate() + Right.Calculate();
        public float Height => Top.Calculate() + Bottom.Calculate();

        public Thickness(Length left, Length top, Length right, Length bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
