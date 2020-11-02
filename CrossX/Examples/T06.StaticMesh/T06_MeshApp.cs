using CrossX;
using CrossX.Core;
using CrossX.Data;
using CrossX.Graphics;
using CrossX.Graphics.Effects;
using CrossX.Graphics3D;
using CrossX.IoC;
using CrossX.Media.Formats;
using System;

namespace T06.StaticMesh
{
    public class T06_MeshApp : IApp
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private Mesh mesh;
        private LightedEffect lightedEffect;
        private float rotation = 0;
        public T06_MeshApp(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory)
        {
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
        }

        public void LoadContent()
        {
            RawMesh rawMesh = null;
            using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream("T06.StaticMesh.fighterjet.obj"))
            {
                rawMesh = WavefrontObjFormat.Instance.FromStream(stream);
            }

            lightedEffect = objectFactory.Create<LightedEffect>();
            mesh = objectFactory.Create<Mesh>(rawMesh, new LoadMeshTexturesDelegate(LoadMeshTextures));
        }

        private void LoadMeshTextures(string name, out Texture2D texture, out Texture2D normal, out bool managedOutside)
        {
            using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream("T06.StaticMesh.diffuse.png"))
            {
                texture = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
            }
            using (var stream = typeof(T06_MeshApp).Assembly.GetManifestResourceStream("T06.StaticMesh.normals.png"))
            {
                normal = objectFactory.Create<Texture2D>(stream, ImagesFormat.Instance);
            }
            managedOutside = false;
        }

        public void Draw(TimeSpan frameTime)
        {
            graphicsDevice.Clear(Color4.CornflowerBlue);

            lightedEffect.SetWorldTransform(
                Matrix.CreateTranslation(-mesh.Bounds.Center) *
                Matrix.CreateRotationY(rotation));

            var dist = MathHelper.Max(mesh.Bounds.Width, mesh.Bounds.Height) * 3;

            var projView = Matrix.CreateLookAt(
                    new Vector3(1, 1, 1).Normalized() * dist,
                    Vector3.Zero,
                    Vector3.Up) *
                    Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3f, (float)graphicsDevice.CurrentTargetSize.Width / graphicsDevice.CurrentTargetSize.Height, 0.1f, dist * 2);

            lightedEffect.SetViewProjectionTransform(projView);
            lightedEffect.LightDir = new Vector3(1, -1, 0).Normalized();
            graphicsDevice.DepthClip = true;
            for (var idx = 0; idx < mesh.Slices.Count; ++idx)
            {
                var slice = mesh.Slices[idx];
                lightedEffect.Texture = slice.Texture;
                lightedEffect.Apply();

                graphicsDevice.SetIndexBuffer(slice.Indices);
                graphicsDevice.SetVertexBuffer(mesh.Vertices);

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, slice.Indices.Count);
            }

            graphicsDevice.Present();
        }

        public void Update(TimeSpan frameTime)
        {
            rotation += (float)frameTime.TotalSeconds * 2;
        }
    }
}
