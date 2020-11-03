using CrossX.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace CrossX.DxCommon.Graphics
{
    internal class DxIndexBuffer2 : IndexBuffer2
    {

        public override int Count => count;

        public Buffer Buffer { get; private set; }
        private readonly int count;
        private readonly DxGraphicsDevice graphicsDevice;

        public DxIndexBuffer2(DxGraphicsDevice graphicsDevice, int count)
        {
            this.graphicsDevice = graphicsDevice;
            this.count = count;
        }

        public override void Dispose()
        {
            Buffer.Dispose();
        }

        private void CreateBufferIfRequired()
        {
            if (Buffer != null && !Buffer.IsDisposed) return;
            Buffer?.Dispose();

            Buffer = new Buffer(graphicsDevice.D3dDevice, Count * 2, ResourceUsage.Dynamic, BindFlags.IndexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, 2);
        }

        public override void SetData(ushort[] data)
        {
            CreateBufferIfRequired();

            var d3dContext = graphicsDevice.D3dDevice.ImmediateContext1;
            lock (d3dContext)
            {
                var mode = MapMode.WriteDiscard;
                var dataBox = d3dContext.MapSubresource(Buffer, 0, mode, MapFlags.None);
                Utilities.Write(dataBox.DataPointer, data, 0, data.Length);
                d3dContext.UnmapSubresource(Buffer, 0);
            }
        }
    }

    internal class DxIndexBuffer4 : IndexBuffer4
    {
        public override int Count => count;

        public Buffer Buffer { get; private set; }
        private readonly int count;
        private readonly DxGraphicsDevice graphicsDevice;

        public DxIndexBuffer4(DxGraphicsDevice graphicsDevice, int count)
        {
            this.graphicsDevice = graphicsDevice;
            this.count = count;
        }

        public override void Dispose()
        {
            Buffer.Dispose();
        }

        private void CreateBufferIfRequired()
        {
            if (Buffer != null && !Buffer.IsDisposed) return;
            Buffer?.Dispose();

            Buffer = new Buffer(graphicsDevice.D3dDevice, Count * 4, ResourceUsage.Dynamic, BindFlags.IndexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, 4);
        }

        public override void SetData(uint[] data)
        {
            CreateBufferIfRequired();

            var d3dContext = graphicsDevice.D3dDevice.ImmediateContext1;
            lock (d3dContext)
            {
                var mode = MapMode.WriteDiscard;
                var dataBox = d3dContext.MapSubresource(Buffer, 0, mode, MapFlags.None);
                Utilities.Write(dataBox.DataPointer, data, 0, data.Length);
                d3dContext.UnmapSubresource(Buffer, 0);
            }
        }
    }
}
