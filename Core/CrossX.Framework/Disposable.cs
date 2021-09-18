using System;

namespace CrossX.Framework.Graphics
{
    public abstract class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
        }
        
        ~Disposable()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
                IsDisposed = true;
            }
        }
    }
}