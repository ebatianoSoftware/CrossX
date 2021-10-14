using CrossX.Framework.Input;
using OpenTK.Input;
using Key = OpenTK.Input.Key;
using XxKey = CrossX.Framework.Input.Key;


namespace CrossX.WindowsForms.Input
{
    internal class OpenTkKeyboard : IKeyboard
    {
        private KeyboardState previousState;
        private KeyboardState currentState;

        public bool IsKeyDown(XxKey key) => currentState.IsKeyDown((Key)key);

        public bool IsKeyJustPressed(XxKey key) => currentState.IsKeyDown((Key)key) && !previousState.IsKeyDown((Key)key);

        public bool IsKeyJustReleased(XxKey key) => !currentState.IsKeyDown((Key)key) && previousState.IsKeyDown((Key)key);
        
        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }
    }
}
