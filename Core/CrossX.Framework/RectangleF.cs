using System;
using System.Numerics;

namespace CrossX.Framework
{
    public struct RectangleF : IEquatable<RectangleF>
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

        public Vector2 Center => new Vector2(X + Width / 2, Y + Height / 2);

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

        public RectangleF(Vector2 position, SizeF size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleF Offset(Vector2 offset)
        {
            return new RectangleF(X + offset.X, Y + offset.Y, Width, Height);
        }

        public RectangleF Inflate(Thickness padding, float onePixelUnit)
        {
            var marginLeft = padding.Left.Calculate(onePixelUnit);
            var marginRight = padding.Right.Calculate(onePixelUnit);
            var marginTop = padding.Top.Calculate(onePixelUnit);
            var marginBottom = padding.Bottom.Calculate(onePixelUnit);

            return new RectangleF(X + marginLeft, Y + marginTop, Width - marginLeft - marginRight, Height - marginTop - marginBottom);
        }

        public override bool Equals(object obj)
        {
            return obj is RectangleF f && Equals(f);
        }

        public bool Equals(RectangleF other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public static bool operator ==(RectangleF r1, RectangleF r2) => r1.Equals(r2);
        public static bool operator !=(RectangleF r1, RectangleF r2) => !r1.Equals(r2);
    }
}
