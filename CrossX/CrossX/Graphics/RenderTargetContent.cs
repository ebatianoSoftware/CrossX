using System;

namespace CrossX.Graphics
{
    [Flags]
    public enum RenderTargetContent
    {
        Color = 1,
        Depth = 2,
        Both = 3
    }
}
