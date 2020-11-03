using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using XxPixelShader = CrossX.Graphics.Shaders.PixelShader;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal class DxPixelShader : XxPixelShader, IDxShader
    {
        private readonly PixelShader shader;

        private readonly DxGraphicsDevice graphicsDevice;
        private Dictionary<int, Buffer> buffers = new Dictionary<int, Buffer>();

        public DxPixelShader(DxGraphicsDevice graphicsDevice, CrossX.Graphics.Shaders.CreatePixelShaderFromResource options)
        {
            this.graphicsDevice = graphicsDevice;

            ShaderBytecode psCode = null;
            using (var stream = options.Assembly.GetManifestResourceStream(options.Path + ".psbc"))
            {
                psCode = ShaderBytecode.FromStream(stream);
            }
            
            shader = new PixelShader(graphicsDevice.D3dDevice, psCode);
        }

        public override void Dispose()
        {
            graphicsDevice.D3dDevice.ImmediateContext1.PixelShader.Set(null);
            
            foreach (var buf in buffers)
            {
                graphicsDevice.D3dDevice.ImmediateContext1.PixelShader.SetConstantBuffer(buf.Key, null);
                buf.Value.Dispose();
            }

            buffers.Clear();
            shader.Dispose();
        }

        public void AttachShader()
        {
            var context = graphicsDevice.D3dDevice.ImmediateContext1;
            context.PixelShader.Set(shader);

            foreach (var buf in buffers)
            {
                context.PixelShader.SetConstantBuffer(buf.Key, buf.Value);
            }
        }

        public override void SetConstData<T>(int slot, ref T data)
        {
            if (buffers.TryGetValue(slot, out var buffer))
            {
                var context = graphicsDevice.D3dDevice.ImmediateContext1;
                context.UpdateSubresource(ref data, buffer);
            }
        }

        public override void CreateConstBuffer<T>(int slot)
        {
            var size = Utilities.SizeOf<T>();

            int power = 1;
            while (power < size)
            {
                power *= 2;
            }

            var constBuffer = new Buffer(graphicsDevice.D3dDevice, power, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            buffers.Add(slot, constBuffer);
        }
    }
}
