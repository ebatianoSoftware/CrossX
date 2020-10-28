namespace CrossX.Input
{
    public abstract class GamePadState
    {
        public bool IsConnected { get; protected set; }
        public GamePadButton ButtonsDown { get; protected set; }

        public Vector2 LeftThumbStick { get; protected set; }

        public Vector2 RightThumbStick { get; protected set; }

        public double LeftTrigger { get; protected set; }

        public double RightTrigger { get; protected set; }

        protected GamePadButton PreviousButtonsDown { get; set; }

        public KeyBtnState GetButtonState(GamePadButton button)
        {
            KeyBtnState state = 0;

            if ((ButtonsDown & button) == button)
            {
                state |= KeyBtnState.Down;
            }

            if ((ButtonsDown & button) != (PreviousButtonsDown & button))
            {
                state |= KeyBtnState.JustSwitched;
            }
            return state;
        }
    }
}
