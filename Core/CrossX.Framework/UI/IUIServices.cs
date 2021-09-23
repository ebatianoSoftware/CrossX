using CrossX.Abstractions.Async;
using CrossX.Framework.Binding;
using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI
{
    public interface IUIServices
    {
        IRedrawService RedrawService { get; }
        IBindingService BindingService { get; }
        IFontManager FontManager { get; }
        IDispatcher Dispatcher { get; }
        IImageCache ImageCache { get; }
    }
}
