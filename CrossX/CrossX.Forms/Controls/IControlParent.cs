using CrossX.Graphics2D;

namespace CrossX.Forms.Controls
{
    public interface IControlParent
    {
        SpriteBatch SpriteBatch { get; }
        PrimitiveBatch PrimitiveBatch { get; }
        IControlsLoader ControlsLoader { get; }
        void InvalidateLayout();
    }
}
