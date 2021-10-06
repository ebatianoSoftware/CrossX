using CrossX.Framework.Input;
using CrossX.Framework.UI.Global;

namespace CrossX.Framework.Core
{
    public interface INativeWindow
    {
        Size MinSize { set; }
        Size MaxSize { set; }
        Size Size { set; }
        bool CanResize { set; }
        bool CanMaximize { set; }
        WindowState State { get; set; }
        string Title { set; }
        CursorType Cursor { set; }
    }
}
