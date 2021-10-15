using CrossX.Framework.Input;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using Key = OpenTK.Input.Key;
using XxKey = CrossX.Framework.Input.Key;


namespace CrossX.WindowsForms.Input
{
    internal class OpenTkKeyboard : IKeyboard
    {
        private KeyboardState previousState;
        private KeyboardState currentState;

        private Dictionary<XxKey, Key> mapping = new Dictionary<XxKey, Key>();

        public bool IsKeyDown(XxKey key) => currentState.IsKeyDown(FromXxKey(key));

        public bool IsKeyJustPressed(XxKey key)
        {
            var tkKey = FromXxKey(key);
            return currentState.IsKeyDown(tkKey) && !previousState.IsKeyDown(tkKey);
        }

        public bool IsKeyJustReleased(XxKey key)
        {
            var tkKey = FromXxKey(key);
            return !currentState.IsKeyDown(tkKey) && previousState.IsKeyDown(tkKey);
        }

        public OpenTkKeyboard()
        {
            foreach(var name in Enum.GetNames(typeof(XxKey)))
            {
                XxKey key = Enum.Parse<XxKey>(name);

                if(Enum.TryParse<Key>(name, out var value))
                {
                    mapping.Add(key, value);
                }
            }
        }

        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }

        private Key FromXxKey(XxKey key)
        {
            if (mapping.TryGetValue(key, out var value)) return value;
            return Key.Unknown;
        }
    }
}
