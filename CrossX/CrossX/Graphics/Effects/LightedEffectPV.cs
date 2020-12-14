using CrossX.Graphics.Shaders;
using XxIoC;

namespace CrossX.Graphics.Effects
{
    public sealed class LightedEffectPV : LightedEffect
    {
        private readonly VertexShader pntVertexShader;
        private readonly PixelShader pntPixelShader;

        public LightedEffectPV(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IShadersRepository shadersRepository): base(graphicsDevice, objectFactory, shadersRepository)
        {
            var assembly = graphicsDevice.GetType().Assembly;

            pntVertexShader = CreateVertexShader<MaterialAndLightData, PointLightsData, SpotLightsData, VertexShaderConst>(assembly, "VertexLightedPNT", VertexPNT.Content);
            pntPixelShader = CreatePixelShader(assembly, "VertexLightedPNT");
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

            var mlDataConsts = GetMaterialAndLightsData();
            var pointLightsData = GetPointLightsData();

            vs.SetConstData(0, ref mlDataConsts);
            vs.SetConstData(1, ref pointLightsData);
            vs.SetConstData(3, ref vsConsts);

            GraphicsDevice.SetShader(vs);
            GraphicsDevice.SetShader(ps);

            GraphicsDevice.SetPixelShaderSampler(0, Sampler);
            GraphicsDevice.SetPixelShaderTexture(0, Texture);
            GraphicsDevice.SetPixelShaderSampler(1, Sampler);
            GraphicsDevice.SetPixelShaderTexture(1, SpecularTexture ?? whiteTexture);
        }
    }
}
