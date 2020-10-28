// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace CrossX.Input
{
    public interface IKeyboard
    {
        KeyBtnState GetKeyState(Key key);
        IEnumerable<Key> GetKeysDown();
    }
}
