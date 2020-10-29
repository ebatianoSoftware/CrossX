using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using CrossX.IoC;
using CrossX.Media.Formats;
using System;

namespace T02.Textures
{
    public class T02_TexturesApp: IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private VertexBuffer vertexBuffer;
        private BasicShader basicShader;
        private Texture2D texture = null;
        public T02_TexturesApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            basicShader = objectFactory.Create<BasicShader>();
            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPT.Content,
                Count = 4
            });

            VertexPT[] vertices = new VertexPT[]
            {
                new VertexPT
                {
                    Position = new Vector4(-0.5f, -0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,1)
                },
                new VertexPT
                {
                    Position = new Vector4(-0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,0)
                },
                new VertexPT
                {
                    Position = new Vector4(0.5f, -0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(1,1)
                },
                new VertexPT
                {
                    Position = new Vector4(0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(1,0)
                },
            };
            vertexBuffer.SetData(vertices);

            using(var stream = typeof(T02_TexturesApp).Assembly.GetManifestResourceStream("T02.Textures.Texture.png"))
            {
                texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
            }
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);

            basicShader.Texture = texture;
            basicShader.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            
        }
    }
}
