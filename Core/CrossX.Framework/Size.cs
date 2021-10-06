using System;

namespace CrossX.Framework
{
    public struct Size : IEquatable<Size>
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator SizeF(Size size) => new SizeF(size.Width, size.Height);

        public static readonly Size Empty = new Size(0, 0);

        public override bool Equals(object obj)
        {
            return obj is Size size && Equals(size);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public static bool operator ==(Size s1, Size s2) => s1.Equals(s2);
        public static bool operator !=(Size s1, Size s2) => !s1.Equals(s2);
    }
}
