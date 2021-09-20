namespace CrossX.Framework.Core
{
    public interface INativeWindow
    {
        Size MinSize { set; }
        Size MaxSize { set; }
        Size Size { set; }
        bool CanResize { set; }
        bool CanMaximize { set; }
    }
}
