using CrossX.Framework.Graphics;
using CrossX.Framework.UI;
using System.Collections;

namespace CrossX.Framework.Services
{
    public interface IContextMenuService
    {
        void ShowContextMenu(IList items, View relatedTo, TextAlign align);
    }

    internal class ContextMenuService : IContextMenuService
    {
        public void ShowContextMenu(IList items, View relatedTo, TextAlign align)
        {
            
        }
    }
}
