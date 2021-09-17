using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System.Linq;
using System.Numerics;

namespace CrossX.Skia.Graphics
{
    internal class SkiaVertexBuffer : VertexBuffer
    {
        public override int Length { get; }

        public SKVertices SKVertices { get; }

        public SkiaVertexBuffer(Parameters parameters)
        {
            Length = parameters.Positions.Length;
            if (parameters.TextureCoordinates == null)
            {
                SKVertices = SKVertices.CreateCopy(SKVertexMode.Triangles,
                    parameters.Positions.Select(o => o.ToSkia()).ToArray(),
                    parameters.Colors.Select(o => o.ToSkia()).ToArray()
                    );
            }
            else
            {
                SKVertices = SKVertices.CreateCopy(SKVertexMode.Triangles,
                    parameters.Positions.Select(o => o.ToSkia()).ToArray(),
                    parameters.TextureCoordinates.Select(o => o.ToSkia()).ToArray(),
                    parameters.Colors.Select(o => o.ToSkia()).ToArray()
                    );
            }
        }
    }
}
