namespace CrossX.Framework.Input
{
    public interface IUiInput
    {
        bool IsJustPressed(UiInputKey inputKey);
        bool IsDown(UiInputKey inputKey);
    }
}
