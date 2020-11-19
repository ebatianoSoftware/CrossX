using CrossX.Forms.Values;
using CrossX.Input;

namespace CrossX.Forms
{
    public interface IFormsInputMapping
    {
        void MapUiButton(UiButton button, Key key);
        void MapUiButton(UiButton button, GamePadButton gamePadButton);
    }
}
