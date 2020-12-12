using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
using S2IoC;
using CrossX.Media.Formats;
using System;

namespace T02.Textures
{
    public class T02_TexturesApp: IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private VertexBuffer vertexBuffer;
        private BasicEffect basicEffect;
        private Texture2D texture = null;

        public T02_TexturesApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            basicEffect = objectFactory.Create<BasicEffect>();
            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = false;
            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPNT.Content,
                Count = 4
            });

            // Remember Counter Clockwise primitives!
            VertexPNT[] vertices = new []
            {
                new VertexPNT
                {
                    Position = new Vector4(-0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,0)
                },
                new VertexPNT
                {
                    Position = new Vector4(-0.5f, -0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(0,4)
                },
                new VertexPNT
                {
                    Position = new Vector4(0.5f, 0.5f, 0.5f, 1),
                    TextureCoordinate = new Vector2(4,0)
                },
                new VertexPNT
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

            basicEffect.Sampler = TextureSamplerDesc.Linear | TextureSamplerDesc.MirrorUV;
            basicEffect.Texture = texture;
            basicEffect.VertexHasNormals = true;

            var projView = Matrix.CreateLookAt(
                    new Vector3(1, 1, 1).Normalized() * 2,
                    Vector3.Zero,
                    Vector3.Up) *
                    Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3f, (float)graphicsDevice.CurrentTargetSize.Width / graphicsDevice.CurrentTargetSize.Height, 0.1f, 1000f);

            basicEffect.SetViewProjectionTransform(projView);

            basicEffect.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            
        }
    }
}
