// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Input
{
    public delegate void MouseEvent(Vector2 position, MouseButtons buttons);

    public interface IMouse
    {
        Vector2 Position { get; set; }

        KeyBtnState GetButtonState(MouseButtons button);

        float WheelDelta { get; }

        MouseCaps Caps { get; }

        event MouseEvent ButtonDown;
        event MouseEvent ButtonUp;

        event MouseEvent MouseMove;
        event MouseEvent MouseEnter;
        event Action MouseLeave;
        CursorType Cursor { get; set; }
    }
}
