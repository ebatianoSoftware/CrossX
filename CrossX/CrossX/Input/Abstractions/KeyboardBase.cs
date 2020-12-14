// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace CrossX.Input.Abstractions
{
    public abstract class KeyboardBase : IKeyboard
    {
        private HashSet<Key> keysDown = new HashSet<Key>();
        private HashSet<Key> previousKeysDown = new HashSet<Key>();

        protected void OnKeyUp(Key key)
        {
            keysDown.Remove(key);
        }

        protected void OnKeyDown(Key key)
        {
            keysDown.Add(key);
        }

        public IEnumerable<Key> GetKeysDown()
        {
            return keysDown;
        }

        public KeyBtnState GetKeyState(Key key)
        {
            bool isDown = keysDown.Contains(key);
            bool wasDown = previousKeysDown.Contains(key);

            var keyState = isDown ? KeyBtnState.Down : KeyBtnState.Up;

            if (isDown != wasDown)
            {
                keyState |= KeyBtnState.JustSwitched;
            }
            return keyState;
        }

        public void Update()
        {
            previousKeysDown.Clear();
            foreach (var key in keysDown)
            {
                previousKeysDown.Add(key);
            }

            UpdateForExtra(Key.LeftControl);
            UpdateForExtra(Key.RightControl);

            UpdateForExtra(Key.LeftShift);
            UpdateForExtra(Key.RightShift);

            UpdateForExtra(Key.LeftAlt);
            UpdateForExtra(Key.RightAlt);
        }

        protected abstract bool IsKeyDown(Key key);

        private void UpdateForExtra(Key key)
        {
            var down = IsKeyDown(key);
            var has = keysDown.Contains(key);

            if (down && !has)
            {
                keysDown.Add(key);
            }

            if (!down && has)
            {
                keysDown.Remove(key);
            }
        }
    }
}
