using CrossX.Forms.Values;
using CrossX.Input;
using System;
using System.Collections.Generic;

namespace CrossX.Forms.Services
{
    internal class FormsInputService : IFormsInputMapping, IFormsInput
    {
        private class Mapping
        {
            public List<Key> Keys { get; } = new List<Key>();
            public List<GamePadButton> Buttons { get; } = new List<GamePadButton>();
        }

        private Dictionary<UiButton, Mapping> keyToButtonMapping = new Dictionary<UiButton, Mapping>();

        private readonly IKeyboard keyboard;
        private readonly IGamePads gamePads;

        public FormsInputService(IKeyboard keyboard, IGamePads gamePads)
        {
            this.keyboard = keyboard;
            this.gamePads = gamePads;

            foreach(var val in Enum.GetValues(typeof(UiButton)))
            {
                var id = (UiButton)val;
                keyToButtonMapping.Add(id, new Mapping());
            }
        }

        public KeyBtnState GetUiButtonState(UiButton button)
        {
            KeyBtnState state = KeyBtnState.Up;
            var mapping = keyToButtonMapping[button];

            for(var idx =0; idx < mapping.Keys.Count; ++idx)
            {
                var key = mapping.Keys[idx];
                state |= keyboard.GetKeyState(key);
            }

            for (var idx = 0; idx < mapping.Buttons.Count; ++idx)
            {
                var btn = mapping.Buttons[idx];
                state |= gamePads.GetState(0).GetButtonState(btn);
            }

            return state;
        }

        public void MapUiButton(UiButton button, Key key)
        {
            keyToButtonMapping[button].Keys.Add(key);
        }

        public void MapUiButton(UiButton button, GamePadButton gamePadButton)
        {
            keyToButtonMapping[button].Buttons.Add(gamePadButton);
        }
    }
}
