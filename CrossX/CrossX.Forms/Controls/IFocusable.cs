using CrossX.Forms.Values;

namespace CrossX.Forms.Controls
{
    public interface IFocusable
    {
        bool OnUiButtonPressed(UiButton button);
        bool OnUiButtonReleased(UiButton button);
        bool OnCharacterInput(char character);
        bool IsVisible { get; }
        void OnFocusFromButton(UiButton uiButton);
    }
}
