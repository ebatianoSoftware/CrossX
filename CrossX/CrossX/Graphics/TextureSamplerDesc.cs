using System;

namespace CrossX.Graphics
{
    [Flags]
    public enum TextureSamplerDesc
    {
        Point = 1,
        Linear = 2,
        Anisotropic = 3,

        WrapU = 0,
        WrapV = 0,

        ClampU = 4,
        ClampV = 8,

        MirrorU = 16,
        MirrorV = 32
    }
}
