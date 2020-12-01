// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Input;
using CrossX.Input.Abstractions;
using System.Windows.Forms;

namespace CrossX.Windows.Input
{
    internal class Win32Keyboard : KeyboardBase
    {
        private readonly Form _form;

        public Win32Keyboard(Form form)
        {
            _form = form;
            _form.KeyDown += (o, args) => OnKeyDown((Key)args.KeyCode);
            _form.KeyUp += (o, args) => OnKeyUp((Key)args.KeyCode);
        }

        protected override bool IsKeyDown(Key key)
        {
            Keys keys = Control.ModifierKeys;

            if (!keys.HasFlag(Keys.Shift)) return false;
            if (!keys.HasFlag(Keys.Control)) return false;
            if (!keys.HasFlag(Keys.Alt)) return false;

            return false;
        }
    }
}
