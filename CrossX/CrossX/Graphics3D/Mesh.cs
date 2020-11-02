using CrossX.Data;
using CrossX.Graphics;
using CrossX.IoC;
using System;
using System.Collections.Generic;

namespace CrossX.Graphics3D
{
    public delegate void LoadMeshTexturesDelegate(string name, out Texture2D texture, out Texture2D normal, out bool managedOutside );

    public class Mesh: IDisposable
    {
        private bool disposed;

        public VertexBuffer Vertices { get; }
        public IList<MeshSlice> Slices { get; }
        public Aabb3 Bounds { get; }

        public Mesh(IObjectFactory objectFactory, RawMesh mesh, LoadMeshTexturesDelegate loadTexturesDelegate)
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
                    objectFactory.Create<MeshSlice>(slice, loadTexturesDelegate)
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
