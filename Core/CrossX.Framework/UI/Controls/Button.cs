using CrossX.Framework.Graphics;
using System.Windows.Input;

namespace CrossX.Framework.UI.Controls
{
    public class Button : TextBasedControl
    {
        private ICommand command;
        private object commandParameter;

        public ICommand Command { get => command; set => SetProperty(ref command, value); }
        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }

        public Button(IFontManager fontManager) : base(fontManager)
        {
        }
    }
}
