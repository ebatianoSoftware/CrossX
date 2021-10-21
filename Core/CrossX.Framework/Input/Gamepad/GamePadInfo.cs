using System.Numerics;

namespace CrossX.Framework.Input.Gamepad
{
    public abstract class GamePadInfo
    {
        public bool Connected { get; protected set; }
        private GamePadButtons previousButtons = 0;
        private GamePadButtons currentButtons = 0;
        private Vector2 leftStick;
        private Vector2 rightStick;
        private float leftTrigger;
        private float rightTrigger;

        protected void Update(GamePadButtons buttons, Vector2 leftStick, Vector2 rightStick, float leftTrigger, float rightTrigger)
        {
            previousButtons = currentButtons;
            currentButtons = buttons;

            this.leftStick = leftStick;
            this.rightStick = rightStick;

            this.leftTrigger = leftTrigger;
            this.rightTrigger = rightTrigger;
        }

        public GamepadState GetState()
        {
            if (!Connected) return GamepadState.NotConnected;

            return new GamepadState(
                new GamePadThumbSticks(leftStick, rightStick),
                new GamePadTriggers(leftTrigger, rightTrigger),
                new GamePadButtonsSet(previousButtons, currentButtons)
                );
        }
    }
}
