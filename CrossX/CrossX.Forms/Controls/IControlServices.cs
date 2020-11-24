using CrossX.Graphics2D;
using CrossX.Input;
using CrossX.IoC;

namespace CrossX.Forms.Controls
{
    public interface IControlServices
    {
        SpriteBatch SpriteBatch { get; }
        PrimitiveBatch PrimitiveBatch { get; }
        IControlsLoader ControlsLoader { get; }
        IObjectFactory ObjectFactory { get; }
        ITransitionsManager TransitionsManager { get; }
        ITransform2D Transform2D { get; }
        IFormsSounds Sounds { get; }
        CursorType CursorType { set; }
        IMouse Mouse { get; }
    }
}
