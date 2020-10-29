// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;

namespace CrossX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct Color4 : IEquatable<Color4>
    {
        public Color4(long color)
        {
            R = (byte)(color & 0xff);
            G = (byte)((color >> 8) & 0xff);
            B = (byte)((color >> 16) & 0xff);
            A = (byte)((color >> 24) & 0xff);
        }

        public Color4(byte red, byte green, byte blue, byte alpha = 255)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color4(float red, float green, float blue, float alpha = 1.0f)
        {
            R = (byte)(red * 255);
            G = (byte)(green * 255);
            B = (byte)(blue * 255);
            A = (byte)(alpha * 255);
        }

        public Color4 Mix(Color4 other, float mix)
        {
            var red = Rf * (1 - mix) + other.Rf * mix;
            var green = Gf * (1 - mix) + other.Gf * mix;
            var blue = Bf * (1 - mix) + other.Bf * mix;
            var alpha = Af * (1 - mix) + other.Af * mix;

            return new Color4(red, green, blue, alpha);
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

        public static Color4 operator *(Color4 color1, Color4 color2)
        {
            return new Color4(color1.Rf * color2.Rf, color1.Gf * color2.Gf, color1.Bf * color2.Bf, color1.Af * color2.Af);
        }

        public static Color4 operator *(Color4 color, float multiply)
        {
            var mul = multiply;

            return new Color4
            {
                Rf = color.Rf * mul,
                Gf = color.Gf * mul,
                Bf = color.Bf * mul,
                Af = color.Af * mul
            };
        }

        public static Color4 operator *(Color4 color, double multiply)
        {
            return color * (float)multiply;
        }

        public override string ToString()
        {
            return $"Color({R}, {G}, {B}, {A})";
        }

        public static Color4 FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color4((byte)r, (byte)g, (byte)b) * (a / 255.0f);
        }

        public static Color4 FromNonPremultiplied(long color)
        {
            var r = (byte)(color & 0xff);
            var g = (byte)((color >> 8) & 0xff);
            var b = (byte)((color >> 16) & 0xff);
            var a = (byte)((color >> 24) & 0xff);
            return new Color4(r, g, b) * a;
        }

        public bool Equals(Color4 other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Color4 c1, Color4 c2) => c1.Equals(c2);

        public static bool operator !=(Color4 c1, Color4 c2) => !c1.Equals(c2);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color4 && Equals((Color4)obj);
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

        public static Color4 FromInt32(int intColor)
        {
            var a = (intColor >> 24) & 0xff;
            var r = (intColor >> 16) & 0xff;
            var g = (intColor >> 8) & 0xff;
            var b = intColor & 0xff;

            return FromNonPremultiplied(r, g, b, a);
        }
    }
}
