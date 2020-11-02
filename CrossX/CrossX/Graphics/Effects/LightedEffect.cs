using CrossX.Graphics.Shaders;
using CrossX.IoC;
using System;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public sealed class LightedEffect : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ConstBuffer
        {
            public Matrix MatrixWorldViewProj;
            public Matrix MatrixWorld;
            public Vector4 LightDir;
            public Vector4 Color;
        }

        private readonly VertexShader<ConstBuffer> pntVertexShader;
        private readonly PixelShader<EmptyConstData> pntPixelShader;

        private Matrix viewProjectionMatrix;
        private Matrix worldMatrix;
        private readonly IGraphicsDevice graphicsDevice;

        public float Alpha { get; set; } = 1;
        public Color4 DiffuseColor { get; set; } = Color4.White;

        public Vector3 LightDir { get; set; } = Vector3.Down;

        public Texture2D Texture { get; set; }
        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public TextureSamplerDesc Sampler { get; set; }

        public LightedEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;
            this.graphicsDevice = graphicsDevice;

            pntVertexShader = CreateVertexShader("LightedPNT", VertexPNT.Content, shadersRepository, objectFactory);
            pntPixelShader = CreatePixelShader("LightedPNT", shadersRepository, objectFactory);
            
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
            VertexShader<ConstBuffer> vs = pntVertexShader;
            PixelShader<EmptyConstData> ps = pntPixelShader;

            var color = DiffuseColor * Alpha;

            var consts = new ConstBuffer
            {
                MatrixWorldViewProj = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                MatrixWorld = worldMatrix,
                Color = new Vector4(color.Rf, color.Gf, color.Bf, color.Af) * Alpha,
                LightDir = new Vector4(-LightDir, 0)
            };

            vs.ConstData = consts;

            graphicsDevice.SetShader(vs);
            graphicsDevice.SetShader(ps);

            graphicsDevice.SetPixelShaderSampler(0, Sampler);
            graphicsDevice.SetPixelShaderTexture(0, Texture);
        }

        public void Dispose()
        {
            
        }
    }
}
