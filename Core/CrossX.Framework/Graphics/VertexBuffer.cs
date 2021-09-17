using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class VertexBuffer: Disposable
    {
        public class Parameters
        {
            public Vector2[] Positions;
            public Color[] Colors;
            public Vector2[] TextureCoordinates;
        }

        public abstract int Length { get; }
    }
}