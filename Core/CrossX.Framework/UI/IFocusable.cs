using CrossX.Abstractions.Input;

namespace CrossX.Framework.UI
{
    public interface IFocusable
    {
        RectangleF ScreenBounds { get; }
        bool HandleUiKey(UiInputKey key);
        bool ResignFocus();
    }
}
