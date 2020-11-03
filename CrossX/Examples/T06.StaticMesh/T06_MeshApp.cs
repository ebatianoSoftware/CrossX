using CrossX;
using CrossX.Core;
using CrossX.Data;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
using CrossX.Graphics3D;
using CrossX.Graphics3D.Light;
using CrossX.Input;
using CrossX.IoC;
using CrossX.Media.Formats;
using System;

namespace T06.StaticMesh
{
    public class T06_MeshApp : IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private readonly IMouse mouse;
        private Mesh mesh;
        private LightedEffect lightedEffect;
        private float rotation = 0;
        private float rotation2 = 0;

        private float yaw = 0;
        private float pitch = 0;

        private Vector2 lastMouse;

        public T06_MeshApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IMouse mouse)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.mouse = mouse;

            lastMouse = mouse.Position;
        }

        public void LoadContent()
        {
            RawMesh rawMesh = null;
            using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream("T06.StaticMesh.Aston.AM-01.obj"))
            {
                rawMesh = WavefrontObjFormat.Instance.FromStream(stream, new OpenMaterialFileDelegate( name =>
                {
                    name = name.Replace("./", "");
                    return typeof(T06_MeshApp).Assembly.GetManifestResourceStream($"T06.StaticMesh.Aston.{name}");
                }));
            }

            lightedEffect = objectFactory.Create<LightedEffect>();
            mesh = objectFactory.Create<Mesh>(rawMesh, new LoadTextureDelegate(LoadTexture));
        }

        private void LoadTexture(string name, TextureTarget target, out Texture2D texture, out bool managedOutside)
        {
            managedOutside = false;
            texture = null;
            if (name == null)
            {
                switch(target)
                {
                    case TextureTarget.Diffuse:
                        using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream($"T06.StaticMesh.Plane.done2.png"))
                        {
                            texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
                        }
                        break;

                    case TextureTarget.Normal:
                        using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream($"T06.StaticMesh.Plane.done2NM.png"))
                        {
                            texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
                        }
                        break;

                    case TextureTarget.Specular:
                        using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream($"T06.StaticMesh.Plane.done2Spec.png"))
                        {
                            texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
                        }
                        break;
                }
                return;
            }

            using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream($"T06.StaticMesh.Aston.{name}"))
            {
                texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
            }
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.Black);

            lightedEffect.SetWorldTransform(
                Matrix.CreateTranslation(-mesh.Bounds.Center) *
                Matrix.CreateFromYawPitchRoll(yaw, 0, pitch));

            var dist = MathHelper.Max(mesh.Bounds.Width, mesh.Bounds.Height) * 2.5f;

            var cameraPos = new Vector3(1, 1, -1).Normalized() * dist;
            var lpos = cameraPos;

            lpos = Vector3.Transform(cameraPos,
                Matrix.CreateRotationY(rotation2)
                );

            var projView = Matrix.CreateLookAt(
                    cameraPos,
                    Vector3.Zero,
                    Vector3.Up) *
                    Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3f, (float)graphicsDevice.CurrentTargetSize.Width / graphicsDevice.CurrentTargetSize.Height, 1f, dist * 1.5f);

            lightedEffect.Reset();

            lightedEffect.SetViewProjectionTransform(projView);
            lightedEffect.CameraPosition = cameraPos;

            lightedEffect.AddLight(new DirectionalLight
            {
                Direction = new Vector3(-0.3f, -1, 0).Normalized(),
                Color = new Color4(255, 255, 224)
            });

            lightedEffect.AddLight(new DirectionalLight
            {
                Direction = new Vector3(-1, 1, 0).Normalized(),
                Color = new Color4(128, 96, 0)
            });

            //lightedEffect.AddLight(new PointLight
            //{
            //    Position = lpos,
            //    Color = new Color4(128,0,255),
            //    Attenuation = new Vector4(1, 0.2f, 0.1f, 0.01f)
            //});

            //lightedEffect.AddLight(new PointLight
            //{
            //    Position = -lpos,
            //    Color = new Color4(0, 255, 0),
            //    Attenuation = new Vector4(1, 0.2f, 0.1f, 0.01f)
            //});


            //lightedEffect.AddLight(new DirectionalLight
            //{
            //    Direction = new Vector3(-0.3f, 1, 0).Normalized(),
            //    Color = new Color4(0, 128,255)
            //});

            lightedEffect.MaterialDiffuseColor = new Color4(255, 255, 255);
            lightedEffect.AmbientColor = new Color4(0, 16, 32);

            graphicsDevice.DepthClip = true;

            for (var idx = 0; idx < mesh.Slices.Count; ++idx)
            {
                var slice = mesh.Slices[idx];
                lightedEffect.Texture = slice.Texture;
                lightedEffect.SpecularTexture = slice.SpecularMap;

                lightedEffect.SpecularColor = new Color4(255, 224, 192);
                lightedEffect.SpecularExponent = slice.Material?.SpecularExponent ?? 64;

                lightedEffect.Apply();

                graphicsDevice.SetIndexBuffer(slice.Indices);
                graphicsDevice.SetVertexBuffer(mesh.Vertices);

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, slice.Indices.Count);
            }

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            //rotation += (float)frameTime.TotalSeconds * 1.23f;
            rotation2 += (float)frameTime.TotalSeconds * 3.1f;

            if(mouse.GetButtonState(MouseButtons.Left) == KeyBtnState.JustPressed)
            {
                lastMouse = mouse.Position;
            }

            if(mouse.GetButtonState(MouseButtons.Left) == KeyBtnState.Down)
            {
                yaw -= (lastMouse.X - mouse.Position.X)/ 100.0f;
                pitch -= (lastMouse.Y - mouse.Position.Y) / 100.0f;
                lastMouse = mouse.Position;
            }
        }
    }
}
