using System;

namespace CrossX.Graphics
{
    public abstract class Texture2D: IDisposable
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public virtual void Dispose()
        {
            
        }
    }
}
