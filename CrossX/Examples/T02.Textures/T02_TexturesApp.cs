using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
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
        private BasicEffect basicShader;
        private Texture2D texture = null;

        public T02_TexturesApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            basicShader = objectFactory.Create<BasicEffect>();
            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPT.Content,
                Count = 4
            });

            // Remember Counter Clockwise primitives!
            VertexPT[] vertices = new VertexPT[]
            {
                new VertexPT
                {
                    Position = new Vector4(-0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,0)
                },
                new VertexPT
                {
                    Position = new Vector4(-0.5f, -0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,4)
                },
                new VertexPT
                {
                    Position = new Vector4(0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(4,0)
                },
                new VertexPT
                {
                    Position = new Vector4(0.5f, -0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(4,4)
                }
            };
            vertexBuffer.SetData(vertices);

            using(var stream = typeof(T02_TexturesApp).Assembly.GetManifestResourceStream("T02.Textures.Texture.png"))
            {
                texture = objectFactory.Create<Texture2D>(stream);
            }
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);

            basicShader.Sampler = TextureSamplerDesc.Linear;
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
