using CrossX.Graphics.Shaders;
using XxIoC;
using System;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public sealed class BasicEffect: Effect
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ConstBuffer
        {
            public Matrix Matrix;
            public Vector4 Color;
        }

        private Matrix viewProjectionMatrix;
        private Matrix worldMatrix;

        public float Alpha { get; set; } = 1;
        public Color4 DiffuseColor { get; set; } = Color4.White;
        public Texture2D Texture { get; set; }
        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public bool VertexColorEnabled { get; set; }
        public bool TextureEnabled { get; set; }
        public bool VertexHasNormals { get; set; }

        public TextureSamplerDesc Sampler { get; set; }

        private readonly VertexShader pcVertexShader;
        private readonly VertexShader ptVertexShader;
        private readonly VertexShader pctVertexShader;
        private readonly VertexShader pntVertexShader;

        private readonly PixelShader pcPixelShader;
        private readonly PixelShader ptPixelShader;
        private readonly PixelShader pctPixelShader;
        private readonly PixelShader pntPixelShader;

        public BasicEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository): base(graphicsDevice, objectFactory, shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;

            var assembly = graphicsDevice.GetType().Assembly;

            pcVertexShader = CreateVertexShader<ConstBuffer>(assembly, "DefaultPC", VertexPC.Content);
            pctVertexShader = CreateVertexShader<ConstBuffer>(assembly, "DefaultPCT", VertexPCT.Content);
            ptVertexShader = CreateVertexShader<ConstBuffer>(assembly, "DefaultPT", VertexPT.Content);
            pntVertexShader = CreateVertexShader<ConstBuffer>(assembly, "DefaultPNT", VertexPNT.Content);

            pcPixelShader = CreatePixelShader(assembly, "DefaultPC");
            pctPixelShader = CreatePixelShader(assembly, "DefaultPCT");
            ptPixelShader = CreatePixelShader(assembly, "DefaultPT");
            pntPixelShader = CreatePixelShader(assembly, "DefaultPNT");
        }

        public void Apply()
        {
            VertexShader vs = null;
            PixelShader ps = null;

            if (VertexColorEnabled)
            {
                vs = TextureEnabled ? pctVertexShader : pcVertexShader;
                ps = TextureEnabled ? pctPixelShader : pcPixelShader;
            }
            else
            {
                if (!TextureEnabled) throw new InvalidOperationException();

                if (VertexHasNormals)
                {
                    vs = pntVertexShader;
                    ps = pntPixelShader;
                }
                else
                {
                    vs = ptVertexShader;
                    ps = ptPixelShader;
                }
            }

            var color = DiffuseColor * Alpha;
            var consts = new ConstBuffer
            {
                Matrix = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                Color = color * Alpha
            };

            vs.SetConstData(0, ref consts);

            GraphicsDevice.SetShader(vs);
            GraphicsDevice.SetShader(ps);

            if (TextureEnabled)
            {
                GraphicsDevice.SetPixelShaderSampler(0, Sampler);
                GraphicsDevice.SetPixelShaderTexture(0, Texture);
            }
        }
    }
}
