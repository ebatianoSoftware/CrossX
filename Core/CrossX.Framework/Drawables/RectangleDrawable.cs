using CrossX.Framework.Graphics;

namespace CrossX.Framework.Drawables
{
    public class RectangleDrawable: Drawable
    {
        public Length Rx { get; set; }
        public Length Ry { get; set; }
        public Color FillColor { get; set; }
        public Color StrokeColor { get; set; }
        public Length StrokeThickness { get; set; }

        public override void Draw(Canvas canvas, RectangleF rectangle, Color color)
        {
            var rx = Rx.Calculate(rectangle.Width);
            var ry = Ry.Calculate(rectangle.Height);

            if (FillColor.A > 0)
            {
                canvas.FillRoundRect(rectangle, new System.Numerics.Vector2(rx, ry), FillColor * color);
            }

            if(StrokeColor.A > 0)
            {
                canvas.DrawRoundRect(rectangle, new System.Numerics.Vector2(rx, ry), StrokeColor * color, StrokeThickness.Calculate());
            }
        }
    }
}
