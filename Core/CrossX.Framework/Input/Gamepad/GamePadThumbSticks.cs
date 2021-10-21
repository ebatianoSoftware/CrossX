using System.Numerics;

namespace CrossX.Framework.Input.Gamepad
{
    public struct GamePadThumbSticks
    {
        public Vector2 Left { get; }
        public Vector2 Right { get; }

        internal GamePadThumbSticks(Vector2 left, Vector2 right)
        {
            Left = left;
            Right = right;
        }
    }
}
