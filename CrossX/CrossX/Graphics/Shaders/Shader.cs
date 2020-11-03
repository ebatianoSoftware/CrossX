using System;

namespace CrossX.Graphics.Shaders
{
    public abstract class Shader: IDisposable
    {
        public abstract void Dispose();
        public abstract void SetConstData<T>(int slot, ref T data) where T : struct;
        public abstract void CreateConstBuffer<T>(int slot) where T : struct;
    }
}
