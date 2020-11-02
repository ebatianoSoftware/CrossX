using CrossX.Data;
using CrossX.Graphics;
using CrossX.IoC;
using System;

namespace CrossX.Graphics3D
{
    public class MeshSlice: IDisposable
    {
        private readonly bool diffManagedOutside;
        private readonly bool normalManagedOutside;
        public RawMaterial Material { get; }
        public Texture2D Texture { get; }
        public Texture2D NormalMap { get; }
        public IndexBuffer Indices { get; }

        public MeshSlice(IObjectFactory objectFactory, RawMeshSlice slice, LoadTextureDelegate loadTextureDelegate)
        {
            Texture2D texture = null;
            Texture2D normal = null;

            loadTextureDelegate?.Invoke(slice.Material.DiffuseMap, TextureTarget.Diffuse, out texture, out diffManagedOutside);
            loadTextureDelegate?.Invoke(slice.Material.NormalMap, TextureTarget.Normal, out normal, out normalManagedOutside);
            
            Texture = texture;
            NormalMap = normal;

            if (slice.Indices2 != null)
            {
                var buffer = objectFactory.Create<IndexBuffer2>(slice.Indices2.Length);
                buffer.SetData(slice.Indices2);
                Indices = buffer;
            }
            else
            {
                var buffer = objectFactory.Create<IndexBuffer4>(slice.Indices4.Length);
                buffer.SetData(slice.Indices4);
                Indices = buffer;
            }
        }

        public void Dispose()
        {
            if (!diffManagedOutside)
            {
                Texture?.Dispose();
            }

            if (!normalManagedOutside)
            {
                NormalMap?.Dispose();
            }
            Indices.Dispose();
        }
    }
}
