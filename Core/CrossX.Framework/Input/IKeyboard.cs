// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace CrossX.Framework.Input
{
    public interface IKeyboard
    {
        bool IsKeyDown(Key key);
        bool IsKeyJustPressed(Key key);
        bool IsKeyJustReleased(Key key);
    }
}
