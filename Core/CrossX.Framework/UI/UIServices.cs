using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.Services;

namespace CrossX.Framework.UI
{
    public class UIServices : IUIServices
    {
        public IBindingService BindingService { get; }
        public IFontManager FontManager { get; }
        public IDispatcher Dispatcher { get; }
        public IImageCache ImageCache { get; }
        public IAppValues AppValues { get; }
        public IObjectFactory ObjectFactory { get; }
        public ITooltipService TooltipService { get; }

        public UIServices(IBindingService bindingService, IFontManager fontManager, 
            IDispatcher dispatcher, IImageCache imageCache, IAppValues appValues, 
            IObjectFactory objectFactory, ITooltipService tooltipService)
        {
            BindingService = bindingService;
            FontManager = fontManager;
            Dispatcher = dispatcher;
            ImageCache = imageCache;
            AppValues = appValues;
            ObjectFactory = objectFactory;
            TooltipService = tooltipService;
        }
    }
}
