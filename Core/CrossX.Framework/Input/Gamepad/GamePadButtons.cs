using System;

namespace CrossX.Framework.Input.Gamepad
{
    [Flags]
    public enum GamePadButtons
    {
        A = 1 << 0,
        B = 1 << 1,
        X = 1 << 2,
        Y = 1 << 3,
        Menu = 1 << 4,
        Select = 1 << 5,
        DPadUp = 1 << 6,
        DPadDown = 1 << 7,
        DPadLeft = 1 << 8,
        DPadRight = 1 << 9,
        LeftShoulder = 1 << 10,
        RightShoulder = 1 << 11,
        LeftTrigger = 1 << 12,
        RightTrigger = 1 << 13,
        LeftThumbstick = 1 << 14,
        RightThumbstick = 1 << 15,
        LeftThumbstickUp = 1 << 16,
        LeftThumbstickDown = 1 << 17,
        LeftThumbstickLeft = 1 << 18,
        LeftThumbstickRight = 1 << 19
    }
}
