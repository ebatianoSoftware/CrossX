using CrossX.Graphics2D;
using CrossX.IoC;

namespace CrossX.Forms.Controls
{
    public interface IControlParent
    {
        void InvalidateLayout();
        IFocusable Focus { get; set; }
    }
}
