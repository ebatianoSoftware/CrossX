namespace CrossX.Graphics
{
    public abstract class RasterizerState
    {
        public abstract RasterizerStateDesc Desc { get; }
    }

    public struct RasterizerStateDesc
    {
        public CullMode CullMode { get; set; }
        public bool IsDepthClipEnabled { get; set; }
        public bool IsScissorEnabled { get; set; }
    }
}
