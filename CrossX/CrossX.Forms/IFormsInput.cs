using CrossX.Forms.Values;
using CrossX.Input;

namespace CrossX.Forms
{
    public interface IFormsInput
    {
        KeyBtnState GetUiButtonState(UiButton button);
    }
}
