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
        ITransitionsManager TransitionsManager { get; }
        ITransform2D Transform2D { get; }
        void InvalidateLayout();
        IFocusable Focus { get; set; }
    }
}
