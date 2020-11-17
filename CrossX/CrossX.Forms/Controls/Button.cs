using CrossX.Graphics2D.Text;

namespace CrossX.Forms.Controls
{
    public class Button : Panel
    {
        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        public bool IsDown { get => isDown; private set => SetProperty(ref isDown, value); }
        public bool IsFocused { get => isFocused; private set => SetProperty(ref isFocused, value); }

        private TextSource text;
        private bool isDown;
        private bool isFocused;

        public Button(IControlParent parent) : base(parent)
        {
        }
    }
}
