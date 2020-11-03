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

        private readonly VertexShader pntVertexShader;
        private readonly PixelShader pntPixelShader;

        private Matrix viewProjectionMatrix;
        private Matrix worldMatrix;
        private readonly IGraphicsDevice graphicsDevice;

        public Color4 MaterialDiffuseColor { get; set; } = Color4.White;
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

        private VertexShader CreateVertexShader(string name, VertexContent vertexContent, IShadersRepository repository, IObjectFactory objectFactory)
        {
            var assembly = graphicsDevice.GetType().Assembly;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = repository.GetVertexShader(name);
            if (shader != null) return shader;

            shader = objectFactory.Create<VertexShader>(new CreateVertexShaderFromResource
            {
                Assembly = assembly,
                Path = name,
                VertexContent = vertexContent
            });

            repository.RegisterVertexShader(name, shader);
            return shader;
        }

        private PixelShader CreatePixelShader(string name, IShadersRepository repository, IObjectFactory objectFactory)
        {
            var assembly = graphicsDevice.GetType().Assembly;
            name = assembly.FullName.Split(',')[0] + ".FX." + name;

            var shader = repository.GetPixelShader(name);
            if (shader != null) return shader;

            shader = objectFactory.Create<PixelShader>(new CreatePixelShaderFromResource
            {
                Assembly = assembly,
                Path = name
            });

            repository.RegisterPixelShader(name, shader);
            return shader;
        }

        public void Apply()
        {
            VertexShader vs = pntVertexShader;
            PixelShader ps = pntPixelShader;

            var color = MaterialDiffuseColor;

            var consts = new ConstBuffer
            {
                MatrixWorldViewProj = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                MatrixWorld = worldMatrix,
                Color = new Vector4(color.Rf, color.Gf, color.Bf, color.Af),
                LightDir = new Vector4(-LightDir, 0)
            };

            //vs.ConstData = consts;

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
