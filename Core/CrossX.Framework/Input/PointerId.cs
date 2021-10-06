using System;

namespace CrossX.Framework.Input
{
    public struct PointerId : IEquatable<PointerId>
    {
        public static readonly PointerId None = new PointerId();

        public PointerKind Kind { get; }
        public long TouchId { get; }

        public PointerId(PointerKind kind)
        {
            Kind = kind;
            TouchId = 0;
        }

        public PointerId(long touchId)
        {
            Kind = PointerKind.Touch;
            TouchId = touchId;
        }

        public override bool Equals(object obj)
        {
            return obj is PointerId id && Equals(id);
        }

        public bool Equals(PointerId other)
        {
            return Kind == other.Kind &&
                   TouchId == other.TouchId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind, TouchId);
        }

        public static bool operator ==(PointerId p1, PointerId p2) => p1.Equals(p2);
        public static bool operator !=(PointerId p1, PointerId p2) => !p1.Equals(p2);
    }
}
