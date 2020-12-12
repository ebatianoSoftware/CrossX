using CrossX.Graphics.Shaders;
using S2IoC;

namespace CrossX.Graphics.Effects
{
    public sealed class LightedEffectPP : LightedEffect
    {
        private readonly VertexShader pntVertexShader;
        private readonly PixelShader pntPixelShader;

        public LightedEffectPP(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository): base(graphicsDevice, objectFactory, shadersRepository)
        {
            var assembly = graphicsDevice.GetType().Assembly;

            pntVertexShader = CreateVertexShader<VertexShaderConst>(assembly, "LightedPNT", VertexPNT.Content);
            pntPixelShader = CreatePixelShader<MaterialAndLightData, PointLightsData>(assembly, "LightedPNT");
        }

        
        public override void Apply()
        {
            VertexShader vs = pntVertexShader;
            PixelShader ps = pntPixelShader;

            var vsConsts = new VertexShaderConst
            {
                MatrixWorldViewProj = Matrix.Multiply(worldMatrix, viewProjectionMatrix),
                MatrixWorld = worldMatrix,
            };

            vs.SetConstData(0, ref vsConsts);

            var psConsts = GetMaterialAndLightsData();
            ps.SetConstData(0, ref psConsts);

            var psPointLightsData = GetPointLightsData();
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
