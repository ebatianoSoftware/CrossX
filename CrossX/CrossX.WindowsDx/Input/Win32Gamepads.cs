// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


using CrossX.Input;

namespace CrossX.Windows.Input
{
    internal class Win32Gamepads : IGamePads
    {
        private readonly Win32GamePadState[] gamePads = new Win32GamePadState[]
        {
            new Win32GamePadState(0),
            new Win32GamePadState(1),
            new Win32GamePadState(2),
            new Win32GamePadState(3)
        };

        public bool ShowVirtualGamePad => false;

        public double VibrationIntensity { get; set; } = 1;

        public void AttachVirtualGamePad(GamePadState gamePad)
        {
        }

        public void DetachVirtualGamePad(GamePadState gamePad)
        {   
        }

        public GamePadState GetState(int index)
        {
            return gamePads[index];
        }

        //public void SetVibration(int index, Vibration vibration)
        //{
        //    vibration.LeftMotor *= VibrationIntensity;
        //    vibration.RightMotor *= VibrationIntensity;
        //    vibration.LeftTrigger *= VibrationIntensity;
        //    vibration.RightTrigger *= VibrationIntensity;

        //    _gamePads[index].SetVibration(vibration);
        //}

        public void Update()
        {
            foreach(var gamepad in gamePads)
            {
                gamepad.Update();
            }
        }
    }
}
