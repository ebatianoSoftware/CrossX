using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using CrossX.Graphics2D.Text;
using CrossX.IoC;
using System;
using System.Drawing;

namespace CrossX.Graphics2D
{
    public abstract class SpriteBatch<TVertex> : IDisposable where TVertex : struct
    {
        private readonly BasicShader basicShader;

        private readonly IGraphicsDevice graphicsDevice;
        private readonly ITransform2D transform2D;
        private Texture2D currentTexture;

        protected TVertex[] buffer = new TVertex[2048];
        private VertexBuffer vertexBuffer;

        private int currentIndex = 0;

        protected SpriteBatch(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, VertexContent vertexContent, ITransform2D transform2D)
        {
            this.graphicsDevice = graphicsDevice;
            this.transform2D = transform2D;
            graphicsDevice.FlushRequest += GraphicsDevice_FlushRequest;

            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = vertexContent,
                Count = buffer.Length
            });

            basicShader = objectFactory.Create<BasicShader>();
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

            basicShader.Texture = currentTexture;
            basicShader.DiffuseColor = Color4.White;
            basicShader.Alpha = 1.0f;
            basicShader.Apply();

            var vpm = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height, 0, 0.1f, 10);
            basicShader.SetViewProjectionTransform(vpm);
            basicShader.SetWorldTransform(transform2D?.Transform ?? Matrix.Identity);

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, currentIndex);

            currentIndex = 0;
        }

        public void DrawImage(Texture2D texture, RectangleF targetRect, Rectangle? srcRect, Color4 color, SpriteFlags spriteFlags = SpriteFlags.None)
        {
            graphicsDevice.Flush(this);
            SetTexture(texture);
            if (currentIndex >= buffer.Length - 6) Flush();

            var srcRect1 = srcRect ?? new Rectangle(0, 0, texture.Width, texture.Height);

            var srcL = srcRect1.X / (float)texture.Width;
            var srcT = srcRect1.Y / (float)texture.Height;
            var srcR = srcRect1.Right / (float)texture.Width;
            var srcB = srcRect1.Bottom / (float)texture.Height;

            if (spriteFlags.HasFlag(SpriteFlags.FlipHorizontal))
            {
                XxUtils.Swap(ref srcL, ref srcR);
            }

            if (spriteFlags.HasFlag(SpriteFlags.FlipVertical))
            {
                XxUtils.Swap(ref srcT, ref srcB);
            }

            AddVertex(new Vector2(targetRect.Left, targetRect.Top), new Vector2(srcL, srcT), color);
            AddVertex(new Vector2(targetRect.Left, targetRect.Bottom), new Vector2(srcL, srcB), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), new Vector2(srcR, srcB), color);

            AddVertex(new Vector2(targetRect.Left, targetRect.Top), new Vector2(srcL, srcT), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), new Vector2(srcR, srcB), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Top), new Vector2(srcR, srcT), color);
        }

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? srcRect, Color4 color, Vector2 scale, SpriteFlags spriteFlags = SpriteFlags.None)
        {
            var srcRect1 = srcRect ?? new Rectangle(0, 0, texture.Width, texture.Height);
            var targetRect = new RectangleF(position.X, position.Y, scale.X * srcRect1.Width, scale.Y * srcRect1.Height);
            DrawImage(texture, targetRect, srcRect1, color, spriteFlags);
        }

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? srcRect, Color4 color, float scale, SpriteFlags spriteFlags = SpriteFlags.None)
            => DrawImage(texture, position, srcRect, color, new Vector2(scale), spriteFlags);

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? srcRect, Color4 color, float scale, Vector2 origin, float rotation, SpriteFlags spriteFlags = SpriteFlags.None)
            => DrawImage(texture, position, srcRect, color, new Vector2(scale), origin, rotation, spriteFlags);

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? srcRect, Color4 color, Vector2 scale, Vector2 origin, float rotation, SpriteFlags spriteFlags = SpriteFlags.None)
        {
            graphicsDevice.Flush(this);
            SetTexture(texture);
            if (currentIndex >= buffer.Length - 6) Flush();

            var srcRect1 = srcRect ?? new Rectangle(0, 0, texture.Width, texture.Height);

            var srcL = srcRect1.X / (float)texture.Width;
            var srcT = srcRect1.Y / (float)texture.Height;
            var srcR = srcRect1.Right / (float)texture.Width;
            var srcB = srcRect1.Bottom / (float)texture.Height;

            if (spriteFlags.HasFlag(SpriteFlags.FlipHorizontal))
            {
                XxUtils.Swap(ref srcL, ref srcR);
            }

            if (spriteFlags.HasFlag(SpriteFlags.FlipVertical))
            {
                XxUtils.Swap(ref srcT, ref srcB);
            }

            var tarTL = new Vector2(position.X, position.Y);
            var tarTR = new Vector2(position.X + scale.X * srcRect1.Width, position.Y);

            var tarBL = new Vector2(position.X, position.Y + scale.Y * srcRect1.Height);
            var tarBR = new Vector2(position.X + scale.X * srcRect1.Width, position.Y + scale.Y * srcRect1.Height);

            origin = origin * scale;

            tarTL -= origin;
            tarTR -= origin;
            tarBL -= origin;
            tarBR -= origin;

            if (Math.Abs(rotation) > float.Epsilon)
            {
                tarTL = Vector2.RotateAround(tarTL, position, rotation);
                tarTR = Vector2.RotateAround(tarTR, position, rotation);
                tarBL = Vector2.RotateAround(tarBL, position, rotation);
                tarBR = Vector2.RotateAround(tarBR, position, rotation);
            }

            AddVertex(new Vector2(tarTL.X, tarTL.Y), new Vector2(srcL, srcT), color);
            AddVertex(new Vector2(tarBL.X, tarBL.Y), new Vector2(srcL, srcB), color);
            AddVertex(new Vector2(tarBR.X, tarBR.Y), new Vector2(srcR, srcB), color);

            AddVertex(new Vector2(tarTL.X, tarTL.Y), new Vector2(srcL, srcT), color);
            AddVertex(new Vector2(tarBR.X, tarBR.Y), new Vector2(srcR, srcB), color);
            AddVertex(new Vector2(tarTR.X, tarTR.Y), new Vector2(srcR, srcT), color);
        }

        public void DrawText(TextObject text, Vector2 position, Color4 color)
        {
            for (var idx = 0; idx < text.Vertices.Count; ++idx)
            {
                var vertices = text.Vertices[idx];
                if (vertices.Count == 0) continue;
                SetTexture(text.Font.Textures[idx]);

                for (var vi = 0; vi < vertices.Count; ++vi)
                {
                    var vertex = vertices[vi];
                    AddVertex(vertex.Position + position, vertex.TextureCoordinate, color);
                }
            }
        }

        private void SetTexture(Texture2D texture)
        {
            if (currentTexture != texture)
            {
                Flush();
                currentTexture = texture;
            }
        }

        private void AddVertex(Vector2 position, Vector2 coordinates, Color4 color)
        {
            SetVertex(currentIndex++, position, coordinates, color);
        }

        protected abstract void SetVertex(int index, Vector2 position, Vector2 coordinates, Color4 color);
    }

    public sealed class SpriteBatch : SpriteBatch<VertexPCT>
    {
        public SpriteBatch(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, ITransform2D transform2D = null) : base(graphicsDevice, objectFactory, VertexPCT.Content, transform2D)
        {
        }

        protected override void SetVertex(int index, Vector2 position, Vector2 coordinates, Color4 color)
        {
            buffer[index] = new VertexPCT
            {
                Position = new Vector4(position, 0, 1),
                Color = color,
                TextureCoordinate = coordinates
            };
        }
    }

    public sealed class SpriteBatchWithNormals : SpriteBatch<VertexPNCT>
    {
        public SpriteBatchWithNormals(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, ITransform2D transform2D = null) : base(graphicsDevice, objectFactory, VertexPNCT.Content, transform2D)
        {
        }

        protected override void SetVertex(int index, Vector2 position, Vector2 coordinates, Color4 color)
        {
            buffer[index] = new VertexPNCT
            {
                Position = new Vector4(position, 0, 1),
                Normal = new Vector4(0, 0, 1, 0),
                Color = color,
                TextureCoordinate = coordinates
            };
        }
    }
}
