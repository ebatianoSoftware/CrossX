using CrossX.Graphics2D;
using XxIoC;

namespace CrossX.Forms.Controls
{
    public interface IControlParent
    {
        void InvalidateLayout();
        IFocusable Focus { get; set; }
    }
}
