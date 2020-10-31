namespace CrossX.Graphics
{
    public abstract class SamplerState
    {
        public abstract SamplerStateDesc Desc { get; }
    }

    public struct SamplerStateDesc
    {
        public TextureFilter Filter { get; set; }
        public TextureMode AddressU { get; set; }
        public TextureMode AddressV { get; set; }
    }
}
