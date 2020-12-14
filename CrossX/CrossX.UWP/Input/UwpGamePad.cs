// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX;
using CrossX.Input;
using System.Numerics;
using Windows.Gaming.Input;

namespace CrossX.WindowsUniversal.Input
{
    internal class UwpGamePad: GamePadState
    {
        public static readonly GamePadState NotConnected = new UwpGamePad();

        const float TriggerPushValue = 0.9f;
        const float StickPushValue = 0.5f;

        public readonly Gamepad Gamepad;

        public UwpGamePad()
        {
            
        }

        public UwpGamePad(Gamepad pad)
        {
            Gamepad = pad;
            IsConnected = true;
        }

        public void Update()
        {
            var reading = Gamepad.GetCurrentReading();

            PreviousButtonsDown = ButtonsDown;
            ButtonsDown = ButtonsFromReading(ref reading);

            LeftThumbStick = new Vector2((float)reading.LeftThumbstickX, (float)reading.LeftThumbstickY);
            RightThumbStick = new Vector2((float)reading.RightThumbstickX, (float)reading.RightThumbstickY);

            LeftTrigger = (float)reading.LeftTrigger;
            RightTrigger = (float)reading.RightTrigger;
        }

        //public override void SetVibration(Vibration vibration)
        //{
        //    Gamepad.Vibration = new GamepadVibration
        //    {
        //        LeftMotor = vibration.LeftMotor,
        //        RightMotor = vibration.RightMotor,
        //        LeftTrigger = vibration.LeftTrigger,
        //        RightTrigger = vibration.RightTrigger
        //    };
        //}
        
        private GamePadButton ButtonsFromReading(ref GamepadReading reading)
        {
            GamePadButton buttons = 0;

            buttons |= reading.Buttons.HasFlag(GamepadButtons.A) ? GamePadButton.A : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.B) ? GamePadButton.B : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.X) ? GamePadButton.X : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.Y) ? GamePadButton.Y : 0;

            buttons |= reading.Buttons.HasFlag(GamepadButtons.DPadDown) ? GamePadButton.DPadDown : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.DPadUp) ? GamePadButton.DPadUp : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.DPadLeft) ? GamePadButton.DPadLeft : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.DPadRight) ? GamePadButton.DPadRight : 0;

            buttons |= reading.Buttons.HasFlag(GamepadButtons.Menu) ? GamePadButton.Menu : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.View) ? GamePadButton.View : 0;

            buttons |= reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick) ? GamePadButton.LeftStick : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.RightThumbstick) ? GamePadButton.RightStick : 0;

            buttons |= reading.Buttons.HasFlag(GamepadButtons.LeftShoulder) ? GamePadButton.LeftShoulder : 0;
            buttons |= reading.Buttons.HasFlag(GamepadButtons.RightShoulder) ? GamePadButton.RightShoulder : 0;

            buttons |= reading.LeftTrigger > TriggerPushValue ? GamePadButton.LeftTrigger : 0;
            buttons |= reading.RightTrigger > TriggerPushValue ? GamePadButton.RightTrigger : 0;

            buttons |= reading.LeftThumbstickX < -StickPushValue ? GamePadButton.LeftThumbstickLeft : 0;
            buttons |= reading.LeftThumbstickX > StickPushValue ? GamePadButton.LeftThumbstickRight : 0;
            buttons |= reading.LeftThumbstickY > StickPushValue ? GamePadButton.LeftThumbstickUp : 0;
            buttons |= reading.LeftThumbstickY < -StickPushValue ? GamePadButton.LeftThumbstickDown : 0;

            buttons |= reading.RightThumbstickX < -StickPushValue ? GamePadButton.RightThumbstickLeft : 0;
            buttons |= reading.RightThumbstickX > StickPushValue ? GamePadButton.RightThumbstickRight : 0;
            buttons |= reading.RightThumbstickY > StickPushValue ? GamePadButton.RightThumbstickUp : 0;
            buttons |= reading.RightThumbstickY < -StickPushValue ? GamePadButton.RightThumbstickDown : 0;

            return buttons;
        }
    }
}
