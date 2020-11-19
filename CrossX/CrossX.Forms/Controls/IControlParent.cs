using CrossX.Graphics2D;
using CrossX.IoC;

namespace CrossX.Forms.Controls
{
    public interface IControlParent
    {
        SpriteBatch SpriteBatch { get; }
        PrimitiveBatch PrimitiveBatch { get; }
        IControlsLoader ControlsLoader { get; }
        IObjectFactory ObjectFactory { get; }
        void InvalidateLayout();
        IFocusable Focus { get; set; }
    }
}
