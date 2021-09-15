using System;

namespace CrossX.Framework
{
    public struct Rectangle
    {
        public readonly int X;
        public readonly int Y;

        public readonly int Width { get; }
        public readonly int Height { get; }

        public int Right => X + Width;
        public int Bottom => Y + Height;

        public Size Size => new Size(Width, Height);

        public bool IsEmpty => Width <= 0 || Height <= 0;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator RectangleF (Rectangle rect) => new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);

        public Rectangle Intersect(Rectangle clip)
        {
            return clip;
        }

        public static bool operator ==(Rectangle r1, Rectangle r2)
        {
            return r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height;
        }

        public static bool operator !=(Rectangle r1, Rectangle r2)
        {
            return r1.X != r2.X || r1.Y != r2.Y || r1.Width != r2.Width || r1.Height != r2.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle rect) return rect == this;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Width ^ Height;
        }
    }
}
