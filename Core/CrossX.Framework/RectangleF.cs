using System.Numerics;

namespace CrossX.Framework
{
    public struct RectangleF
    {
        public readonly float X;
        public readonly float Y;

        public readonly float Width;
        public readonly float Height;

        public Vector2 TopLeft => new Vector2(X, Y);
        public Vector2 TopRight => new Vector2(X + Width, Y);
        public Vector2 BottomLeft => new Vector2(X, Y + Height);
        public Vector2 BottomRight => new Vector2(X + Width, Y + Height);
        public SizeF Size => new SizeF(Width, Height);

        public float Left => X;
        public float Top => Y;

        public float Right => X + Width;
        public float Bottom => Y + Height;

        public bool IsEmpty => Width <= 0 || Height <= 0;

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
