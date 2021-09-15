using System;
using System.Runtime.InteropServices;

namespace CrossX.Framework
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct Color : IEquatable<Color>
    {
        public Color(long color)
        {
            R = (byte)(color & 0xff);
            G = (byte)((color >> 8) & 0xff);
            B = (byte)((color >> 16) & 0xff);
            A = (byte)((color >> 24) & 0xff);
        }

        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color(float red, float green, float blue, float alpha = 1.0f)
        {
            R = (byte)(red * 255);
            G = (byte)(green * 255);
            B = (byte)(blue * 255);
            A = (byte)(alpha * 255);
        }

        public Color Mix(Color other, float mix)
        {
            var red = Rf * (1 - mix) + other.Rf * mix;
            var green = Gf * (1 - mix) + other.Gf * mix;
            var blue = Bf * (1 - mix) + other.Bf * mix;
            var alpha = Af * (1 - mix) + other.Af * mix;

            return new Color(red, green, blue, alpha);
        }

        public int ToInt32()
        {
            long argb = (B | (G << 8) | (R << 16) | (A << 24));
            return (int)argb;
        }

        public uint ToUInt32()
        {
            long argb = (R | (G << 8) | (B << 16) | (A << 24));
            return (uint)argb;
        }

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public float Rf
        {
            get => R / 255.0f;
            set => R = (byte)(Math.Min(255, value * 255.0f));
        }

        public float Gf
        {
            get => G / 255.0f;
            set => G = (byte)(Math.Min(255, value * 255.0f));
        }

        public float Bf
        {
            get => B / 255.0f;
            set => B = (byte)(Math.Min(255, value * 255.0f));
        }

        public float Af
        {
            get => A / 255.0f;
            set => A = (byte)(Math.Min(255, value * 255.0f));
        }

        public static Color operator *(Color color1, Color color2)
        {
            return new Color(color1.Rf * color2.Rf, color1.Gf * color2.Gf, color1.Bf * color2.Bf, color1.Af * color2.Af);
        }

        public static Color operator *(Color color, float multiply)
        {
            var mul = multiply;

            return new Color
            {
                Rf = color.Rf * mul,
                Gf = color.Gf * mul,
                Bf = color.Bf * mul,
                Af = color.Af * mul
            };
        }

        public static Color operator *(Color color, double multiply)
        {
            return color * (float)multiply;
        }

        public override string ToString()
        {
            return $"Color({R}, {G}, {B}, {A})";
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Color c1, Color c2) => c1.Equals(c2);

        public static bool operator !=(Color c1, Color c2) => !c1.Equals(c2);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }
    }
    
}
