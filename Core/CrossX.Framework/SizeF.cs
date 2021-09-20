using System.Numerics;

namespace CrossX.Framework
{
    public struct SizeF
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
    }
}
