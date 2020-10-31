using System;

namespace CrossX.Graphics
{
    public abstract class SamplerState: IDisposable
    {
        public abstract SamplerStateDesc Desc { get; }
        public abstract void Dispose();
    }

    public struct SamplerStateDesc
    {
        public TextureFilter Filter { get; set; }
        public TextureMode AddressU { get; set; }
        public TextureMode AddressV { get; set; }
    }
}
