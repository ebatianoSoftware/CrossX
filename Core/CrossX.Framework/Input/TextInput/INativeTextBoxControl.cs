namespace CrossX.Framework.Input.TextInput
{
    public interface INativeTextBoxControl
    {
        Rectangle Bounds { get; }
        string Text { get; set; }
        Color BackgroundColor { get; }
        Color TextColor { get; }
        Color SelectionColor { get; }
        void OnLostFocus();
        string FontFamily { get; }
        FontWeight FontWeight { get; }
        Length FontSize { get; }
        int MaxLength { get; }
    }
}
