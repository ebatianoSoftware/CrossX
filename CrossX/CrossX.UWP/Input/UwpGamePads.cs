// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Input;
using System.Collections.Generic;
using System.Linq;
using Windows.Gaming.Input;

namespace CrossX.WindowsUniversal.Input
{
    internal class UwpGamePads : IGamePads
    {
        private readonly List<UwpGamePad> _gamePads = new List<UwpGamePad>();

        public bool ShowVirtualGamePad => _gamePads.Count == 0 && _isTouchEnabled;
        private bool _isTouchEnabled;

        public UwpGamePads()
        {
            _isTouchEnabled = Windows.Devices.Input
              .PointerDevice.GetPointerDevices()
              .Any(p => p.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch);

            foreach (var pad in Gamepad.Gamepads)
            {
                AddPad(this, pad);
            }
            Gamepad.GamepadAdded += AddPad;
            Gamepad.GamepadRemoved += RemovePad;
        }

        public void Update()
        {
            foreach (var gp in _gamePads) gp?.Update();
        }

        public GamePadState GetState(int index)
        {
            if (index >= _gamePads.Count) return UwpGamePad.NotConnected;
            return _gamePads[index] ?? UwpGamePad.NotConnected;
        }

        private void AddPad(object sender, Gamepad pad)
        {
            var index = _gamePads.FindIndex(p => p == null);
            if(index >=0)
            {
                _gamePads[index] = new UwpGamePad(pad);
            }
            else
            {
                _gamePads.Add(new UwpGamePad(pad));
            }
        }

        private void RemovePad(object sender, Gamepad pad)
        {
            var index = _gamePads.FindIndex(p => p.Gamepad == pad);
            if (index < 0) return;
            _gamePads[index] = null;
        }

        //public void SetVibration(int index, Vibration vibration)
        //{
        //    if (index >= _gamePads.Count) return;
        //    _gamePads[index]?.SetVibration(vibration);
        //}

        //public void AttachVirtualGamePad(IGamePadState gamePad)
        //{
        //}

        //public void DetachVirtualGamePad(IGamePadState gamePad)
        //{   
        //}
    }
}
