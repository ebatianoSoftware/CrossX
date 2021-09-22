using System.Numerics;

namespace CrossX.Framework.Input
{
    public class Gesture
    {
        public GestureType GestureType { get; internal set; }
        public PointerId PointerId { get; internal set; }
        public Vector2 Position { get; internal set; }
    }
}
