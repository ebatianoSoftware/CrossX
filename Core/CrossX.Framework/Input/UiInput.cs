using CrossX.Abstractions.Input;
using CrossX.Framework.Input.Gamepad;

namespace CrossX.Framework.Input
{
    public class UiInput : IUiInput
    {
        private readonly IGamePads gamePads;
        private readonly IKeyboard keyboard;

        public UiInput(IGamePads gamePads, IKeyboard keyboard)
        {
            this.gamePads = gamePads;
            this.keyboard = keyboard;
        }

        public bool IsDown(UiInputKey inputKey)
        {
            var state = gamePads.GetState(0);

            switch (inputKey)
            {
                case UiInputKey.Up:
                    return keyboard.IsKeyDown(Key.Up) || state.Buttons.Down.HasFlag(GamePadButtons.DPadUp) || state.Buttons.Down.HasFlag(GamePadButtons.LeftThumbstickUp);

                case UiInputKey.Down:
                    return keyboard.IsKeyDown(Key.Down) || state.Buttons.Down.HasFlag(GamePadButtons.DPadDown) || state.Buttons.Down.HasFlag(GamePadButtons.LeftThumbstickDown);

                case UiInputKey.Left:
                    return keyboard.IsKeyDown(Key.Left) || state.Buttons.Down.HasFlag(GamePadButtons.DPadLeft) || state.Buttons.Down.HasFlag(GamePadButtons.LeftThumbstickLeft);

                case UiInputKey.Right:
                    return keyboard.IsKeyDown(Key.Right) || state.Buttons.Down.HasFlag(GamePadButtons.DPadRight) || state.Buttons.Down.HasFlag(GamePadButtons.LeftThumbstickRight);

                case UiInputKey.Select:
                    return keyboard.IsKeyDown(Key.Enter) || state.Buttons.Down.HasFlag(GamePadButtons.A);

                case UiInputKey.Back:
                    return state.Buttons.Down.HasFlag(GamePadButtons.B);

                case UiInputKey.MenuOrBack:
                    return keyboard.IsKeyDown(Key.Escape) || state.Buttons.Down.HasFlag(GamePadButtons.B);

                case UiInputKey.Menu:
                    return state.Buttons.Down.HasFlag(GamePadButtons.Menu);
            }
            return false;
        }

        public bool IsJustPressed(UiInputKey inputKey)
        {
            var state = gamePads.GetState(0);

            switch (inputKey)
            {
                case UiInputKey.Up:
                    return keyboard.IsKeyJustPressed(Key.Up) || state.Buttons.JustPressed.HasFlag(GamePadButtons.DPadUp) || state.Buttons.JustPressed.HasFlag(GamePadButtons.LeftThumbstickUp);

                case UiInputKey.Down:
                    return keyboard.IsKeyJustPressed(Key.Down) || state.Buttons.JustPressed.HasFlag(GamePadButtons.DPadDown) || state.Buttons.JustPressed.HasFlag(GamePadButtons.LeftThumbstickDown);

                case UiInputKey.Left:
                    return keyboard.IsKeyJustPressed(Key.Left) || state.Buttons.JustPressed.HasFlag(GamePadButtons.DPadLeft) || state.Buttons.JustPressed.HasFlag(GamePadButtons.LeftThumbstickLeft);

                case UiInputKey.Right:
                    return keyboard.IsKeyJustPressed(Key.Right) || state.Buttons.JustPressed.HasFlag(GamePadButtons.DPadRight) || state.Buttons.JustPressed.HasFlag(GamePadButtons.LeftThumbstickRight);

                case UiInputKey.Select:
                    return keyboard.IsKeyJustPressed(Key.Enter) || state.Buttons.JustPressed.HasFlag(GamePadButtons.A);

                case UiInputKey.Back:
                    return state.Buttons.JustPressed.HasFlag(GamePadButtons.B);

                case UiInputKey.MenuOrBack:
                    return keyboard.IsKeyJustPressed(Key.Escape) || state.Buttons.JustPressed.HasFlag(GamePadButtons.B);

                case UiInputKey.Menu:
                    return state.Buttons.JustPressed.HasFlag(GamePadButtons.Menu);
            }
            return false;
        }
    }
}
