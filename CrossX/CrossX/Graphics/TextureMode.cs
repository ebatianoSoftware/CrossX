using System;

namespace CrossX.Graphics
{
    [Flags]
    public enum TextureMode
    {
        WrapU = 0,
        WrapV = 0,
        WrapUV = WrapU | WrapV,

        ClampU = 4,
        ClampV = 8,
        ClampUV = ClampU | ClampV,

        MirrorU = 16,
        MirrorV = 32,
        MirrorUV = MirrorU | MirrorV
    }
}