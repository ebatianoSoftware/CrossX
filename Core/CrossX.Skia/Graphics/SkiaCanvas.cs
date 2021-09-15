using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System.Numerics;

namespace CrossX.Skia.Graphics
{
    internal class SkiaCanvas: Canvas, ISkiaCanvas
    {
        private SKCanvas skCanvas;
        private SKPaint skPaint = new SKPaint();
        private Size size;

        public override Size Size => size;

        public Canvas Canvas => this;

        public void Prepare(SKCanvas canvas, int width, int height)
        {
            skCanvas = canvas;
            size = new Size(width, height);
        }

        public override void Clear(Color color) => skCanvas.Clear(color.ToSkia());
        public override void ClipRect(RectangleF clip) => skCanvas.ClipRect(clip.ToSkia(), SKClipOperation.Intersect);
        public override void DrawTriangles(VertexBuffer vertexBuffer, Image image)
        {
            var vb = (SkiaVertexBuffer)vertexBuffer;
            var img = (SkiaImage)image;

            skPaint.Color = Color.White.ToSkia();
            skPaint.Shader = img.SKShader;

            skCanvas.DrawVertices(SKVertexMode.Triangles, vb.PositionBuffer, vb.TextureBuffer, vb.ColorBuffer, skPaint);
        }

        public override void Restore() => skCanvas.Restore();
        public override void Restore(int count) => skCanvas.RestoreToCount(count);
        public override int SaveState() => skCanvas.Save();
        public override void Transform(Matrix3x2 transform) => skCanvas.SetMatrix((transform * skCanvas.TotalMatrix.ToNumerics()).ToSkia());

        public override void DrawText(string text, Font font, RectangleF target, TextAlign align, Color color)
        {
            var fnt = (SkiaFont)font;
            skPaint.Color = color.ToSkia();
            skCanvas.DrawText(text, target.X, target.Y, fnt.SKFont, skPaint);
        }
    }
}
