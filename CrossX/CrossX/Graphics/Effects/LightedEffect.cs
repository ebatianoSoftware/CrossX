using CrossX.Data;
using CrossX.Graphics.Shaders;
using CrossX.Graphics3D.Light;
using XxIoC;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    public abstract class LightedEffect : Effect
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct VertexShaderConst
        {
            public Matrix MatrixWorldViewProj;
            public Matrix MatrixWorld;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct MaterialAndLightData
        {
            public Vector4 Ambient;
            public Vector4 MatDiffuse;
            public Vector4 CameraPosition;
            public Vector4 Specular;
            public DirectionalLight DirectionalLight0;
            public DirectionalLight DirectionalLight1;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct PointLightsData
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
        protected struct SpotLightsData
        {
            public SpotLight SpotLight0;
            public SpotLight SpotLight1;
            public SpotLight SpotLight2;
            public SpotLight SpotLight3;
        }

        protected Matrix viewProjectionMatrix;
        protected Matrix worldMatrix;

        public Vector4 AmbientColor { get; set; } = Color4.Black;
        public Color4 MaterialDiffuseColor { get; set; } = Color4.White;

        public Vector3 CameraPosition { get; set; }
        public Vector4 SpecularColor { get; set; }
        public float SpecularExponent { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D SpecularTexture { get; set; }
        protected readonly Texture2D whiteTexture;

        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;

        public TextureSamplerDesc Sampler { get; set; }

        protected readonly List<PointLight> pointLights = new List<PointLight>();
        protected readonly List<SpotLight> spotLights = new List<SpotLight>();
        protected readonly List<DirectionalLight> directionalLights = new List<DirectionalLight>();

        public LightedEffect(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository): base(graphicsDevice, objectFactory, shadersRepository)
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;

            var assembly = graphicsDevice.GetType().Assembly;
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

        public abstract void Apply();

        public override void Dispose()
        {
            base.Dispose();
            whiteTexture?.Dispose();
        }

        protected MaterialAndLightData GetMaterialAndLightsData()
        {
            return new MaterialAndLightData
            {
                Ambient = AmbientColor,
                MatDiffuse = MaterialDiffuseColor,
                CameraPosition = new Vector4(CameraPosition, 1),
                Specular = new Vector4(SpecularColor.X, SpecularColor.Y, SpecularColor.Z, SpecularExponent),
                DirectionalLight0 = directionalLights.Count > 0 ? directionalLights[0] : default,
                DirectionalLight1 = directionalLights.Count > 1 ? directionalLights[1] : default
            };
        }

        protected PointLightsData GetPointLightsData()
        {
            return new PointLightsData
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
        }
    }
}
