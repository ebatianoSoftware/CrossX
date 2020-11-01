using SharpDX.Direct3D11;

namespace CrossX.DxCommon.Graphics
{
    internal interface IDxTexture
    {
        ShaderResourceView ShaderResourceView { get; }
    }
}
