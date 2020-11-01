using CrossX.Graphics;
using CrossX.Graphics.Shaders;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.IO;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal class DxVertexShader<TConstStruct> : VertexShader<TConstStruct>, IDxShader where TConstStruct : struct
    {
        private readonly VertexShader shader;
        public override VertexContent VertexContent { get; }

        private readonly DxGraphicsDevice graphicsDevice;

        private Buffer constBuffer;

        private readonly InputLayout inputLayout;

        public DxVertexShader(DxGraphicsDevice graphicsDevice, CreateVertexShaderFromResource options)
        {
            this.graphicsDevice = graphicsDevice;

            ShaderBytecode vsCode = null;
            using (var stream = options.Assembly.GetManifestResourceStream(options.Path + ".vsbc"))
            {
                vsCode = ShaderBytecode.FromStream(stream);
            }
            VertexContent = options.VertexContent;

            var elements = ElementsFromVertexContent(VertexContent);
            
            inputLayout = new InputLayout(graphicsDevice.D3dDevice, vsCode, elements);
            shader = new VertexShader(graphicsDevice.D3dDevice, vsCode);

            var size = Utilities.SizeOf<TConstStruct>();
            if (size > 0)
            {
                constBuffer = new Buffer(graphicsDevice.D3dDevice, size, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }
        }

        public void AttachShader()
        {
            var context = graphicsDevice.D3dDevice.ImmediateContext1;
            context.VertexShader.Set(shader);
            context.InputAssembler.InputLayout = inputLayout;

            if (constBuffer != null)
            {
                context.VertexShader.SetConstantBuffer(0, constBuffer);

                var consts = ConstData;
                context.UpdateSubresource(ref consts, constBuffer);
            }
        }

        public override void Dispose()
        {
            inputLayout.Dispose();
            shader.Dispose();

            constBuffer?.Dispose();
            constBuffer = null;
        }

        private static InputElement[] ElementsFromVertexContent(VertexContent content)
        {
            var list = new List<InputElement>();
            var offset = 0;
            if (content.HasFlag(VertexContent.Position))
            {
                list.Add(new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0));
                offset += 16;
            }

            if (content.HasFlag(VertexContent.Normal))
            {
                list.Add(new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0));
                offset += 16;
            }

            if (content.HasFlag(VertexContent.Color))
            {
                list.Add(new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, offset, 0));
                offset += 4;
            }

            if (content.HasFlag(VertexContent.TextureCoordinates))
            {
                list.Add(new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, offset, 0));
                offset += 8;
            }

            return list.ToArray();
        }
    }
}
