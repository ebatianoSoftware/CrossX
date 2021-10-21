using System.Windows.Input;

namespace CrossX.Abstractions.Menu
{
    public class MenuItem: MenuItemBase, ICommandContainer, ITitleContainer, IFontIconContainer
    {
        public string Title { get; }
        public ICommand Command { get; }

        public (string fontFamily, string iconText) Icon { get; }

        public MenuItem(string title, ICommand command, string iconFontFamily = null, string iconText = null)
        {
            Title = title;
            Command = command;
            Icon = (iconFontFamily, iconText);
        }
    }
}
