using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.Abstractions.Menu
{
    public class MenuGroup: MenuItemBase, ITitleContainer, IItemsContainer
    {
        public string Title { get; }
        public IList Items { get; }

        public MenuGroup(string title, IEnumerable<MenuItemBase> items)
        {
            Title = title;
            Items = items.ToArray();
        }
    }
}
