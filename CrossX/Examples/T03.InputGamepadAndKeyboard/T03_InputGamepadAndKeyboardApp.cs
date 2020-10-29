using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using CrossX.Input;
using CrossX.IoC;
using System;

namespace T03.InputGamepadAndKeyboard
{
    public class T03_InputGamepadAndKeyboardApp: IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private readonly IGamePads gamePads;
        private readonly IKeyboard keyboard;
        private VertexBuffer vertexBuffer;
        private BasicShader basicShader;

        private Vector2 offset = Vector2.Zero;
        private float rotation = 0;

        public T03_InputGamepadAndKeyboardApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, 
                IGamePads gamePads, IKeyboard keyboard)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.gamePads = gamePads;
            this.keyboard = keyboard;
        }

        #region Drawing Rectangle

        public void LoadContent()
        {
            basicShader = objectFactory.Create<BasicShader>();
            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPC.Content,
                Count = 4
            });

            VertexPC[] vertices = new VertexPC[]
            {
                new VertexPC
                {
                    Position = new Vector4(-100, -100, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(-100, 100, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(100, -100, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(100, 100, 0.0f, 1),
                    Color = Color4.White
                },
            };
            vertexBuffer.SetData(vertices);
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);
            
            var worldMatrix = 
                Matrix.CreateRotationY(rotation) *
                Matrix.CreateTranslation(graphicsDevice.Size.Width / 2 + offset.X, graphicsDevice.Size.Height / 2 + offset.Y, 0);
            basicShader.SetWorldTransform(worldMatrix);

            var viewProjMatrix = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Size.Width, graphicsDevice.Size.Height, 0, 0.1f, 10);
            basicShader.SetViewProjectionTransform(viewProjMatrix);
            
            basicShader.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);

            graphicsDevice.Present();
        }

        #endregion

        public void Update(TimeSpan frameTime)
        {
            var moveSpeed = (float)frameTime.TotalSeconds * 200;
            var rotSpeed = (float)frameTime.TotalSeconds * (float)Math.PI;
            var state = gamePads.GetState(0);

            if (keyboard.GetKeyState(Key.Right).HasFlag(KeyBtnState.Down) || state.LeftThumbStick.X > 0.25f)
            {
                offset.X += moveSpeed;
            }

            if (keyboard.GetKeyState(Key.Left).HasFlag(KeyBtnState.Down) || state.LeftThumbStick.X < -0.25f)
            {
                offset.X -= moveSpeed;
            }

            if (keyboard.GetKeyState(Key.Down).HasFlag(KeyBtnState.Down) || state.LeftThumbStick.Y < -0.25f)
            {
                offset.Y += moveSpeed;
            }

            if (keyboard.GetKeyState(Key.Up).HasFlag(KeyBtnState.Down) ||  state.LeftThumbStick.Y > 0.25f)
            {
                offset.Y -= moveSpeed;
            }

            if(keyboard.GetKeyState(Key.A).HasFlag(KeyBtnState.Down) || state.GetButtonState(GamePadButton.X).HasFlag(KeyBtnState.Down))
            {
                rotation += rotSpeed;
            }

            if (keyboard.GetKeyState(Key.D).HasFlag(KeyBtnState.Down) || state.GetButtonState(GamePadButton.B).HasFlag(KeyBtnState.Down))
            {
                rotation -= rotSpeed;
            }

            offset.X = Math.Min(200, Math.Max(-200, offset.X));
            offset.Y = Math.Min(200, Math.Max(-200, offset.Y));
        }
    }
}
