using CrossX.Audio;
using CrossX.Content;
using CrossX.Content.Loaders;
using CrossX.Core;
using CrossX.Forms.Services;
using CrossX.Graphics;
using CrossX.IoC;
using System;

namespace CrossX.Forms
{
    public class FormsApp<TServicesInitializer, TFormsStartup> : IApp where TFormsStartup: class, IFormsStartup where TServicesInitializer: class, IServicesInitializer
    {
        private readonly ScopeBuilder scopeBuilder;
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;

        public FormsApp(IServicesProvider servicesProvider, IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            scopeBuilder = new ScopeBuilder().WithParent(servicesProvider);
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            var servicesInitializer = this.objectFactory.Create<TServicesInitializer>();
            servicesInitializer.InitializeServices(scopeBuilder);

            scopeBuilder
                .WithType<Navigation>().As<INavigation>().AsSingleton()
                .WithType<FontsContainer>().As<IFontsContainer>().As<IFontsLoader>().AsSingleton();

            if (!scopeBuilder.TryResolveInstance(out IContentManager contentManager))
            {
                contentManager = new ContentManager();
                scopeBuilder.WithInstance(contentManager).As<IContentManager>();
            }

            var objectFactory = scopeBuilder.Build().GetService<IObjectFactory>();

            if (!contentManager.CanLoadContent<Texture2D>()) contentManager.SetContentLoader(objectFactory.Create<TextureLoader>());
            if (!contentManager.CanLoadContent<Sound>()) contentManager.SetContentLoader(objectFactory.Create<SoundLoader>());

            var formsStartup = objectFactory.Create<TFormsStartup>();
            formsStartup.Load();
            formsStartup.Run();
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Red);
            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            
        }
    }
}
