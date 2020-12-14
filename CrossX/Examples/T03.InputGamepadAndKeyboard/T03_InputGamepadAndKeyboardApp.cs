using CrossX;
using CrossX.Core;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
using CrossX.Input;
using XxIoC;
using System;

namespace T03.InputGamepadAndKeyboard
{
    public class T03_InputGamepadAndKeyboardApp: IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private readonly IGamePads gamePads;
        private readonly IKeyboard keyboard;
        private readonly IMouse mouse;
        private VertexBuffer vertexBuffer;
        private IndexBuffer2 indexBuffer;

        private BasicEffect basicEffect;

        private Vector2 offset = Vector2.Zero;
        private float rotation = 0;

        private float size = 100;

        public T03_InputGamepadAndKeyboardApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, 
                IGamePads gamePads, IKeyboard keyboard, IMouse mouse)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.gamePads = gamePads;
            this.keyboard = keyboard;
            this.mouse = mouse;
        }

        #region Drawing Rectangle

        public void LoadContent()
        {
            basicEffect = objectFactory.Create<BasicEffect>();

            basicEffect.TextureEnabled = false;
            basicEffect.VertexColorEnabled = true;

            vertexBuffer = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                VertexContent = VertexPC.Content,
                Count = 4
            });

            indexBuffer = objectFactory.Create<IndexBuffer2>(6);

            VertexPC[] vertices = new VertexPC[]
            {
                new VertexPC
                {
                    Position = new Vector4(-1, -1, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(-1, 1, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(1, -1, 0.0f, 1),
                    Color = Color4.White
                },
                new VertexPC
                {
                    Position = new Vector4(1, 1, 0.0f, 1),
                    Color = Color4.White
                },
            };

            var indices = new ushort[]
            {
                0,1,2,
                3,2,1
            };

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);
            
            var worldMatrix =
                Matrix.CreateScale(100) *
                Matrix.CreateRotationY(rotation) *
                Matrix.CreateTranslation(graphicsDevice.Size.Width / 2 + offset.X, graphicsDevice.Size.Height / 2 + offset.Y, 0);
            basicEffect.SetWorldTransform(worldMatrix);

            var viewProjMatrix = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Size.Width, graphicsDevice.Size.Height, 0, 0.1f, 10);
            basicEffect.SetViewProjectionTransform(viewProjMatrix);
            basicEffect.DiffuseColor = Color4.White;
            basicEffect.Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.SetIndexBuffer(indexBuffer);

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 6);

            var pos = mouse.Position;

            worldMatrix =
                Matrix.CreateScale(size) *
                Matrix.CreateTranslation(new Vector3(pos, 0));
            basicEffect.SetWorldTransform(worldMatrix);

            basicEffect.DiffuseColor = (mouse.GetButtonState(MouseButtons.Left).HasFlag(KeyBtnState.Down) ? Color4.LightCoral : Color4.White) *
                                        (mouse.GetButtonState(MouseButtons.Right).HasFlag(KeyBtnState.Down) ? Color4.LightGreen : Color4.White) *
                                        (mouse.GetButtonState(MouseButtons.Middle).HasFlag(KeyBtnState.Down) ? Color4.LightBlue : Color4.White);
            basicEffect.Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 6);

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

            size += mouse.WheelDelta / 20;
            size = Math.Min(Math.Max(10, size), 200);
        }
    }
}
