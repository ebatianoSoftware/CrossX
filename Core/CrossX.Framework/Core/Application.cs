using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework.ApplicationDefinition;
using CrossX.Framework.Binding;
using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;
using CrossX.Framework.Services;
using CrossX.Framework.UI;
using CrossX.Framework.XxTools;
using System;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.Core
{
    public delegate void InitServicesDelegate(IServicesProvider systemServices, IScopeBuilder scopeBuilder);

    public abstract class Application: ICoreApplication
    {
        protected IServicesProvider Services { get; private set; }
        protected IWindowsService WindowsService { get; private set; }

        public event InitServicesDelegate BeforeInitServices;
        public event InitServicesDelegate AfterInitServices;

        IServicesProvider ICoreApplication.Initialize(IServicesProvider servicesProvider)
        {
            LoadFonts(servicesProvider.GetService<IFontManager>());

            var assembly = GetType().Assembly;
            var builder = new ScopeBuilder(servicesProvider);
            var elementTypeMapping = new ElementTypeMapping(assembly);
            var appValues = new AppValues();
            var conversionService = new ConversionService(assembly);
            var bindingService = new BindingService(appValues, conversionService);

            LoadApplicationDefinition(appValues, bindingService, elementTypeMapping, servicesProvider.GetService<IObjectFactory>());

            builder
                .WithInstance(this).As<Application>().As(GetType())
                .WithInstance(elementTypeMapping).As<IElementTypeMapping>()
                .WithInstance(appValues).As<IAppValues>()
                .WithInstance(bindingService).As<IBindingService>()
                .WithType<TooltipService>().As<ITooltipService>().AsSingleton()
                .WithType<UIServices>().As<IUIServices>().AsSingleton()
                .WithType<XxFileParserImpl>().As<IXxFileParser>().AsSingleton();

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);

            if (!builder.TryResolveInstance<IViewLocator>(out var _))
            {
                builder.WithType<DefaultViewLocator>().As<IViewLocator>().AsSingleton();
            }

            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();
            WindowsService = Services.GetService<IWindowsService>();

            return Services;
        }

        protected virtual void LoadFonts(IFontManager fontManager)
        {
            using(var stream = typeof(Application).Assembly.GetManifestResourceStream("CrossX.Framework.Styles.Fonts.FluentSystemIcons-Filled.ttf"))
            {
                fontManager.LoadTTF(stream);
            }
        }

        private void LoadApplicationDefinition(IAppValues appValues, IBindingService bindingService, IElementTypeMapping elementTypeMapping, IObjectFactory objectFactory)
        {
            try
            {
                XxElement element;
                using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Assembly.GetName().Name + ".App.xml"))
                {
                    var parser = objectFactory.Create<XxFileParser>(elementTypeMapping);
                    element = parser.Parse(stream);
                }

                var scopeBuilder = objectFactory.Create<IScopeBuilder>();

                scopeBuilder
                    .WithInstance(elementTypeMapping).As<IElementTypeMapping>()
                    .WithInstance(appValues).As<IAppValues>()
                    .WithInstance(bindingService).As<IBindingService>();

                var services = scopeBuilder.Build();
                var defObjectFactory = services.GetService<IObjectFactory>().Create<XxDefinitionObjectFactory>();

                defObjectFactory.CreateObject<ApplicationElement>(element);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        void ICoreApplication.Load()
        {
            StartApp();
        }

        protected abstract void StartApp();

        protected virtual void InitServices(IServicesProvider systemServices, IScopeBuilder scopeBuilder)
        {

        }

        public void Dispose() { }
    }
}
