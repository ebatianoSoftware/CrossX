using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class VertexBuffer: Disposable
    {
        public abstract int Length { get; }

        public abstract void SetPosition(int index, Vector2 position);
        public abstract void SetTexCoord(int index, Vector2 position);
        public abstract void SetColor(int index, Color color);

        public abstract void SetData(Vector2[] positions, Vector2[] texCoords, Color[] colors);
    }
}
