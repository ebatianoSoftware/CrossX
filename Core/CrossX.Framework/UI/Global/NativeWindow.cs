using CrossX.Framework.Input.TextInput;
using System.Threading.Tasks;

namespace CrossX.Framework.UI.Global
{
    public class NativeWindow : INativeWindow
    {
        public Window Window { get; }
        public RectangleF Bounds 
        { 
            get
            {
                var center = parentWindow.ScreenBounds.Center;
                return new RectangleF(center.X - Window.Size.Width / 2, center.Y - Window.Size.Height / 2, Window.Size.Width, Window.Size.Height);
            }
        }

        private Window parentWindow;

        public NativeWindow(Window window, Window parentWindow)
        {
            this.parentWindow = parentWindow;
            parentWindow.AddPopup(this);

            Window = window;
            Window.Size = new SizeF(Window.Desktop_InitialWidth.Calculate(), Window.Desktop_InitialHeight.Calculate());
            Window.NativeWindow = this;
        }

        public void Close()
        {
            parentWindow.RemovePopup(this);
        }

        public Task<INativeTextBox> CreateNativeTextBox(INativeTextBoxControl control) => parentWindow.NativeWindow.CreateNativeTextBox(control);
    }
}
