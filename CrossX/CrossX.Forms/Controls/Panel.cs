using CrossX.Forms.Values;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public class Panel : ContainerControl
    {
        public Panel(IControlParent parent, IControlServices services) : base(parent, services)
        {
        }

        protected override void CalculateLayout()
        {
            ShouldCalculateLayout = false;
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].CalculateSizeWithMargins(ClientArea, out var size, out var _);
                var position = children[idx].CalculatePosition(ClientArea, size);
                children[idx].PositionControl(position, size);
            }
        }

        public override Vector2 CalculateSize(RectangleF clientArea, bool includeMargins)
        {
            var panelSize = base.CalculateSize(clientArea, includeMargins);

            if(Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
            {
                panelSize.X = 0;

                for (var idx = 0; idx < children.Count; ++idx)
                {
                    children[idx].CalculateSizeWithMargins(RectangleF.Empty, out var _, out var sizeWithMargins);
                    panelSize.X = MathHelper.Max(panelSize.X, sizeWithMargins.X);
                }
            }

            if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
            {
                panelSize.Y = 0;

                for (var idx = 0; idx < children.Count; ++idx)
                {
                    children[idx].CalculateSizeWithMargins(RectangleF.Empty, out var _, out var sizeWithMargins);
                    panelSize.Y = MathHelper.Max(panelSize.Y, sizeWithMargins.Y);
                }
            }

            return panelSize;
        }
    }
}
