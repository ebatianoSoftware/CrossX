using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics2D;
using CrossX.Graphics2D.Text;
using CrossX.IoC;
using CrossX.Media.Formats;
using System;
using System.Drawing;

namespace T04.Graphics2D
{
    public class T04_Graphics2DApp : IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private readonly SpriteBatch spriteBatch;
        private readonly PrimitiveBatch primitiveBatch;
        private readonly TextObjectFactory textObjectFactory;
        private Texture2D texture = null;
        private ITransform2D transform2D;
        private TextObject text;

        private float rotation = 0;

        public T04_Graphics2DApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;

            transform2D = objectFactory.Create<Transform2D>();

            spriteBatch = objectFactory.Create<SpriteBatch>(transform2D);
            primitiveBatch = objectFactory.Create<PrimitiveBatch>(transform2D);
            textObjectFactory = new TextObjectFactory();
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);
            
            spriteBatch.DrawImage(texture, Vector2.Zero, null, Color4.White, 1);

            primitiveBatch.BlendMode = BlendMode.Add;

            primitiveBatch.DrawRect(new RectangleF(300, 300, 200, 200), Color4.Green);

            primitiveBatch.DrawOval(new RectangleF(700, 700, 200, 100), Color4.Orange);

            primitiveBatch.BlendMode = BlendMode.AlphaBlend;

            transform2D.Push(Matrix.CreateTranslation(new Vector3(800, 400, 0)));
            transform2D.Push(Matrix.CreateRotationZ(rotation));
            spriteBatch.DrawText(text, -text.Size / 2, Color4.Yellow);
            transform2D.Pop();
            transform2D.Pop();
            graphicsDevice.Present();
        }

        public void LoadContent()
        {
            using (var stream = typeof(T04_Graphics2DApp).Assembly.GetManifestResourceStream("T04.Graphics2D.Texture.png"))
            {
                texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
            }

            using (var stream = typeof(T04_Graphics2DApp).Assembly.GetManifestResourceStream("T04.Graphics2D.Font.fnt"))
            {
                var font = new Font(stream, name =>
                {
                    using (var stream2 = typeof(T04_Graphics2DApp).Assembly.GetManifestResourceStream($"T04.Graphics2D.{name}"))
                    {
                        return objectFactory.Create<Texture2D>(stream2);
                    }
                });

                text = textObjectFactory.CreateText(font, new TextSource("This text is multiline text with justify. Word wrap happens automatically."), 24, 256, TextAlignment.Justify);
            }
        }

        public void Update(TimeSpan frameTime)
        {
            rotation += (float)frameTime.TotalSeconds;
        }
    }
}
