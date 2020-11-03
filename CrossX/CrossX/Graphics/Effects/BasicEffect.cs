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

        public BasicEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;
            this.graphicsDevice = graphicsDevice;

            pcVertexShader = CreateVertexShader("DefaultPC", VertexPC.Content, shadersRepository, objectFactory);
            pctVertexShader = CreateVertexShader("DefaultPCT", VertexPCT.Content, shadersRepository, objectFactory);
            ptVertexShader = CreateVertexShader("DefaultPT", VertexPT.Content, shadersRepository, objectFactory);
            pntVertexShader = CreateVertexShader("DefaultPNT", VertexPNT.Content, shadersRepository, objectFactory);

            pcPixelShader = CreatePixelShader("DefaultPC", shadersRepository, objectFactory);
            pctPixelShader = CreatePixelShader("DefaultPCT", shadersRepository, objectFactory);
            ptPixelShader = CreatePixelShader("DefaultPT", shadersRepository, objectFactory);
            pntPixelShader = CreatePixelShader("DefaultPNT", shadersRepository, objectFactory);
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
            shader.CreateConstBuffer<ConstBuffer>(0);
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
