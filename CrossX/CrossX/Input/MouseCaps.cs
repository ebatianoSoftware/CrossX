// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;

namespace CrossX.Input
{
    /// <summary>
    /// Defines capabilities of a mouse.
    /// </summary>
    [Flags]
    public enum MouseCaps
    {
        LButton = 1,
        RButton = 2,
        MButton = 4,
        Wheel = 8
    }
}
