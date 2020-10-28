// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Input
{
    [Flags]
    public enum KeyBtnState
    {
        /// <summary>
        /// The button is up.
        /// </summary>
        Up = 0x00,
        /// <summary>
        /// The button is down.
        /// </summary>
        Down = 0x01,
        /// <summary>
        /// The button has just been pressed down.
        /// </summary>
        JustPressed = Down |JustSwitched,
        /// <summary>
        /// The button has just been released.
        /// </summary>
        JustReleased = Up | JustSwitched,
        /// <summary>
        /// The button has just changed its up/down state.
        /// </summary>
        JustSwitched = 0x10
    }
}
