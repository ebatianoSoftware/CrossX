namespace CrossX.Framework.Input.TextInput
{
    public interface INativeTextBox
    {
        string Text { get; set; }
        void Release();
    }
}
