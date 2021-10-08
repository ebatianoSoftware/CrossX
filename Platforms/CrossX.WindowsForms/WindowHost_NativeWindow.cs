using CrossX.Framework;
using CrossX.Framework.Input.TextInput;
using CrossX.Framework.UI.Global;
using CrossX.WindowsForms.Input;
using System.Numerics;
using System.Threading.Tasks;

namespace CrossX.WindowsForms
{
    public partial class WindowHost: INativeWindow
    {
        void INativeWindow.Close()
        {
            Close();
            DestroyHandle();
            Dispose();
        }

        Task<INativeTextBox> INativeWindow.CreateNativeTextBox(INativeTextBoxControl control, Vector2 position)
        {
            position *= UiUnit.PixelsPerUnit;

            var nativeTextBox = new NativeTextBox(this, control, new System.Drawing.Point((int)position.X, (int)position.Y));
            return Task.FromResult<INativeTextBox>(nativeTextBox);
        }
    }
}
