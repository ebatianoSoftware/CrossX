using CrossX.Framework.Input.Gamepad;
using OpenTK.Input;
using System.Numerics;
using FrameworkGamePadButtons = CrossX.Framework.Input.Gamepad.GamePadButtons;

namespace CrossX.WindowsForms.Input
{
    internal class OpenTkGamePadInfo: GamePadInfo
    {
        private readonly int index;

        public OpenTkGamePadInfo(int index)
        {
            this.index = index;
            Update();
        }

        public void Update()
        {
            var state = GamePad.GetState(index);
            Connected = state.IsConnected;

            if (!Connected) return;

            var leftStick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
            var rightStick = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);

            var leftTrigger = state.Triggers.Left;
            var rightTrigger = state.Triggers.Right;

            FrameworkGamePadButtons gpButtons = 0;

            var buttons = state.Buttons;

            if (buttons.A == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.A;
            if (buttons.B == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.B;
            if (buttons.X == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.X;
            if (buttons.Y == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.Y;

            if (buttons.Back == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.Select;
            if (buttons.Start == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.Menu;

            if (buttons.LeftShoulder == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.LeftShoulder;
            if (buttons.RightShoulder == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.RightShoulder;

            if (buttons.LeftStick == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.LeftThumbstick;
            if (buttons.RightStick == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.RightThumbstick;

            var dpad = state.DPad;

            if (dpad.Up == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.DPadUp;
            if (dpad.Down == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.DPadDown;
            if (dpad.Left == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.DPadLeft;
            if (dpad.Right == ButtonState.Pressed) gpButtons |= FrameworkGamePadButtons.DPadRight;

            if (leftTrigger > 0.5f) gpButtons |= FrameworkGamePadButtons.LeftTrigger;
            if (rightTrigger > 0.5f) gpButtons |= FrameworkGamePadButtons.RightTrigger;

            if (leftStick.X < -0.5f) gpButtons |= FrameworkGamePadButtons.LeftThumbstickLeft;
            if (leftStick.X > 0.5f) gpButtons |= FrameworkGamePadButtons.LeftThumbstickRight;

            if (leftStick.Y < -0.5f) gpButtons |= FrameworkGamePadButtons.LeftThumbstickDown;
            if (leftStick.Y > 0.5f) gpButtons |= FrameworkGamePadButtons.LeftThumbstickUp;

            Update(gpButtons, leftStick, rightStick, leftTrigger, rightTrigger);
        }
    }
}
