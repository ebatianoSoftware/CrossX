using CrossX.Framework.Graphics;
using System;
using System.Numerics;

namespace CrossX.Framework.Drawables
{
    public class SplitterDotsDrawable: Drawable
    {
        public Length Padding { get; set; } = new Length(1);
        public Length Spacing { get; set; } = new Length(1);
        public Length Rx { get; set; }
        public Length Ry { get; set; }
        public int Count { get; set; }
        public Color BackgroundTint { get; set; }


        public override void Draw(Canvas canvas, RectangleF rectangle, Color color)
        {
            canvas.FillRect(rectangle, color * BackgroundTint);

            var padding = Padding.Calculate();

            var size = Math.Min(rectangle.Width, rectangle.Height) - padding * 2;

            var spacing = Spacing.Calculate();

            var rx = Rx.Calculate(size);
            var ry = Ry.Calculate(size);

            Vector2 step = Vector2.Zero;

            if(rectangle.Width > rectangle.Height)
            {
                step = new Vector2(size + spacing, 0);
            }
            else
            {
                step = new Vector2(0, size + spacing);
            }

            var startC = rectangle.Center - step * Count / 2f;
            var bounds = new RectangleF(startC.X - size / 2, startC.Y - size / 2, size, size);

            for (var idx = 0; idx < Count; ++idx)
            {    
                canvas.FillRoundRect(bounds, new Vector2(rx, ry), color);
                bounds = bounds.Offset(step);
            }
        }
    }
}
