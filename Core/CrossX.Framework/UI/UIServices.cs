using CrossX.Abstractions.Async;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI
{
    public class UIServices : IUIServices
    {
        public IRedrawService RedrawService { get; }
        public IBindingService BindingService { get; }
        public IFontManager FontManager { get; }
        public IDispatcher Dispatcher { get; }
        public IImageCache ImageCache { get; }
        public IAppValues AppValues { get; }

        public UIServices(IRedrawService redrawService, IBindingService bindingService, IFontManager fontManager, IDispatcher dispatcher, IImageCache imageCache, IAppValues appValues)
        {
            RedrawService = redrawService;
            BindingService = bindingService;
            FontManager = fontManager;
            Dispatcher = dispatcher;
            ImageCache = imageCache;
            AppValues = appValues;
        }
    }
}
