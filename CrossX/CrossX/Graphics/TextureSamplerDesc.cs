using System;

namespace CrossX.Graphics
{
    [Flags]
    public enum TextureSamplerDesc
    {
        Point = TextureFilter.Point,
        Linear = TextureFilter.Linear,
        Anisotropic = TextureFilter.Anisotropic,

        WrapU = TextureMode.WrapU,
        WrapV = TextureMode.WrapV,
        WrapUV = WrapU | WrapV,

        ClampU = TextureMode.ClampU,
        ClampV = TextureMode.ClampV,
        ClampUV = ClampU | ClampV,

        MirrorU = TextureMode.MirrorU,
        MirrorV = TextureMode.MirrorV,
        MirrorUV = MirrorU | MirrorV
            
    }
}
