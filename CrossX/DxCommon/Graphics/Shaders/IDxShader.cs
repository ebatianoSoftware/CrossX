using CrossX.Graphics;

namespace CrossX.DxCommon.Graphics.Shaders
{
    internal interface IDxShader
    {
        DxEffect EffectForContent(VertexContent content);
    }
}
