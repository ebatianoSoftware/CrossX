using System.Numerics;

namespace CrossX.Input
{
    public struct TouchPoint
    {
        public const long InvalidTouchId = long.MaxValue;

        // TODO: Change Id to struct with touch id as long and mouse buttons enum value
        public long Id;
        public Vector2 Position;
        public KeyBtnState State;
    }
}
