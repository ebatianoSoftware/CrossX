// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Input;
using CrossX;
using SharpDX.XInput;

namespace CrossX.Windows.Input
{
    internal class Win32GamePadState : GamePadState
    {
        const double TriggerPushValue = 0.9f;
        const double StickPushValue = 0.5f;

        private readonly Controller controller;
        public Win32GamePadState(int index)
        {
            controller = new Controller((UserIndex)index);
        }

        public void Update()
        {
            IsConnected = controller.IsConnected;
            if (!controller.IsConnected)
            {
                PreviousButtonsDown = 0;
                ButtonsDown = 0;
                LeftThumbStick = Vector2.Zero;
                RightThumbStick = Vector2.Zero;
                LeftTrigger = 0;
                RightTrigger = 0;
                return;
            }

            var state = controller.GetState();

            int move = (int)(GamepadButtonFlags.A | GamepadButtonFlags.B | GamepadButtonFlags.X | GamepadButtonFlags.Y);
            int abxy = (int)state.Gamepad.Buttons & move;
            abxy >>= 2;

            var buttons = (int)state.Gamepad.Buttons & (~move);
            buttons |= abxy;

            var xxButtons = (GamePadButton)buttons;

            LeftThumbStick = new Vector2( (float)state.Gamepad.LeftThumbX / short.MaxValue, (float)state.Gamepad.LeftThumbY / short.MaxValue);
            RightThumbStick = new Vector2((float)state.Gamepad.RightThumbX / short.MaxValue, (float)state.Gamepad.RightThumbY / short.MaxValue);

            LeftTrigger = (float)state.Gamepad.LeftTrigger / byte.MaxValue;
            RightTrigger = (float)state.Gamepad.RightTrigger / byte.MaxValue;


            xxButtons |= LeftTrigger > TriggerPushValue ? GamePadButton.LeftTrigger : 0;
            xxButtons |= RightTrigger > TriggerPushValue ? GamePadButton.RightTrigger : 0;

            xxButtons |= LeftThumbStick.X < -StickPushValue ? GamePadButton.LeftThumbstickLeft : 0;
            xxButtons |= LeftThumbStick.X > StickPushValue ? GamePadButton.LeftThumbstickRight : 0;
            xxButtons |= LeftThumbStick.Y > StickPushValue ? GamePadButton.LeftThumbstickUp : 0;
            xxButtons |= LeftThumbStick.Y < -StickPushValue ? GamePadButton.LeftThumbstickDown : 0;

            xxButtons |= RightThumbStick.X < -StickPushValue ? GamePadButton.RightThumbstickLeft : 0;
            xxButtons |= RightThumbStick.X > StickPushValue ? GamePadButton.RightThumbstickRight : 0;
            xxButtons |= RightThumbStick.Y > StickPushValue ? GamePadButton.RightThumbstickUp : 0;
            xxButtons |= RightThumbStick.Y < -StickPushValue ? GamePadButton.RightThumbstickDown : 0;

            PreviousButtonsDown = ButtonsDown;
            ButtonsDown = xxButtons;
        }

        //public override void SetVibration(Vibration vibration)
        //{
        //    if (!IsConnected) return;

        //    var dxVibration = new SharpDX.XInput.Vibration
        //    {
        //        LeftMotorSpeed = (ushort)(vibration.LeftMotor * ushort.MaxValue),
        //        RightMotorSpeed = (ushort)(vibration.RightMotor * ushort.MaxValue),
        //    };
        //    controller.SetVibration(dxVibration);
        //}
    }
}
