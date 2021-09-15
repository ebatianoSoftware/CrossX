namespace CrossX.Framework
{
    public struct Size
    {
        public readonly int Width;
        public readonly int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator SizeF(Size size) => new SizeF(size.Width, size.Height);

        public static readonly Size Empty = new Size(0, 0);
    }
}
