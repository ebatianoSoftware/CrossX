using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using System.Windows.Input;

namespace CrossX.Framework.UI.Controls
{
    public class Button : TextBasedControl
    {
        private ICommand command;
        private object commandParameter;
        private Drawable backgroundDrawable;

        private Color backgroundColorPushed = Color.Transparent;
        private Color backgroundColorOver = Color.Transparent;

        private Color foregroundColorPushed = Color.Black;
        private Color foregroundColorOver = Color.Black;

        public ICommand Command { get => command; set => SetProperty(ref command, value); }
        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }
        public Drawable BackgroundDrawable { get => backgroundDrawable; set => SetProperty(ref backgroundDrawable, value); }

        public Color BackgroundColorPushed { get => backgroundColorPushed; set => SetProperty(ref backgroundColorPushed, value); }
        public Color BackgroundColorOver { get => backgroundColorOver; set => SetProperty(ref backgroundColorOver, value); }

        public Color ForegroundColorPushed { get => foregroundColorPushed; set => SetProperty(ref foregroundColorPushed, value); }
        public Color ForegroundColorOver { get => foregroundColorOver; set => SetProperty(ref foregroundColorOver, value); }

        public Button(IFontManager fontManager) : base(fontManager)
        {

        }
    }
}
