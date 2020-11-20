using CrossX.Forms.Values;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public class PositionPanel : Panel
    {
        public Length X { get => x; set => SetProperty(ref x, value); }
        public Length Y { get => y; set => SetProperty(ref y, value); }

        private Length x;
        private Length y;

        public PositionPanel(IControlParent parent, IControlServices services) : base(parent, services)
        {
            HorizontalAlignment = Alignment.Center;
            VerticalAlignment = Alignment.Center;
        }

        public override Vector2 CalculatePosition(RectangleF clientArea, Vector2 size)
        {
            clientArea = ClientAreaWithMargin(clientArea);

            var offsetX = x.Value + x.Percent * clientArea.Width;
            var offsetY = y.Value + y.Percent * clientArea.Height;

            float px = clientArea.X + offsetX;
            float py = clientArea.Y + offsetY;

            switch (HorizontalAlignment)
            {
                case Alignment.End:
                    px = px - size.X;
                    break;

                case Alignment.Center:
                    px = px - size.X / 2;
                    break;
            }

            switch (VerticalAlignment)
            {
                case Alignment.End:
                    py = py - size.Y;
                    break;

                case Alignment.Center:
                    py = py - size.Y / 2;
                    break;
            }

            return new Vector2(px, py);
        }
    }
}
