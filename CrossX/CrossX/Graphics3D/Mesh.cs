using CrossX.Data;
using CrossX.Graphics;
using XxIoC;
using System;
using System.Collections.Generic;


namespace CrossX.Graphics3D
{
    public enum TextureTarget
    {
        Diffuse,
        Normal,
        Specular
    }
    public delegate void LoadTextureDelegate(string name, TextureTarget target, out Texture2D texture, out bool managedOutside);

    public class Mesh: IDisposable
    {
        private bool disposed;

        public VertexBuffer Vertices { get; }
        public IList<MeshSlice> Slices { get; }
        public Aabb3 Bounds { get; }

        public Mesh(IObjectFactory objectFactory, RawMesh mesh, LoadTextureDelegate loadTextureDelegate)
        {
            Vertices = objectFactory.Create<VertexBuffer>(new VertexBufferCreationOptions
            {
                Count = mesh.Vertices.Length,
                VertexContent = VertexPNT.Content
            });
            Vertices.SetData(mesh.Vertices);

            Slices = new List<MeshSlice>();
            foreach(var slice in mesh.Slices)
            {
                Slices.Add(
                    objectFactory.Create<MeshSlice>(slice, loadTextureDelegate)
                    );
            }

            Bounds = mesh.Bounds;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            Vertices.Dispose();
            foreach(var slice in Slices)
            {
                slice.Dispose();
            }
            Slices.Clear();
        }
    }
}
