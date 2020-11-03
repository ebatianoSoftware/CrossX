using CrossX.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal class DxVertexShader : CrossX.Graphics.Shaders.VertexShader, IDxShader
    {
        private readonly VertexShader shader;
        public override VertexContent VertexContent { get; }
        private Dictionary<int, Buffer> buffers = new Dictionary<int, Buffer>();

        private readonly DxGraphicsDevice graphicsDevice;
        private readonly InputLayout inputLayout;

        public DxVertexShader(DxGraphicsDevice graphicsDevice, CrossX.Graphics.Shaders.CreateVertexShaderFromResource options)
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
        }

        public void AttachShader()
        {
            var context = graphicsDevice.D3dDevice.ImmediateContext1;
            context.VertexShader.Set(shader);
            context.InputAssembler.InputLayout = inputLayout;

            foreach (var buf in buffers)
            {
                context.VertexShader.SetConstantBuffer(buf.Key, buf.Value);
            }
        }

        public override void Dispose()
        {
            graphicsDevice.D3dDevice.ImmediateContext1.VertexShader.Set(null);

            foreach (var buf in buffers)
            {
                graphicsDevice.D3dDevice.ImmediateContext1.VertexShader.SetConstantBuffer(buf.Key, null);
                buf.Value.Dispose();
            }

            inputLayout.Dispose();
            shader.Dispose();
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
                list.Add(new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, offset, 0));
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
