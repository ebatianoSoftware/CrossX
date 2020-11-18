namespace CrossX.Input
{
    public struct TouchPoint
    {
        public const long InvalidTouchId = long.MaxValue;

        public long Id;
        public Vector2 Position;
        public KeyBtnState State;
    }
}
