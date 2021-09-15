using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class Canvas: Disposable
    {
        public abstract Size Size { get; }
        public abstract int SaveState();
        public abstract void Restore();
        public abstract void Restore(int count);
        public abstract void Transform(Matrix3x2 transform);
        public abstract void ClipRect(RectangleF clip);
        public abstract void Clear(Color color);
        public abstract void DrawTriangles(VertexBuffer vertexBuffer, Image image);
        public abstract void DrawText(string text, Font font, RectangleF target, TextAlign align, Color color);
    }
}
