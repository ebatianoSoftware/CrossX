using System.Numerics;

namespace CrossX.Framework.Input
{
    internal class GestureProcessor
    {
        private readonly Gesture gesture = new Gesture();

        public Gesture OnPointerDown(PointerId pointerId, Vector2 position)
        {
            gesture.GestureType = GestureType.PointerDown;
            gesture.PointerId = pointerId;
            gesture.Position = position;
            gesture.SetCursor = CursorType.Default;
            return gesture;
        }

        public Gesture OnPointerUp(PointerId pointerId, Vector2 position)
        {
            gesture.GestureType = GestureType.PointerUp;
            gesture.PointerId = pointerId;
            gesture.Position = position;
            gesture.SetCursor = CursorType.Default;
            return gesture;
        }

        public Gesture OnPointerMove(PointerId pointerId, Vector2 position)
        {
            gesture.GestureType = GestureType.PointerMove;
            gesture.PointerId = pointerId;
            gesture.Position = position;
            gesture.SetCursor = CursorType.Default;
            return gesture;
        }

        public Gesture OnPointerCancel(PointerId pointerId)
        {
            gesture.GestureType = GestureType.CancelPointer;
            gesture.PointerId = pointerId;
            gesture.Position = Vector2.Zero;
            gesture.SetCursor = CursorType.Default;
            return gesture;
        }
    }
}
