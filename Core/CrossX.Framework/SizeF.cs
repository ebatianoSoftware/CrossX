using System;
using System.Numerics;

namespace CrossX.Framework
{
    public struct SizeF : IEquatable<SizeF>
    {
        public float Width;
        public float Height;

        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static readonly SizeF Zero = new SizeF(0, 0);

        public static implicit operator Vector2(SizeF size) => new Vector2(size.Width, size.Height);
        public static implicit operator SizeF(Vector2 size) => new SizeF(size.X, size.Y);

        public Size Round() => new Size((int)Width, (int)Height);

        public override bool Equals(object obj)
        {
            return obj is SizeF f && Equals(f);
        }

        public bool Equals(SizeF other)
        {
            return Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public static bool operator ==(SizeF s1, SizeF s2) => s1.Equals(s2);
        public static bool operator !=(SizeF s1, SizeF s2) => !s1.Equals(s2);
    }
}
