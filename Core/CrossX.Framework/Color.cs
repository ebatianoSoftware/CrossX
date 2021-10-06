using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"#[\dA-Fa-f]{6}([\dA-Fa-f][\dA-Fa-f])?|#[\dA-Fa-f]{3}([\dA-Fa-f])?")]
    public partial struct Color : IEquatable<Color>
    {
        

        private static Dictionary<string, Color> builtInColors = null;

        private static void InitBuiltInValues()
        {
            builtInColors = new Dictionary<string, Color>();
            var fields = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(f => f.IsInitOnly);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Color))
                {
                    builtInColors.Add(field.Name, (Color)field.GetValue(null));
                }
            }
        }

        public static Color Parse(string text)
        {
            if(builtInColors == null)
            {
                InitBuiltInValues();
            }
            if (builtInColors.TryGetValue(text, out var color)) return color;

            string colorcode = text;
            colorcode = colorcode.TrimStart('#').ToUpperInvariant();

            switch(colorcode.Length)
            {
                case 6: 
                    return new Color(byte.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
                                byte.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                                byte.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber));
                case 8: 
                    return new Color(
                            byte.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                            byte.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
                            byte.Parse(colorcode.Substring(6, 2), NumberStyles.HexNumber),
                            byte.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber));

                case 3:
                    var r = byte.Parse(colorcode.Substring(0, 1), NumberStyles.HexNumber);
                    var g = byte.Parse(colorcode.Substring(1, 1), NumberStyles.HexNumber);
                    var b = byte.Parse(colorcode.Substring(2, 1), NumberStyles.HexNumber);
                    return new Color((byte)(r | (r << 4)), (byte)(g | (g << 4)), (byte)(b | (b << 4)));

                case 4:
                    var aa = byte.Parse(colorcode.Substring(0, 1), NumberStyles.HexNumber);
                    var rr = byte.Parse(colorcode.Substring(1, 1), NumberStyles.HexNumber);
                    var gg = byte.Parse(colorcode.Substring(2, 1), NumberStyles.HexNumber);
                    var bb = byte.Parse(colorcode.Substring(3, 1), NumberStyles.HexNumber);
                    return new Color((byte)(rr | (rr << 4)), (byte)(gg | (gg << 4)), (byte)(bb | (bb << 4)), (byte)(aa | (aa << 4)));
            }

            throw new FormatException();
        }

        public Color(uint color)
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
            var alpha = Af * (1 - mix) + other.Af * mix;
            var red = Rf * (1 - mix) + other.Rf * mix;
            var green = Gf * (1 - mix) + other.Gf * mix;
            var blue = Bf * (1 - mix) + other.Bf * mix;

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
