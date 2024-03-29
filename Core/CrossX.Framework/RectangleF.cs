﻿using System;
using System.Numerics;

namespace CrossX.Framework
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        public float X;
        public float Y;

        public float Width;

        public float Height;

        public Vector2 TopLeft => new Vector2(X, Y);
        public Vector2 TopRight => new Vector2(X + Width, Y);
        public Vector2 BottomLeft => new Vector2(X, Y + Height);
        public Vector2 BottomRight => new Vector2(X + Width, Y + Height);
        public SizeF Size => new SizeF(Width, Height);

        public Vector2 Center => new Vector2(X + Width / 2, Y + Height / 2);

        public float Left 
        { 
            get => X;
            set
            {
                Width -= value - X;
                X = value;
            }
        }

        public float Top
        {
            get => Y;
            set
            {
                Height -= value - Y;
                Y = value;
            }
        }

        public float Right
        {
            get => X + Width;
            set
            {
                Width = value - X;
            }
        }

        public float Bottom
        {
            get => Y + Height;
            set
            {
                Height = value - Y;
            }
        }

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

        public RectangleF Deflate(Thickness padding, SizeF? size = null)
        {
            var marginLeft = padding.Left.Calculate(size.GetValueOrDefault().Width);
            var marginRight = padding.Right.Calculate(size.GetValueOrDefault().Width);
            var marginTop = padding.Top.Calculate(size.GetValueOrDefault().Height);
            var marginBottom = padding.Bottom.Calculate(size.GetValueOrDefault().Height);

            return new RectangleF(X + marginLeft, Y + marginTop, Width - marginLeft - marginRight, Height - marginTop - marginBottom);
        }

        public RectangleF Deflate(float w, float h)
        {
            return new RectangleF(X + w, Y + h, Width - w * 2, Height - h * 2);
        }

        public override bool Equals(object obj)
        {
            return obj is RectangleF f && Equals(f);
        }

        public RectangleF Intersect(RectangleF other)
        {
            var left = Math.Max(other.X, X);
            var right = Math.Min(other.Right, Right);

            var top = Math.Max(other.Y, Y);
            var bottom = Math.Min(other.Bottom, Bottom);

            return new RectangleF(left, top, right - left, bottom - top);
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

        public bool Contains(Vector2 position)
        {
            if (position.X > Right) return false;
            if (position.Y > Bottom) return false;

            if (position.X < X) return false;
            if (position.Y < Y) return false;

            return true;
        }
    }
}
