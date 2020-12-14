using CrossX.Graphics;
using CrossX.Graphics.Effects;
using CrossX.Graphics2D.Sprites;
using CrossX.Graphics2D.Text;
using XxIoC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace CrossX.Graphics2D
{
    public abstract class SpriteBatch<TVertex> : IDisposable where TVertex : struct
    {
        private readonly BasicEffect basicShader;

        private readonly IGraphicsDevice graphicsDevice;
        private readonly ITransform2D transform2D;
        private Texture2D currentTexture;

        protected TVertex[] buffer = new TVertex[2048];
        private VertexBuffer vertexBuffer;

        private int currentIndex = 0;
        private TextureFilter textureFilter = TextureFilter.Linear;
        private TextureMode textureMode = TextureMode.WrapU | TextureMode.WrapV;
        private BlendMode blendMode = BlendMode.AlphaBlend;

        private Texture2D whiteTexture;

        public Matrix4x4 CurrentTransform => transform2D?.Transform ?? Matrix4x4.Identity;

        public TextureFilter TextureFilter
        {
            get => textureFilter;
            set
            {
                if (textureFilter != value) Flush();
                textureFilter = value;
            }
        }
        public TextureMode TextureMode
        {
            get => textureMode;
            set
            {
                if (textureMode != value) Flush();
                textureMode = value;
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

        protected SpriteBatch(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, VertexContent vertexContent, ITransform2D transform2D)
        {
            this.graphicsDevice = graphicsDevice;
            this.transform2D = transform2D;

            using (var stream = typeof(SpriteBatch).Assembly.GetManifestResourceStream("CrossX.Graphics2D.Assets.White9.png"))
            {
                whiteTexture = objectFactory.Create<Texture2D>(stream);
            }

            graphicsDevice.FlushRequest += GraphicsDevice_FlushRequest;

            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = vertexContent,
                Count = buffer.Length
            });

            basicShader = objectFactory.Create<BasicEffect>();

            basicShader.Alpha = 1.0f;
            basicShader.TextureEnabled = true;
            basicShader.VertexColorEnabled = true;
        }

        public void Dispose()
        {
            Flush();
            whiteTexture.Dispose();
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
            basicShader.Sampler = (TextureSamplerDesc)((int)TextureFilter | (int)TextureMode);

            var vpm = Matrix4x4.CreateOrthographicOffCenter(0, graphicsDevice.CurrentTargetSize.Width, graphicsDevice.CurrentTargetSize.Height, 0, 0.1f, 10);

            basicShader.SetViewProjectionTransform(vpm);
            basicShader.SetWorldTransform(transform2D?.Transform ?? Matrix4x4.Identity);
            basicShader.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            var dc = graphicsDevice.DepthClip;
            var bm = graphicsDevice.BlendMode;

            graphicsDevice.BlendMode = BlendMode;
            graphicsDevice.DepthClip = false;

            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, currentIndex);

            graphicsDevice.DepthClip = dc;
            graphicsDevice.BlendMode = bm;

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

        public void DrawSprite(SpriteInstance instance, IDictionary<string, Texture2D> sheets, Vector2 position, Color4 color, SpriteFlags spriteFlags = SpriteFlags.None) 
            => DrawSprite(instance, sheets, position, color, Vector2.One, 0, spriteFlags);

        public void DrawSprite(SpriteInstance instance, IDictionary<string, Texture2D> sheets, Vector2 position, Color4 color, Vector2 scale, float rotation = 0, SpriteFlags spriteFlags = SpriteFlags.None)
        {
            var frame = instance.CurrentFrame;
            if (frame == null) return;
            var texture = sheets[instance.CurrentSequence.SpriteSheet];
            DrawImage(texture, position, frame.SourceRect, color, scale, new Vector2(frame.Origin.X, frame.Origin.Y), rotation, spriteFlags);
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
                // TODO: implement rotation
                //tarTL = Vector2.RotateAround(tarTL, position, rotation);
                //tarTR = Vector2.RotateAround(tarTR, position, rotation);
                //tarBL = Vector2.RotateAround(tarBL, position, rotation);
                //tarBR = Vector2.RotateAround(tarBR, position, rotation);
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
            graphicsDevice.Flush(this);
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

        private struct ARect
        {
            public float Left;
            public float Right;
            public float Top;
            public float Bottom;

            public ARect(float l, float t, float r, float b)
            {
                Left = l;
                Top = t;
                Right = r;
                Bottom = b;
            }
        }

        public void DrawRect(RectangleF rect, Color4 color)
        {
            graphicsDevice.Flush(this);
            SetTexture(whiteTexture);

            var a = 2f / 8f;
            var b = 6f / 8f;

            // TopLeft
            AddRect(new RectangleF(rect.Left - 1, rect.Top - 1, 2, 2), color, new ARect(0, 0, a, a));

            // Top
            AddRect(new RectangleF(rect.Left + 1, rect.Top - 1, rect.Width - 2, 2), color, new ARect(a, 0, b, a));

            // TopRight
            AddRect(new RectangleF(rect.Right - 1, rect.Top - 1, 2, 2), color, new ARect(b, 0, 1, a));

            // Left
            AddRect(new RectangleF(rect.Left - 1, rect.Top + 1, 2, rect.Height - 2), color, new ARect(0, a, a, b));

            // Middle
            AddRect(new RectangleF(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2), color, new ARect(a, a, b, b));

            // Right
            AddRect(new RectangleF(rect.Right - 1, rect.Top + 1, 2, rect.Height - 2), color, new ARect(b, a, 1, b));

            // Bottom
            AddRect(new RectangleF(rect.Left + 1, rect.Bottom - 1, rect.Width - 2, 2), color, new ARect(a, b, b, 1));

            // BottomLeft
            AddRect(new RectangleF(rect.Left - 1, rect.Bottom - 1, 2, 2), color, new ARect(0, b, a, 1));

            // BottomRight
            AddRect(new RectangleF(rect.Right - 1, rect.Bottom - 1, 2, 2), color, new ARect(b, b, 1, 1));
        }

        private void AddRect(RectangleF targetRect, Color4 color, ARect tex)
        {
            AddVertex(new Vector2(targetRect.Left, targetRect.Top), new Vector2(tex.Left, tex.Top), color);
            AddVertex(new Vector2(targetRect.Left, targetRect.Bottom), new Vector2(tex.Left, tex.Bottom), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), new Vector2(tex.Right, tex.Bottom), color);

            AddVertex(new Vector2(targetRect.Left, targetRect.Top), new Vector2(tex.Left, tex.Top), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Bottom), new Vector2(tex.Right, tex.Bottom), color);
            AddVertex(new Vector2(targetRect.Right, targetRect.Top), new Vector2(tex.Right, tex.Top), color);
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
            if ((currentIndex % 3) == 0 && currentIndex > buffer.Length - 3) Flush();
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
