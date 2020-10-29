using CrossX.Core;
using CrossX.Graphics;
using CrossX.UWP;
using System;
using CrossX;
using CrossX.IoC;
using CrossX.Data;
using CrossX.IO;
using CrossX.Media.Formats;

namespace CrossXExample.UWP
{
    internal class App : IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private Texture2D texture;
        private readonly IObjectFactory objectFactory;
        private readonly IFilesRepository filesRepository;
        private readonly SpriteBatch spriteBatch;
        private readonly PrimitiveBatch primitiveBatch;

        private float rotation = 0;

        public App(IGraphicsDevice graphicsDevice, IServicesProvider serviceProvider)
        {
            this.graphicsDevice = graphicsDevice;

            var filesRepository = new MultiSourceFilesRepository();
            filesRepository.DefaultSource = new EmbededResourceFileSource(GetType().Assembly, "CrossXExample.UWP.Resources");

            this.filesRepository = filesRepository;
            
            serviceProvider = new ScopeBuilder()
                .WithParent(serviceProvider)
                .WithInstance(filesRepository).As<IFilesRepository>()
                .WithInstance(ImagesFormat.Instance).As<IRawLoader<RawImage>>().AsSingleton()
                .Build();

            objectFactory = serviceProvider.GetService<IObjectFactory>();
            spriteBatch = objectFactory.Create<SpriteBatch>();
            primitiveBatch = objectFactory.Create<PrimitiveBatch>();
        }

        public void Draw(TimeSpan frameTime)
        {
            rotation += (float)frameTime.TotalSeconds * 4;

            graphicsDevice.BeginRender();
            graphicsDevice.Clear(Color4.Orange);
            graphicsDevice.BlendMode = BlendMode.AlphaBlend;

            primitiveBatch.DrawCircle(new Vector2(graphicsDevice.CurrentTargetSize.Width / 2, graphicsDevice.CurrentTargetSize.Height / 2), 
                Math.Min(graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height) / 2.2f, Color4.Black);

            spriteBatch.DrawImage(texture, new Vector2(graphicsDevice.CurrentTargetSize.Width / 2, graphicsDevice.CurrentTargetSize.Height / 2),
                null, Color4.White, 1, new Vector2(texture.Width / 2, texture.Height / 2), rotation);

            graphicsDevice.Present();
        }

        public void LoadContent()
        {
            using (var stream = filesRepository.Open("Texture.png"))
            {
                texture = objectFactory.Create<Texture2D>(stream);
            }
        }

        public void Update(TimeSpan frameTime)
        {
            
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var runner = new AppRunner<App>();
            runner.Run(0);
        }   
    }
}
