using CrossX.Framework;

namespace CrossX.WindowsForms
{
    internal static class ConversionHelpers
    {
        public static System.Drawing.Color ToDrawing(this Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        public static System.Drawing.Rectangle ToDrawing(this Rectangle rect) => new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
