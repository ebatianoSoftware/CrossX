using System;

namespace CrossX.Graphics
{
    public abstract class IndexBuffer: IDisposable
    {
        public abstract int Count { get; }
        public abstract void Dispose();
    }

    public abstract class IndexBuffer2: IndexBuffer
    {
        public abstract void SetData(ushort[] indices);
    }

    public abstract class IndexBuffer4 : IndexBuffer
    {
        public abstract void SetData(uint[] indices);
    }
}
