// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpDX;
using SharpDX.Direct3D11;

namespace CrossX.DxCommon.Graphics
{
    internal class RenderStates
    {
        private readonly Device1 _d3dDevice;

        public RasterizerState1 RasterizerState { get; private set; }
        public RasterizerState1 ClipRasterizerState { get; private set; }

        public BlendState1 NoBlendState { get; private set; }
        public BlendState1 AlphaBlendState { get; private set; }
        public BlendState1 AddBlendState { get; private set; }
        public BlendState1 MultiplyBlendState { get; internal set; }

        public SamplerState NearestSamplerState { get; private set; }
        public SamplerState LinearSamplerState { get; private set; }
        public SamplerState AnisotropicSamplerState { get; internal set; }

        public RenderStates(DxGraphicsDevice device)
        {
            _d3dDevice = device.D3dDevice;
        }

        public void Initialize()
        {
            InitBlendStates();
            InitRasterizerStates();
            InitSamplerStates();
        }

        private void InitBlendStates()
        {
            var blendStateDesc = BlendStateDescription1.Default();
            blendStateDesc.RenderTarget[0].IsBlendEnabled = false;
            NoBlendState = new BlendState1(_d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;

            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            AlphaBlendState = new BlendState1(_d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
            AddBlendState = new BlendState1(_d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.Zero;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.SourceColor;

            MultiplyBlendState = new BlendState1(_d3dDevice, blendStateDesc);
        }

        private void InitSamplerStates()
        {
            var samplerDesc = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Mirror,
                AddressV = TextureAddressMode.Mirror,
                AddressW = TextureAddressMode.Mirror,
                MipLodBias = 0,
                MaximumAnisotropy = 8,
                ComparisonFunction = Comparison.Always,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            NearestSamplerState = new SamplerState(_d3dDevice, samplerDesc);

            samplerDesc.Filter = Filter.MinMagMipLinear;
            LinearSamplerState = new SamplerState(_d3dDevice, samplerDesc);

            samplerDesc.Filter = Filter.Anisotropic;
            AnisotropicSamplerState = new SamplerState(_d3dDevice, samplerDesc);
        }

        private void InitRasterizerStates()
        {
            var rasterizerDesc = new RasterizerStateDescription1
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0,
            };

            RasterizerState = new RasterizerState1(_d3dDevice, rasterizerDesc);

            rasterizerDesc.IsScissorEnabled = true;
            ClipRasterizerState = new RasterizerState1(_d3dDevice, rasterizerDesc);
        }

    }
}
