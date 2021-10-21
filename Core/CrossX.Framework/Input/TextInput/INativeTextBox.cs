namespace CrossX.Framework.Input.TextInput
{
    public interface INativeTextBox
    {
        string Text { get; set; }
        void Release();
        
        (int start, int length) Selection { get; set; }

        void Focus();
    }
}
