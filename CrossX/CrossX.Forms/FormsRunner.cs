using CrossX.Audio;
using CrossX.Content;
using CrossX.Content.Loaders;
using CrossX.Core;
using CrossX.Forms.Services;
using CrossX.Forms.Styles;
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
        }

        public void LoadContent()
        {
            var servicesInitializer = objectFactory.Create<TServicesInitializer>();
            servicesInitializer.InitializeServices(scopeBuilder);

            scopeBuilder
                .WithType<NavigationView>().As<INavigation>().AsSelf().AsSingleton()
                .WithType<FontsContainer>().As<IFontsContainer>().As<IFontsLoader>().AsSingleton()
                .WithType<StylesService>().As<IStylesService>().AsSingleton()
                .WithType<Application>().As<IApplication>().AsSingleton();

            if (!scopeBuilder.TryResolveInstance(out IContentManager contentManager))
            {
                contentManager = new ContentManager();
                scopeBuilder.WithInstance(contentManager).As<IContentManager>();
            }

            servicesProvider = scopeBuilder.Build();
            objectFactory = servicesProvider.GetService<IObjectFactory>();
            navigationView = servicesProvider.GetService<NavigationView>();

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
