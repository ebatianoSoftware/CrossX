using CrossX.Graphics.Shaders;
using CrossX.IoC;
using System;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public sealed class BasicEffect: IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ConstBuffer
        {
            public Matrix Matrix;
            public Vector4 Color;
        }

        private Matrix viewProjectionMatrix;
        private Matrix worldMatrix;
        private readonly IGraphicsDevice graphicsDevice;

        public float Alpha { get; set; } = 1;
        public Color4 DiffuseColor { get; set; } = Color4.White;
        public Texture2D Texture { get; set; }
        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public bool VertexColorEnabled { get; set; }
        public bool TextureEnabled { get; set; }

        public TextureSamplerDesc Sampler { get; set; }

        private readonly VertexShader<ConstBuffer> pcVertexShader;
        private readonly VertexShader<ConstBuffer> ptVertexShader;
        private readonly VertexShader<ConstBuffer> pctVertexShader;

        private readonly PixelShader<EmptyConstData> pcPixelShader;
        private readonly PixelShader<EmptyConstData> ptPixelShader;
        private readonly PixelShader<EmptyConstData> pctPixelShader;

        public BasicEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;
            this.graphicsDevice = graphicsDevice;

            pcVertexShader = CreateVertexShader("DefaultPC", VertexPC.Content, shadersRepository, objectFactory);
            pctVertexShader = CreateVertexShader("DefaultPCT", VertexPCT.Content, shadersRepository, objectFactory);
            ptVertexShader = CreateVertexShader("DefaultPT", VertexPT.Content, shadersRepository, objectFactory);

            pcPixelShader = CreatePixelShader("DefaultPC", shadersRepository, objectFactory);
            pctPixelShader = CreatePixelShader("DefaultPCT", shadersRepository, objectFactory);
            ptPixelShader = CreatePixelShader("DefaultPT", shadersRepository, objectFactory);
        }

        private VertexShader<ConstBuffer> CreateVertexShader(string name, VertexContent vertexContent, IShadersRepository repository, IObjectFactory objectFactory)
        {
            var assembly = graphicsDevice.GetType().Assembly;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = repository.GetVertexShader<ConstBuffer>(name);
            if (shader != null) return shader;

            shader = objectFactory.Create<VertexShader<ConstBuffer>>(new CreateVertexShaderFromResource
            {
                Assembly = assembly,
                Path = name,
                VertexContent = vertexContent
            });

            repository.RegisterVertexShader(name, shader);
            return shader;
        }

        private PixelShader<EmptyConstData> CreatePixelShader(string name, IShadersRepository repository, IObjectFactory objectFactory)
        {
            var assembly = graphicsDevice.GetType().Assembly;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = repository.GetPixelShader<EmptyConstData>(name);
            if (shader != null) return shader;

            shader = objectFactory.Create<PixelShader<EmptyConstData>>(new CreatePixelShaderFromResource
            {
                Assembly = assembly,
                Path = name
            });

            repository.RegisterPixelShader(name, shader);
            return shader;
        }

        public void Apply()
        {
            VertexShader<ConstBuffer> vs = null;
            PixelShader<EmptyConstData> ps = null;

            if (VertexColorEnabled)
            {
                vs = TextureEnabled ? pctVertexShader : pcVertexShader;
                ps = TextureEnabled ? pctPixelShader : pcPixelShader;
            }
            else
            {
                if (!TextureEnabled) throw new InvalidOperationException();
                vs = ptVertexShader;
                ps = ptPixelShader;
            }

            var color = DiffuseColor * Alpha;

            var consts = new ConstBuffer
            {
                Matrix = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                Color = new Vector4(color.Rf, color.Gf, color.Bf, color.Af)
            };

            vs.ConstData = consts;

            graphicsDevice.SetShader(vs);
            graphicsDevice.SetShader(ps);

            if (TextureEnabled)
            {
                graphicsDevice.SetPixelShaderSampler(0, Sampler);
                graphicsDevice.SetPixelShaderTexture(0, Texture);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
