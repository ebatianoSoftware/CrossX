using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.IO;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal class DxPixelShader<TConstStruct> : PixelShader<TConstStruct>, IDxShader where TConstStruct : struct
    {
        private readonly PixelShader shader;

        private readonly DxGraphicsDevice graphicsDevice;
        private Buffer constBuffer;

        public DxPixelShader(DxGraphicsDevice graphicsDevice, CreatePixelShaderFromResource options)
        {
            this.graphicsDevice = graphicsDevice;

            ShaderBytecode psCode = null;
            using (var stream = options.Assembly.GetManifestResourceStream(options.Path + ".psbc"))
            {
                psCode = ShaderBytecode.FromStream(stream);
            }
            
            shader = new PixelShader(graphicsDevice.D3dDevice, psCode);

            var size = Utilities.SizeOf<TConstStruct>();
            if (size > 1)
            {
                constBuffer = new Buffer(graphicsDevice.D3dDevice, size, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }
        }

        public override void Dispose()
        {
            shader.Dispose();

            constBuffer?.Dispose();
            constBuffer = null;
        }

        public void AttachShader()
        {
            var context = graphicsDevice.D3dDevice.ImmediateContext1;

            context.PixelShader.Set(shader);

            if (constBuffer != null)
            {
                context.VertexShader.SetConstantBuffer(0, constBuffer);
                var consts = ConstData;
                context.UpdateSubresource(ref consts, constBuffer);
            }
        }
    }
}
