using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System;
using System.Numerics;

namespace CrossX.Skia.Graphics
{
    internal class SkiaCanvas : Canvas, ISkiaCanvas
    {
        private SKCanvas skCanvas;
        private SKPaint skPaint = new SKPaint
        {
            IsAntialias = true,
            SubpixelText = true,
            FilterQuality = SKFilterQuality.High
        };

        private Size size;
        public override Size Size => size;

        public Canvas Canvas => this;

        public void Prepare(SKCanvas canvas, int width, int height)
        {
            skCanvas = canvas;
            size = new Size(width, height);
        }

        public override void Clear(Color color) => skCanvas.Clear(color.ToSkia());
        public override void ClipRect(RectangleF rect) => skCanvas.ClipRect(rect.ToSkia(), SKClipOperation.Intersect, true);

        private SKPath path = new SKPath();
        private SKPath GetTempPath()
        {
            if(path.Handle == IntPtr.Zero)
            {
                path = new SKPath();
            }
            path.Reset();
            return path;
        }

        public override void DrawTriangles(VertexBuffer vertexBuffer, Image image)
        {
            var vb = (SkiaVertexBuffer)vertexBuffer;
            var img = (SkiaImage)image;

            skPaint.Color = Color.White.ToSkia();
            skPaint.Shader = img.SKShader;

            skCanvas.DrawVertices(vb.SKVertices, SKBlendMode.Multiply, skPaint);
        }

        public override void Restore() => skCanvas.Restore();
        public override void Restore(int count) => skCanvas.RestoreToCount(count);
        public override int SaveState() => skCanvas.Save();
        public override void Transform(Matrix3x2 transform) => skCanvas.SetMatrix((transform * skCanvas.TotalMatrix.ToNumerics()).ToSkia());

        private Vector2 CalculateTargetPosition(Font font, string line, RectangleF target, TextAlign textAlign, FontMeasure fontMeasure, out SizeF size)
        {
            var skiaFont = (SkiaFont)font;

            var skPaint = skiaFont.SKPaint;
            skPaint.IsStroke = false;

            size = font.MeasureText(line, fontMeasure);
            float positionX = target.X;

            switch (textAlign & (TextAlign.Left | TextAlign.Center | TextAlign.Right))
            {
                case TextAlign.Right:
                    positionX = target.Right - size.Width;
                    break;

                case TextAlign.Center:
                    positionX = target.Center.X - size.Width / 2;
                    break;
            }

            float positionY = target.Top;

            switch (textAlign & (TextAlign.Top | TextAlign.Middle | TextAlign.Bottom))
            {
                case TextAlign.Bottom:
                    positionY = target.Bottom - size.Height;
                    break;

                case TextAlign.Middle:
                    positionY = target.Center.Y - size.Height / 2;
                    break;
            }

            var offsetY = size.Height;
            if (fontMeasure == FontMeasure.Extended)
            {
                offsetY = skiaFont.SKFont.Size;
            }

            return new Vector2(positionX, positionY + offsetY);
        }

        public override Vector2 DrawText(string line, Font font, RectangleF target, TextAlign textAlign, Color color, FontMeasure fontMeasure = FontMeasure.Extended)
        {
            if (font == null) return Vector2.Zero;
            if (line == null) return Vector2.Zero;

            var skiaFont = (SkiaFont)font;

            var position = CalculateTargetPosition(font, line, target, textAlign, fontMeasure, out var size);

            var skPaint = skiaFont.SKPaint;
            skPaint.IsStroke = false;
            skPaint.IsAntialias = true;
            skPaint.HintingLevel = SKPaintHinting.Full;
            skPaint.FilterQuality = SKFilterQuality.High;
            skPaint.Color = color.ToSkia();

            skCanvas.DrawText(line, position.X, position.Y, skPaint);
            return size;
        }

        public override void DrawEllipse(RectangleF rect, Color color, float thickness)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = true;
            skPaint.StrokeWidth = thickness;
            skCanvas.DrawOval(rect.ToSkia(), skPaint);
        }

        public override void FillEllipse(RectangleF rect, Color color)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = false;
            skCanvas.DrawOval(rect.ToSkia(), skPaint);
        }


        public override void FillRect(RectangleF rect, Color color)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = false;
            skCanvas.DrawRect(rect.ToSkia(), skPaint);
        }

        public override void DrawRect(RectangleF rect, Color color, float thickness)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = true;
            skPaint.StrokeWidth = thickness;
            skCanvas.DrawRect(rect.ToSkia(), skPaint);
        }

        public override void FillRoundRect(RectangleF rect, Vector2 roundness, Color color)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = false;
            skCanvas.DrawRoundRect(rect.ToSkia(), new SKSize(roundness.X, roundness.Y), skPaint);
        }

        public override void DrawRoundRect(RectangleF rect, Vector2 roundness, Color color, float thickness)
        {
            skPaint.Color = color.ToSkia();
            skPaint.IsStroke = true;
            skPaint.StrokeWidth = thickness;
            skCanvas.DrawRoundRect(rect.ToSkia(), new SKSize(roundness.X, roundness.Y), skPaint);
        }

        public override void DrawImage(Image image, RectangleF target, RectangleF source, float opacity)
        {
            var skiaImage = (SkiaImage)image;
            skPaint.Color = new SKColor(255, 255, 255, (byte)(opacity * 255));
            skCanvas.DrawImage(skiaImage.SKImage, source.ToSkia(), target.ToSkia(), skPaint);
        }
    }
}
