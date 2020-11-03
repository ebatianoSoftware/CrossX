using CrossX.Data;
using CrossX.Graphics.Shaders;
using CrossX.Graphics3D.Light;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public sealed class LightedEffect : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VertexShaderConst
        {
            public Matrix MatrixWorldViewProj;
            public Matrix MatrixWorld;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BasicPixelShaderData
        {
            public Vector4 Ambient;
            public Vector4 MatDiffuse;
            public Vector4 CameraPosition;
            public Vector4 Specular;
            public DirectionalLight DirectionalLight0;
            public DirectionalLight DirectionalLight1;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct PointLightsData
        {
            public PointLight PointLight0;
            public PointLight PointLight1;
            public PointLight PointLight2;
            public PointLight PointLight3;
            public PointLight PointLight4;
            public PointLight PointLight5;
            public PointLight PointLight6;
            public PointLight PointLight7;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SpotLightsData
        {
            public SpotLight SpotLight0;
            public SpotLight SpotLight1;
            public SpotLight SpotLight2;
            public SpotLight SpotLight3;
        }

        private readonly VertexShader pntVertexShader;
        private readonly PixelShader pntPixelShader;

        private Matrix viewProjectionMatrix;
        private Matrix worldMatrix;
        private readonly IGraphicsDevice graphicsDevice;

        public Color4 AmbientColor { get; set; } = Color4.Black;
        public Color4 MaterialDiffuseColor { get; set; } = Color4.White;

        public Vector3 CameraPosition { get; set; }
        public Color4 SpecularColor { get; set; }
        public float SpecularExponent { get; set; }

        public Texture2D Texture { get; set; }
        public Texture2D SpecularTexture { get; set; }

        private Texture2D whiteTexture;

        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public TextureSamplerDesc Sampler { get; set; }

        private readonly List<PointLight> pointLights = new List<PointLight>();
        private readonly List<SpotLight> spotLights = new List<SpotLight>();
        private readonly List<DirectionalLight> directionalLights = new List<DirectionalLight>();

        public LightedEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;
            this.graphicsDevice = graphicsDevice;

            pntVertexShader = CreateVertexShader("LightedPNT", VertexPNT.Content, shadersRepository, objectFactory);
            pntPixelShader = CreatePixelShader("LightedPNT", shadersRepository, objectFactory);

            whiteTexture = objectFactory.Create<Texture2D>(
                new RawImage(2, 2, new byte[] 
                { 
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255
                }, PixelDataFormat.Format32bppRGBA));
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
            shader.CreateConstBuffer<VertexShaderConst>(0);
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
            shader.CreateConstBuffer<BasicPixelShaderData>(1);
            shader.CreateConstBuffer<PointLightsData>(2);
            return shader;
        }

        public void Reset()
        {
            pointLights.Clear();
            spotLights.Clear();
            directionalLights.Clear();
        }

        public void AddLight(DirectionalLight light)
        {
            if (directionalLights.Count >= 2) throw new Exception($"Max 2 directional lights.");
            light.Direction = -light.Direction;
            directionalLights.Add(light);
        }

        public void AddLight(PointLight light)
        {
            if (pointLights.Count >= 8) throw new Exception($"Max 8 point lights.");
            pointLights.Add(light);
        }

        public void AddLight(SpotLight light)
        {
            if (spotLights.Count >= 8) throw new Exception($"Max 8 spot lights.");
            light.Direction = -light.Direction;
            spotLights.Add(light);
        }

        public void Apply()
        {
            VertexShader vs = pntVertexShader;
            PixelShader ps = pntPixelShader;

            var vsConsts = new VertexShaderConst
            {
                MatrixWorldViewProj = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                MatrixWorld = worldMatrix,
            };

            vs.SetConstData(0, ref vsConsts);

            var psConsts = new BasicPixelShaderData
            {
                Ambient = AmbientColor,
                MatDiffuse = MaterialDiffuseColor,
                CameraPosition = new Vector4(CameraPosition, 1),
                Specular = new Vector4(SpecularColor.Rf, SpecularColor.Gf, SpecularColor.Bf, SpecularExponent),
                DirectionalLight0 = directionalLights.Count > 0 ? directionalLights[0] : default,
                DirectionalLight1 = directionalLights.Count > 1 ? directionalLights[1] : default
            };

            ps.SetConstData(1, ref psConsts);

            var psPointLightsData = new PointLightsData
            {
                PointLight0 = pointLights.Count > 0 ? pointLights[0] : default,
                PointLight1 = pointLights.Count > 1 ? pointLights[1] : default,
                PointLight2 = pointLights.Count > 2 ? pointLights[2] : default,
                PointLight3 = pointLights.Count > 3 ? pointLights[3] : default,
                PointLight4 = pointLights.Count > 4 ? pointLights[4] : default,
                PointLight5 = pointLights.Count > 5 ? pointLights[5] : default,
                PointLight6 = pointLights.Count > 6 ? pointLights[6] : default,
                PointLight7 = pointLights.Count > 7 ? pointLights[7] : default
            };
            ps.SetConstData(2, ref psPointLightsData);

            //    SpotLightsNumber = numSpotLights,
            //    SpotLight0 = numSpotLights > 0 ? spotLights[0] : default,
            //    SpotLight1 = numSpotLights > 1 ? spotLights[1] : default,
            //    SpotLight2 = numSpotLights > 2 ? spotLights[2] : default,
            //    SpotLight3 = numSpotLights > 3 ? spotLights[3] : default,
            //    //SpotLight4 = numSpotLights > 4 ? spotLights[4] : default,
            //    //SpotLight5 = numSpotLights > 5 ? spotLights[5] : default,
            //    //SpotLight6 = numSpotLights > 6 ? spotLights[6] : default,
            //    //SpotLight7 = numSpotLights > 7 ? spotLights[7] : default,
            //};

            graphicsDevice.SetShader(vs);
            graphicsDevice.SetShader(ps);

            graphicsDevice.SetPixelShaderSampler(0, Sampler);
            graphicsDevice.SetPixelShaderTexture(0, Texture);
            graphicsDevice.SetPixelShaderSampler(1, Sampler);
            graphicsDevice.SetPixelShaderTexture(1, SpecularTexture ?? whiteTexture);
        }

        public void Dispose()
        {
            
        }
    }
}
