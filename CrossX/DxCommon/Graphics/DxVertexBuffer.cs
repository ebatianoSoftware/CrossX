// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.DxCommon.Helpers;
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

        public override int Count { get; }
        public int Stride { get; }

        public override VertexContent VertexContent { get; }

        public bool IsDisposed { get; private set; }

        
        public DxVertexBuffer(VertexBufferCreationOptions creationOptions, DxGraphicsDevice graphicsDevice)
        {
            var device = graphicsDevice.D3dDevice;

            Stride = GeometryExtensions.StrideFromVertexContent(creationOptions.VertexContent);
            Count = creationOptions.Count;
            VertexContent = creationOptions.VertexContent;
            this.device = device;
        }

        private void CreateBufferIfRequired()
        {
            if (Buffer != null && !Buffer.IsDisposed) return;
            Buffer?.Dispose();

            Buffer = new Buffer(device, Count * Stride, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, Stride);
        }

        public override void SetData<T>(T[] data)
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

        public override void Dispose()
        {
            Buffer?.Dispose();
            Buffer = null;
        }
    }
}
