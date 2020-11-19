using CrossX.Forms.Values;
using CrossX.Input;

namespace CrossX.Forms.Helpers
{
    public static class DefaultSettings
    {
        public static void SetDefaultMapping(this IFormsInputMapping mapping)
        {
            mapping.MapUiButton(UiButton.Select, GamePadButton.A);
            mapping.MapUiButton(UiButton.Back, GamePadButton.B);
            mapping.MapUiButton(UiButton.Menu, GamePadButton.Menu);

            mapping.MapUiButton(UiButton.Down, GamePadButton.DPadDown);
            mapping.MapUiButton(UiButton.Down, GamePadButton.LeftThumbstickDown);

            mapping.MapUiButton(UiButton.Up, GamePadButton.DPadUp);
            mapping.MapUiButton(UiButton.Up, GamePadButton.LeftThumbstickUp);

            mapping.MapUiButton(UiButton.Left, GamePadButton.DPadLeft);
            mapping.MapUiButton(UiButton.Left, GamePadButton.LeftThumbstickLeft);

            mapping.MapUiButton(UiButton.Right, GamePadButton.DPadRight);
            mapping.MapUiButton(UiButton.Right, GamePadButton.LeftThumbstickRight);

            mapping.MapUiButton(UiButton.Select, Key.Enter);
            mapping.MapUiButton(UiButton.MenuAndBack, Key.Escape);
            mapping.MapUiButton(UiButton.Down, Key.Down);
            mapping.MapUiButton(UiButton.Up, Key.Up);
            mapping.MapUiButton(UiButton.Left, Key.Left);
            mapping.MapUiButton(UiButton.Right, Key.Right);
        }
    }
}
