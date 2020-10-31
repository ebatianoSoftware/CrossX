using CrossX.Graphics;
using SharpDX.Direct3D11;

namespace CrossX.DxCommon.Graphics
{
    internal class DxSamplerState: CrossX.Graphics.SamplerState
    {
        public SharpDX.Direct3D11.SamplerState State { get; }

        public override SamplerStateDesc Desc { get; }

        public DxSamplerState(Device1 device, SamplerStateDesc desc)
        {
            Desc = desc;
            var filter = Filter.MinMagMipPoint;

            switch(desc.Filter)
            {
                case TextureFilter.Linear:
                    filter = Filter.MinMagMipLinear;
                    break;

                case TextureFilter.Anisotropic:
                    filter = Filter.Anisotropic;
                    break;
            }

            State = new SharpDX.Direct3D11.SamplerState(device, new SamplerStateDescription
            {
                Filter = filter,
                AddressU = FromTextureMode(desc.AddressU),
                AddressV = FromTextureMode(desc.AddressV),
                AddressW = TextureAddressMode.Mirror,
                MipLodBias = 0,
                MaximumAnisotropy = 8,
                ComparisonFunction = Comparison.Always,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            });
        }

        private static TextureAddressMode FromTextureMode(TextureMode mode)
        {
            switch(mode)
            {
                case TextureMode.Clamp:
                    return TextureAddressMode.Clamp;

                case TextureMode.Mirror:
                    return TextureAddressMode.Mirror;
            }
            return TextureAddressMode.Wrap;
        }
    }
}
