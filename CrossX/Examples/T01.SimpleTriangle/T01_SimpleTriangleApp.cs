using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
using XxIoC;
using System;
using System.Numerics;

namespace T01.SimpleTriangle
{
    public class T01_SimpleTriangleApp : IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private VertexBuffer vertexBuffer;
        private BasicEffect basicShader;

        public T01_SimpleTriangleApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            basicShader = objectFactory.Create<BasicEffect>();
            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPC.Content,
                Count = 3
            });

            VertexPC[] vertices = new VertexPC[]
            {
                new VertexPC
                {
                    Position = new Vector4(0.0f, 0.5f, 0.5f, 1),
                    Color = Color4.Red
                },
                new VertexPC
                {
                    Position = new Vector4(-0.5f, -0.5f, 0.5f, 1),
                    Color = Color4.Green
                },
                new VertexPC
                {
                    Position = new Vector4(0.5f, -0.5f, 0.5f, 1),
                    Color = Color4.Blue
                },
            };
            vertexBuffer.SetData(vertices);
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);
            
            basicShader.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            
        }
    }
}
