using CrossX.Data;
using CrossX.Graphics.Shaders;
using CrossX.Graphics3D.Light;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public sealed class LightedEffect : Effect
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
            public Vector4 LightMask;
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
            public PointLight PointLight8;
            public PointLight PointLight9;
            public PointLight PointLight10;
            public PointLight PointLight11;
            public PointLight PointLight12;
            public PointLight PointLight13;
            public PointLight PointLight14;
            public PointLight PointLight15;
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

        public Vector4 AmbientColor { get; set; } = Color4.Black;
        public Color4 MaterialDiffuseColor { get; set; } = Color4.White;

        public Vector3 CameraPosition { get; set; }
        public Vector4 SpecularColor { get; set; }
        public float SpecularExponent { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D SpecularTexture { get; set; }
        private Texture2D whiteTexture;

        public float DiffuseLightMultiplier { get; set; } = 1;
        public float DiffuseLightForce { get; set; }
        public float ColorBias { get; set; }
        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public TextureSamplerDesc Sampler { get; set; }

        private readonly List<PointLight> pointLights = new List<PointLight>();
        private readonly List<SpotLight> spotLights = new List<SpotLight>();
        private readonly List<DirectionalLight> directionalLights = new List<DirectionalLight>();

        public LightedEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository): base(graphicsDevice, objectFactory, shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;

            var assembly = graphicsDevice.GetType().Assembly;

            pntVertexShader = CreateVertexShader<VertexShaderConst>(assembly, "LightedPNT", VertexPNT.Content);
            pntPixelShader = CreatePixelShader<BasicPixelShaderData, PointLightsData>(assembly, "LightedPNT");

            whiteTexture = objectFactory.Create<Texture2D>(
                new RawImage(1, 1, new byte[] 
                { 
                    255, 255, 255, 255,
                }, PixelDataFormat.Format32bppRGBA));
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
            if (pointLights.Count >= 15) throw new Exception($"Max 16 point lights.");
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
                Specular = new Vector4(SpecularColor.X, SpecularColor.Y, SpecularColor.Z, SpecularExponent),
                LightMask = new Vector4(DiffuseLightMultiplier, DiffuseLightForce, ColorBias, 0),
                DirectionalLight0 = directionalLights.Count > 0 ? directionalLights[0] : default,
                DirectionalLight1 = directionalLights.Count > 1 ? directionalLights[1] : default
            };

            ps.SetConstData(0, ref psConsts);

            var psPointLightsData = new PointLightsData
            {
                PointLight0 = pointLights.Count > 0 ? pointLights[0] : default,
                PointLight1 = pointLights.Count > 1 ? pointLights[1] : default,
                PointLight2 = pointLights.Count > 2 ? pointLights[2] : default,
                PointLight3 = pointLights.Count > 3 ? pointLights[3] : default,
                PointLight4 = pointLights.Count > 4 ? pointLights[4] : default,
                PointLight5 = pointLights.Count > 5 ? pointLights[5] : default,
                PointLight6 = pointLights.Count > 6 ? pointLights[6] : default,
                PointLight7 = pointLights.Count > 7 ? pointLights[7] : default,
                PointLight8 = pointLights.Count > 8 ? pointLights[8] : default,
                PointLight9 = pointLights.Count > 9 ? pointLights[9] : default,
                PointLight10 = pointLights.Count > 10 ? pointLights[10] : default,
                PointLight11 = pointLights.Count > 11 ? pointLights[11] : default,
                PointLight12 = pointLights.Count > 12 ? pointLights[12] : default,
                PointLight13 = pointLights.Count > 13 ? pointLights[13] : default,
                PointLight14 = pointLights.Count > 14 ? pointLights[14] : default,
                PointLight15 = pointLights.Count > 15 ? pointLights[15] : default,
            };
            ps.SetConstData(1, ref psPointLightsData);

            GraphicsDevice.SetShader(vs);
            GraphicsDevice.SetShader(ps);

            GraphicsDevice.SetPixelShaderSampler(0, Sampler);
            GraphicsDevice.SetPixelShaderTexture(0, Texture);
            GraphicsDevice.SetPixelShaderSampler(1, Sampler);
            GraphicsDevice.SetPixelShaderTexture(1, SpecularTexture ?? whiteTexture);
        }
    }
}
