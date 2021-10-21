using CrossX.Framework.UI.Global;
using System.Collections.Generic;

namespace CrossX.Framework.UI
{
    public interface IViewParent
    {
        void InvalidateLayout();
        RectangleF ScreenBounds { get; }
        Window Window { get; }
        bool DisplayVisible { get; }
    }
}
