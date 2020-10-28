// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using SharpDX;
using SharpDX.Direct3D11;

using SdxDevice1 = SharpDX.Direct3D11.Device1;

namespace CrossX.DxCommon.Graphics
{
    internal class DxVertexBuffer : VertexBuffer
    {
        private readonly SdxDevice1 device;
        public Buffer Buffer { get; private set; }

        public int Count { get; }
        public int Stride { get; }

        public VertexContent VertexContent { get; }

        public bool IsDisposed { get; private set; }

        public T UnderlyingObject<T>() where T: class => (T)(object)Buffer;

        private static int StrideFromVertexContent(VertexContent content)
        {
            int stride = 0;

            if (content.HasFlag(VertexContent.Position))
            {
                stride += sizeof(float) * 4;
            }

            if (content.HasFlag(VertexContent.Color))
            {
                stride += 4 * sizeof(byte);
            }

            if (content.HasFlag(VertexContent.TextureCoordinates))
            {
                stride += sizeof(float) * 2;
            }

            return stride;
        }

        public DxVertexBuffer(VertexBufferCreationOptions creationOptions, DxGraphicsDevice graphicsDevice)
        {
            var device = graphicsDevice.D3dDevice;

            Stride = StrideFromVertexContent(creationOptions.VertexContent);
            Count = creationOptions.Count;
            VertexContent = creationOptions.VertexContent;
            this.device = device;
        }

        public void Dispose()
        {
            
        }

        private void CreateBufferIfRequired()
        {
            if (Buffer != null && !Buffer.IsDisposed) return;
            Buffer?.Dispose();

            Buffer = new Buffer(device, Count * Stride, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, Stride);
        }

        public void SetData<T>(T[] data) where T: struct
        {
            CreateBufferIfRequired();

            var d3dContext = device.ImmediateContext1;
            lock (d3dContext)
            {
                var mode = MapMode.WriteDiscard;
                var dataBox = d3dContext.MapSubresource(Buffer, 0, mode, MapFlags.None);
                Utilities.Write(dataBox.DataPointer, data, 0, data.Length);
                d3dContext.UnmapSubresource(Buffer, 0);
            }
        }

        public void Recreate()
        {
            
        }

        public void Reset()
        {
            
        }
    }
}
