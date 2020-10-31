// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Input;
using CrossX.Input.Abstractions;
using Windows.UI.Core;

namespace CrossX.WindowsUniversal.Input
{
    internal class UwpKeyboard : KeyboardBase
    {
        private CoreWindow coreWindow; 

        public UwpKeyboard(CoreWindow coreWindow)
        {
            this.coreWindow = coreWindow;
            this.coreWindow.KeyDown += (o,args)=> OnKeyDown((Key)args.VirtualKey);
            this.coreWindow.KeyUp += (o,args) => OnKeyUp((Key)args.VirtualKey);
        }

        protected override bool IsKeyDown(Key key)
        {
            return coreWindow.GetKeyState((Windows.System.VirtualKey)key).HasFlag(CoreVirtualKeyStates.Down);
        }
    }
}
