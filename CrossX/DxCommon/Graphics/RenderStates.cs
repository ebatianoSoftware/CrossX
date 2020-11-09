// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace CrossX.DxCommon.Graphics
{
    internal class RenderStates
    {
        private readonly Device1 d3dDevice;

        public DepthStencilState NoClipDepthStencilState { get; private set; }
        public DepthStencilState DepthStencilState { get; private set; }
        public RasterizerState1 RasterizerState { get; private set; }
        public RasterizerState1 ClipRasterizerState { get; private set; }

        public RasterizerState1 DepthRasterizerState { get; private set; }
        public RasterizerState1 DepthClipRasterizerState { get; private set; }

        public BlendState1 NoBlendState { get; private set; }
        public BlendState1 AlphaBlendState { get; private set; }
        public BlendState1 AddBlendState { get; private set; }
        public BlendState1 MultiplyBlendState { get; internal set; }

        private Dictionary<TextureSamplerDesc, SamplerState> samplerStates = new Dictionary<TextureSamplerDesc, SamplerState>();

        public RenderStates(DxGraphicsDevice device)
        {
            d3dDevice = device.D3dDevice;
        }

        public void Initialize()
        {
            InitBlendStates();
            InitRasterizerStates();
            InitDepthStencilStates();
        }

        private void InitDepthStencilStates()
        {
            var stateDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                IsStencilEnabled = true,
                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.All,
                StencilWriteMask = (byte)DepthWriteMask.All,
                FrontFace = new DepthStencilOperationDescription 
                {
                    Comparison = Comparison.Always,
                    DepthFailOperation = StencilOperation.Increment,
                    FailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep
                },
                BackFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Always,
                    DepthFailOperation = StencilOperation.Decrement,
                    FailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep
                }
            };

            DepthStencilState = new DepthStencilState(d3dDevice, stateDesc);
            stateDesc.IsStencilEnabled = false;
            stateDesc.IsDepthEnabled = false;



            NoClipDepthStencilState = new DepthStencilState(d3dDevice, stateDesc);
        }

        private void InitBlendStates()
        {
            var blendStateDesc = BlendStateDescription1.Default();
            blendStateDesc.RenderTarget[0].IsBlendEnabled = false;
            NoBlendState = new BlendState1(d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;

            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;

            blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
            blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;

            blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            AlphaBlendState = new BlendState1(d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
            AddBlendState = new BlendState1(d3dDevice, blendStateDesc);

            blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.Zero;
            blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.SourceColor;

            MultiplyBlendState = new BlendState1(d3dDevice, blendStateDesc);
        }

        public SamplerState GetSampler(TextureSamplerDesc sampler)
        {
            if (samplerStates.TryGetValue(sampler, out var samplerState)) return samplerState;

            var filter = Filter.MinMagMipPoint;
            switch( (TextureFilter)((int)sampler & 0x03))
            {
                case TextureFilter.Linear:
                    filter = Filter.MinMagMipLinear;
                    break;

                case TextureFilter.Anisotropic:
                    filter = Filter.Anisotropic;
                    break;

            }

            var addressU = TextureAddressMode.Wrap;
            var addressV = TextureAddressMode.Wrap;

            if (sampler.HasFlag(TextureSamplerDesc.ClampU)) addressU = TextureAddressMode.Clamp;
            else if (sampler.HasFlag(TextureSamplerDesc.MirrorU)) addressU = TextureAddressMode.Mirror;

            if (sampler.HasFlag(TextureSamplerDesc.ClampV)) addressV = TextureAddressMode.Clamp;
            else if (sampler.HasFlag(TextureSamplerDesc.MirrorV)) addressV = TextureAddressMode.Mirror;

            var samplerDesc = new SamplerStateDescription
            {
                Filter = filter,
                AddressU = addressU,
                AddressV = addressV,
                AddressW = TextureAddressMode.Mirror,
                MipLodBias = 0,
                MaximumAnisotropy = 8,
                ComparisonFunction = Comparison.Always,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            samplerState = new SamplerState(d3dDevice, samplerDesc);
            samplerStates.Add(sampler, samplerState);
            return samplerState;
        }

        private void InitRasterizerStates()
        {
            var rasterizerDesc = new RasterizerStateDescription1
            {
                CullMode = SharpDX.Direct3D11.CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0,
            };

            RasterizerState = new RasterizerState1(d3dDevice, rasterizerDesc);

            rasterizerDesc.IsScissorEnabled = true;
            ClipRasterizerState = new RasterizerState1(d3dDevice, rasterizerDesc);

            rasterizerDesc.IsDepthClipEnabled = true;
            rasterizerDesc.IsScissorEnabled = false;
            DepthRasterizerState = new RasterizerState1(d3dDevice, rasterizerDesc);

            rasterizerDesc.IsScissorEnabled = true;
            DepthClipRasterizerState = new RasterizerState1(d3dDevice, rasterizerDesc);
        }

    }
}
