using System;

namespace CrossX.Graphics.Shaders
{
    public struct EmptyConstData { }

    public abstract class Shader<TConstStruct>: IDisposable where TConstStruct: struct
    {
        public TConstStruct ConstData { get; set; }
        public abstract void Dispose();
    }
}
