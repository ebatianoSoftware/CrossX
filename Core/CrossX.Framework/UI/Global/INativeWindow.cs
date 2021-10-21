using CrossX.Framework.Input.TextInput;
using System.Numerics;
using System.Threading.Tasks;

namespace CrossX.Framework.UI.Global
{
    public interface INativeWindow
    {
        Task<INativeTextBox> CreateNativeTextBox(INativeTextBoxControl control);
        void Close();
        RectangleF Bounds { get; }
    }
}
