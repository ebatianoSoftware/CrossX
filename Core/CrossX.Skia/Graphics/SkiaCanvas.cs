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

        public Vector2 CalculateTargetPosition(Font font, string line, RectangleF target, TextAlign textAlign, out Vector2 size)
        {
            var skiaFont = (SkiaFont)font;

            var skPaint = skiaFont.SKPaint;
            skPaint.IsStroke = false;
            var width = skPaint.MeasureText(line);

            var skRect = new SKRect();
            skPaint.MeasureText("X", ref skRect);

            var height = skRect.Height;
            float positionX = target.X;

            size = new Vector2(width, height);

            switch (textAlign & (TextAlign.Left | TextAlign.Center | TextAlign.Right))
            {
                case TextAlign.Right:
                    positionX = target.Right - width;
                    break;

                case TextAlign.Center:
                    positionX = target.Center.X - width / 2;
                    break;
            }

            float positionY = target.Top + height;

            switch (textAlign & (TextAlign.Top | TextAlign.Middle | TextAlign.Bottom))
            {
                case TextAlign.Bottom:
                    positionY = target.Bottom;
                    break;

                case TextAlign.Middle:
                    positionY = target.Center.Y - skRect.Top / 2;
                    break;
            }

            return new Vector2(positionX, positionY - height);
        }

        public override float DrawText(string text, Font font, Vector2 position, Color color, float calculatedHeight = 0)
        {
            if (font == null) return 0;

            var skiaFont = (SkiaFont)font;
            var skPaint = skiaFont.SKPaint;
            skPaint.IsStroke = false;
            skPaint.Color = color.ToSkia();

            if (calculatedHeight == 0)
            {
                var skRect = new SKRect();
                skPaint.MeasureText("X", ref skRect);
                calculatedHeight = skRect.Height;
            }

            var size = skPaint.MeasureText(text);
            skCanvas.DrawText(text, position.X, position.Y + calculatedHeight, skPaint);
            return size;
        }

        public override Vector2 DrawText(string line, Font font, RectangleF target, TextAlign textAlign, Color color)
        {
            if (font == null) return Vector2.Zero;
            var skiaFont = (SkiaFont)font;

            var position = CalculateTargetPosition(font, line, target, textAlign, out var size);

            var skPaint = skiaFont.SKPaint;
            skPaint.IsStroke = false;
            skPaint.Color = color.ToSkia();

            skCanvas.DrawText(line, position.X, position.Y + size.Y, skPaint);
            return size;
        }


        public override void FillRect(RectangleF rect, Color color)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = false;
            skCanvas.DrawRect(rect.ToSkia(), skPaint);
        }
    }
}
