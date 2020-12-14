// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Input
{
    [Flags]
    public enum GamePadButton
    {
        None = 0,
        DPadUp = 1 << 0,
        DPadDown = 1 << 1,
        DPadLeft = 1 << 2,
        DPadRight = 1 << 3,
        Menu = 1 << 4,
        View = 1 << 5,
        Back = 1 << 5,
        LeftStick = 1 << 6,
        RightStick = 1 << 7,
        LeftShoulder = 1 << 8,
        RightShoulder = 1 << 9,
        A = 1 << 10,
        B = 1 << 11,
        X = 1 << 12,
        Y = 1 << 13,  
        RightTrigger = 1 << 14,
        LeftTrigger = 1 << 15,
        LeftThumbstickLeft = 1 << 16,
        LeftThumbstickUp = 1 << 17,
        LeftThumbstickDown = 1 << 18,
        LeftThumbstickRight = 1 << 19,
        RightThumbstickUp = 1 << 20,
        RightThumbstickDown = 1 << 21,
        RightThumbstickRight = 1 << 22,
        RightThumbstickLeft = 1 << 23
    }
}
