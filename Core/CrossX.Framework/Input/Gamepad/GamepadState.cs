namespace CrossX.Framework.Input.Gamepad
{

    public struct GamePadButtonsSet
    {
        public GamePadButtons Down { get; }
        public GamePadButtons JustPressed { get; }
        public GamePadButtons JustReleased { get; }

        internal GamePadButtonsSet(GamePadButtons previous, GamePadButtons current)
        {
            JustPressed = current & (~previous);
            JustReleased = previous & (~current);
            Down = current;
        }
    }

    public struct GamepadState
    {
        public static readonly GamepadState NotConnected = new GamepadState();

        public bool Connected { get; }
        public GamePadThumbSticks ThumbSticks { get; }
        public GamePadTriggers Triggers { get; }
        public GamePadButtonsSet Buttons { get; }

        public GamepadState(GamePadThumbSticks thumbSticks, GamePadTriggers triggers, GamePadButtonsSet buttons)
        {
            Connected = true;
            ThumbSticks = thumbSticks;
            Triggers = triggers;
            Buttons = buttons;
        }
    }
}
