using CrossX.Audio;
using CrossX.Content;
using CrossX.Content.Loaders;
using CrossX.Core;
using CrossX.Forms.Controls;
using CrossX.Forms.Converters;
using CrossX.Forms.Converters.Arythmetic;
using CrossX.Forms.Services;
using CrossX.Forms.Styles;
using CrossX.Forms.Transitions;
using CrossX.Forms.UiHosts;
using CrossX.Forms.Values;
using CrossX.Forms.Views;
using CrossX.Graphics;
using CrossX.Graphics2D.Text;
using XxIoC;
using System;
using System.Text;

namespace CrossX.Forms
{
    public class FormsRunner<TServicesInitializer, TFormsStartup> : IApp where TFormsStartup: class, IFormsAppContext where TServicesInitializer: class, IServicesInitializer
    {
        private readonly ScopeBuilder scopeBuilder;
        private readonly IGraphicsDevice graphicsDevice;
        private IObjectFactory objectFactory;
        private IServicesProvider servicesProvider;
        private NavigationView navigationView;
        private Application application;
        private IUiHost uiHost;

        public FormsRunner(IServicesProvider servicesProvider, IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            scopeBuilder = new ScopeBuilder().WithParent(servicesProvider);
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.servicesProvider = servicesProvider;
        }

        public void LoadContent()
        {
            var servicesInitializer = objectFactory.Create<TServicesInitializer>();
            servicesInitializer.InitializeServices(scopeBuilder, servicesProvider);

            scopeBuilder
                .WithType<ConvertersService>().As<IConverters>().AsSingleton()
                .WithType<StylesService>().As<IStylesService>().As<IStylesServiceEx>().AsSingleton()
                .WithType<XmlFlagsService>().As<IXmlFlagsService>().AsSingleton()
                .WithType<FormsSounds>().As<IFormsSounds>().AsSingleton()
                .WithType<TransitionsManager>().As<ITransitionsManager>().AsSingleton()
                .WithType<FormsInputService>().As<IFormsInputMapping>().As<IFormsInput>().AsSingleton()
                .WithType<NavigationView>().As<INavigation>().AsSelf().AsSingleton()
                .WithType<Application>().As<IApplication>().AsSelf().AsSingleton()
                .WithType<FontsContainer>().As<IFontsContainer>().As<IFontsLoader>().AsSingleton();

            if (!servicesProvider.TryResolveInstance(out IContentManager contentManager) && !scopeBuilder.HasRegisteredInstance(typeof(IContentManager)))
            {
                contentManager = new ContentManager();
                scopeBuilder.WithInstance(contentManager).As<IContentManager>();
            }

            if (!servicesProvider.TryResolveInstance(out uiHost) && !scopeBuilder.HasRegisteredInstance(typeof(IUiHost)))
            {
                scopeBuilder.WithType<TransparentUiHost>().As<IUiHost>().AsSingleton();
            }

            servicesProvider = scopeBuilder.Build();
            objectFactory = servicesProvider.GetService<IObjectFactory>();
            navigationView = servicesProvider.GetService<NavigationView>();
            uiHost = servicesProvider.GetService<IUiHost>();

            var defaultConverters = servicesProvider.GetService<IConverters>();
            RegisterConverters(defaultConverters, objectFactory);

            application = servicesProvider.GetService<Application>();

            if (!contentManager.CanLoadContent<Texture2D>()) contentManager.SetContentLoader(objectFactory.Create<TextureLoader>());
            if (!contentManager.CanLoadContent<Sound>()) contentManager.SetContentLoader(objectFactory.Create<SoundLoader>());

            var formsStartup = objectFactory.Create<TFormsStartup>();
            formsStartup.Load();
            formsStartup.Run();
        }

        private static void RegisterConverters(IConverters defaultConverters, IObjectFactory objectFactory )
        {
            defaultConverters.RegisterConverter<string, TextSource>(new StringToTextSourceConverter());
            defaultConverters.RegisterConverter<StringBuilder, TextSource>(new StringBuilderToTextSourceConverter());
            defaultConverters.RegisterConverter<string, int>(new StringToIntConverter());
            defaultConverters.RegisterConverter<string, Length>(new StringToLengthConverter());
            defaultConverters.RegisterConverter<string, float>(new StringToFloatConverter());
            defaultConverters.RegisterConverter<string, Margin>(new StringToMarginConverter());
            defaultConverters.RegisterConverter<string, bool>(new StringToBoolConverter());
            defaultConverters.RegisterConverter<string, GridLength[]>(new StringToGridRowColumnDefinitionsConverter());
            defaultConverters.RegisterConverter<string, Color4>(StringToColorConverter.Instance);
            defaultConverters.RegisterConverter<float, Length>(new UniversalConverter<float, Length>( o=>new Length(0, o)));
            defaultConverters.RegisterConverter<float, GridLength>(new UniversalConverter<float, GridLength>(o => new GridLength(GridLengthMode.Value, o)));
            defaultConverters.RegisterConverter<string, ImageSource>(objectFactory.Create<StringToImageSourceConverter>());

            var notConverter = new NotConverter();
            defaultConverters.RegisterConverter("neg", notConverter);
            defaultConverters.RegisterConverter("not", notConverter);
        }

        public void Draw(TimeSpan frameTime)
        {
            uiHost.BeginDraw();
            navigationView.Draw(frameTime);
            uiHost.EndDraw();
        }

        public void Update(TimeSpan frameTime)
        {
            uiHost.Update();
            application.RaiseBeforeUpdate(frameTime);
            navigationView.Update(frameTime);
            application.RaiseAfterUpdate(frameTime);
        }
    }
}
