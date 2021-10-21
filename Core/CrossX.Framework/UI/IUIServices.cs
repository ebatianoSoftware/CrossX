using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.Services;

namespace CrossX.Framework.UI
{
    public interface IUIServices
    {
        IBindingService BindingService { get; }
        IFontManager FontManager { get; }
        IDispatcher Dispatcher { get; }
        IImageCache ImageCache { get; }
        IAppValues AppValues { get; }
        IObjectFactory ObjectFactory { get; }
        ITooltipService TooltipService { get; }
    }
}
