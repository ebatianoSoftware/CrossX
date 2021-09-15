using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System.Numerics;

namespace CrossX.Skia.Graphics
{
    internal class SkiaVertexBuffer : VertexBuffer
    {
        private SKPoint[] positionBuffer;
        private SKPoint[] textureBuffer;
        private SKColor[] colorBuffer;

        public SKPoint[] PositionBuffer { get => positionBuffer; }
        public SKPoint[] TextureBuffer { get => textureBuffer; }
        public SKColor[] ColorBuffer { get => colorBuffer; }


        public override int Length { get; }

        public SkiaVertexBuffer(int length)
        {
            Length = length;
        }

        public override void SetColor(int index, Color color)
        {
            Utils.AllocBuffer(ref colorBuffer, Length);
            colorBuffer[index] = color.ToSkia();
        }

        public override void SetData(Vector2[] positions, Vector2[] texCoords, Color[] colors)
        {
            Utils.AllocBuffer(ref colorBuffer, Length);
            Utils.AllocBuffer(ref positionBuffer, Length);
            Utils.AllocBuffer(ref textureBuffer, Length);

            for(var idx =0; idx < Length; ++idx)
            {
                positionBuffer[idx] = positions[idx].ToSkia();
                textureBuffer[idx] = texCoords[idx].ToSkia();
                colorBuffer[idx] = colors[idx].ToSkia();
            }
        }

        public override void SetPosition(int index, Vector2 position)
        {
            Utils.AllocBuffer(ref positionBuffer, Length);
            positionBuffer[index] = position.ToSkia();
        }

        public override void SetTexCoord(int index, Vector2 texCoord)
        {
            Utils.AllocBuffer(ref textureBuffer, Length);
            textureBuffer[index] = texCoord.ToSkia();
        }
    }
}
