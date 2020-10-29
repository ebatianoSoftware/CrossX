using System;

namespace CrossX.Graphics
{
    public abstract class VertexBuffer: IDisposable
    {
        public abstract VertexContent VertexContent { get; }
        public abstract void SetData<T>(T[] vertices) where T : struct;

        public abstract void Dispose();
        public abstract int Count { get; }
    }
}
