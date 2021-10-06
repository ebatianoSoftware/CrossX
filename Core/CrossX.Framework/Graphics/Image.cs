namespace CrossX.Framework.Graphics
{
    public abstract class Image: Disposable
    {
        public abstract Size Size { get; }

        public abstract Image Scale(float scale);
    }
}
