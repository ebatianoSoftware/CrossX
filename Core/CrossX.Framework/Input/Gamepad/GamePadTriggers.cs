namespace CrossX.Framework.Input.Gamepad
{
    public struct GamePadTriggers
    {
        public float Left { get; }
        public float Right { get; }

        internal GamePadTriggers(float left, float right)
        {
            Left = left;
            Right = right;
        }
    }
}
