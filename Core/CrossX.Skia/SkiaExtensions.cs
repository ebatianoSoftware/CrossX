using CrossX.Framework;
using SkiaSharp;
using System.Numerics;

namespace CrossX.Skia
{
    internal static class SkiaExtensions
    {
        public static SKColor ToSkia(this Color color) => new SKColor(color.R, color.G, color.B, color.A);
        public static SKPoint ToSkia(this Vector2 vector) => new SKPoint(vector.X, vector.Y);
        public static SKPoint ToSkia(this Point point) => new SKPoint(point.X, point.Y);
        public static SKRect ToSkia(this Rectangle rect) => new SKRect(rect.X, rect.Y, rect.Right, rect.Bottom);
        public static SKRect ToSkia(this RectangleF rect) => new SKRect(rect.X, rect.Y, rect.Right, rect.Bottom);
        public static SKRectI ToSkiaI(this Rectangle rect) => new SKRectI(rect.X, rect.Y, rect.Right, rect.Bottom);
        public static SKMatrix ToSkia(this Matrix3x2 matrix) => new SKMatrix(matrix.M11, matrix.M21, matrix.M31, matrix.M12, matrix.M22, matrix.M32, 0, 0, 1);
        public static Matrix3x2 ToNumerics(this SKMatrix matrix) => new Matrix3x2(matrix.ScaleX, matrix.SkewY, matrix.SkewX, matrix.ScaleY, matrix.TransX, matrix.TransY);
        public static SKFontStyleWeight ToSkia(this FontWeight weight) => (SKFontStyleWeight)weight;
    }
}
