using CrossX.Audio;
using CrossX.Content;
using CrossX.Content.Loaders;
using CrossX.Core;
using CrossX.Forms.Converters;
using CrossX.Forms.Services;
using CrossX.Forms.Styles;
using CrossX.Forms.Values;
using CrossX.Forms.View;
using CrossX.Graphics;
using CrossX.IoC;
using System;

namespace CrossX.Forms
{
    public class FormsRunner<TServicesInitializer, TFormsStartup> : IApp where TFormsStartup: class, IFormsStartup where TServicesInitializer: class, IServicesInitializer
    {
        private readonly ScopeBuilder scopeBuilder;
        private readonly IGraphicsDevice graphicsDevice;
        private IObjectFactory objectFactory;
        private IServicesProvider servicesProvider;
        private NavigationView navigationView;

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
            servicesInitializer.InitializeServices(scopeBuilder);

            scopeBuilder
                .WithType<DefaultConverters>().As<IDefaultConverters>().AsSingleton()
                .WithType<NavigationView>().As<INavigation>().AsSelf().AsSingleton()
                .WithType<FontsContainer>().As<IFontsContainer>().As<IFontsLoader>().AsSingleton()
                .WithType<StylesService>().As<IStylesService>().AsSingleton()
                .WithType<Application>().As<IApplication>().AsSingleton();

            if (!servicesProvider.TryResolveInstance(out IContentManager contentManager) && !scopeBuilder.HasRegisteredInstance(typeof(IContentManager)))
            {
                contentManager = new ContentManager();
                scopeBuilder.WithInstance(contentManager).As<IContentManager>();
            }

            servicesProvider = scopeBuilder.Build();
            objectFactory = servicesProvider.GetService<IObjectFactory>();
            navigationView = servicesProvider.GetService<NavigationView>();

            var defaultConverters = servicesProvider.GetService<IDefaultConverters>();
            defaultConverters.RegisterConverter<string, int>(new StringToIntConverter());
            defaultConverters.RegisterConverter<string, Length>(new StringToLengthConverter());

            if (!contentManager.CanLoadContent<Texture2D>()) contentManager.SetContentLoader(objectFactory.Create<TextureLoader>());
            if (!contentManager.CanLoadContent<Sound>()) contentManager.SetContentLoader(objectFactory.Create<SoundLoader>());

            var formsStartup = objectFactory.Create<TFormsStartup>();
            formsStartup.Load();
            formsStartup.Run();
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);
            navigationView.Draw(frameTime);
            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            navigationView.Update(frameTime);
        }
    }
}
