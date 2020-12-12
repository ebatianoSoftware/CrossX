using CrossX.Graphics;
using CrossX.Graphics.Effects;
using S2IoC;
using System;
using System.Drawing;

namespace CrossX.Graphics2D
{
    public sealed class PrimitiveBatch : IDisposable
    {
        private readonly BasicEffect basicShader;
        private readonly IGraphicsDevice graphicsDevice;
        private readonly ITransform2D transform2D;
        private VertexPC[] buffer = new VertexPC[2048];
        private VertexBuffer vertexBuffer;

        private int currentIndex = 0;
        private BlendMode blendMode = BlendMode.AlphaBlend;
        private bool lines;

        private bool Lines 
        { 
            get => lines;
            set
            {
                if (lines != value) Flush();
                lines = value;
            }
        }

        public BlendMode BlendMode
        {
            get => blendMode;
            set
            {
                if (blendMode != value) Flush();
                blendMode = value;
            }
        }

        public PrimitiveBatch(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, ITransform2D transform2D = null)
        {
            this.graphicsDevice = graphicsDevice;
            this.transform2D = transform2D;
            graphicsDevice.FlushRequest += GraphicsDevice_FlushRequest;

            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPC.Content,
                Count = buffer.Length
            });

            basicShader = objectFactory.Create<BasicEffect>();
            basicShader.Alpha = 1.0f;
            basicShader.VertexColorEnabled = true;
            basicShader.TextureEnabled = false;
        }

        public void Dispose()
        {
            Flush();
            vertexBuffer.Dispose();
            graphicsDevice.FlushRequest -= GraphicsDevice_FlushRequest;
        }

        private void GraphicsDevice_FlushRequest(object sender, EventArgs _)
        {
            if (sender != this)
            {
                Flush();
            }
        }

        public void Flush()
        {
            if (currentIndex == 0) return;

            vertexBuffer.SetData(buffer);

            basicShader.DiffuseColor = Color4.White;

            var vpm = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height, 0, 0.1f, 10);
            basicShader.SetViewProjectionTransform(vpm);
            basicShader.SetWorldTransform(transform2D?.Transform ?? Matrix.Identity);

            basicShader.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            var dc = graphicsDevice.DepthClip;
            var bm = graphicsDevice.BlendMode;

            graphicsDevice.BlendMode = BlendMode;
            graphicsDevice.DepthClip = false;

            graphicsDevice.DrawPrimitives(lines ? PrimitiveType.LineList : PrimitiveType.TriangleList, 0, currentIndex);

            graphicsDevice.DepthClip = dc;
            graphicsDevice.BlendMode = bm;

            currentIndex = 0;
        }

        public void DrawRect(RectangleF targetRect, Color4 color)
        {
            graphicsDevice.Flush(this);
            Lines = false;

            AddVertex(new Vector2(targetRect.Left, targetRect.Top), color);
            AddVertex(new Vector2(targetRect.Left, targetRect.Bottom), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), color);

            AddVertex(new Vector2(targetRect.Left, targetRect.Top), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Top), color);
        }

        public void DrawCircle(Vector2 center, float radius, Color4 color, int steps = 0)
        {
            DrawOval(new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2), color, steps);
        }

        public void DrawOval(Vector2 center, Vector2 size, Color4 color, int steps = 0)
        {
            DrawOval(new RectangleF(center.X - size.X / 2, center.Y - size.Y / 2, size.X, size.Y), color, steps);
        }

        public void DrawOval(RectangleF targetRect, Color4 color, int steps = 0)
        {
            graphicsDevice.Flush(this);
            Lines = false;
            var center = new Vector2(targetRect.X + targetRect.Width / 2, targetRect.Y + targetRect.Height / 2);
            var radX = targetRect.Width / 2;
            var radY = targetRect.Height / 2;

            if (steps <= 0)
            {
                steps = (int)(Math.Max(radX, radY) / 2.25);
            }

            float stepAngle = (float)(Math.PI * 2 / steps);

            for (var idx = 0; idx < steps; ++idx)
            {
                var angle1 = idx * stepAngle;
                var angle2 = (idx + 1) * stepAngle;

                var pos1 = new Vector2(radX * (float)Math.Cos(angle1), radY * (float)Math.Sin(angle1)) + center;
                var pos2 = new Vector2(radX * (float)Math.Cos(angle2), radY * (float)Math.Sin(angle2)) + center;

                AddVertex(center, color);
                AddVertex(pos2, color);
                AddVertex(pos1, color);
            }
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color4 color)
        {
            graphicsDevice.Flush(this);
            Lines = true;

            AddVertex(p1, color);
            AddVertex(p2, color);
        }

        private void AddVertex(Vector2 position, Color4 color)
        {
            buffer[currentIndex++] = new VertexPC
            {
                Position = new Vector4(position, 0, 1),
                Color = color
            };

            int c = lines ? 2 : 3;
            if (currentIndex % c == 0 && currentIndex + c >= buffer.Length)
            {
                Flush();
            }
        }
    }
}
