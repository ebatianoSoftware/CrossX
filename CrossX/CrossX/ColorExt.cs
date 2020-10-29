using System.Drawing;

namespace CrossX
{
    public static class ColorExt
    {
        public static float Rf(this Color color) => color.R / 255.0f;
        public static float Gf(this Color color) => color.G / 255.0f;
        public static float Bf(this Color color) => color.B / 255.0f;
        public static float Af(this Color color) => color.A / 255.0f;

        public static Color SetAlpha(this Color color, float alpha)
        {
            return Color.FromArgb((byte)(color.R * alpha), (byte)(color.G * alpha), (byte)(color.B * alpha), (byte)(alpha * 255));
        }
    }

    
}
