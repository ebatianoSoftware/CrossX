using System.Numerics;

namespace CrossX.Framework.Input
{
    public class Gesture
    {
        public GestureType GestureType { get; set; }
        public PointerId PointerId { get; set; }
        public Vector2 Position { get; set; }
        public CursorType SetCursor { get; set; }
    }
}
