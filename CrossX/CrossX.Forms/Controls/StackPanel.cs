using CrossX.Forms.Values;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public class StackPanel : ContainerControl
    {
        private Orientation orientation = Orientation.Vertical;
        public Orientation Orientation { get => orientation; set => SetProperty( ref orientation, value); }

        public StackPanel(IControlParent parent, IControlServices services) : base(parent, services)
        {
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case nameof(Orientation):
                    ShouldCalculateLayout = true;
                    break;
            }
        }

        public override Vector2 CalculateSize(RectangleF clientArea, bool includeMargins)
        {
            var size = base.CalculateSize(clientArea, includeMargins);

            if (orientation == Orientation.Horizontal)
            {
                if (HorizontalAlignment == Alignment.Stretch || !Width.IsAuto)
                {
                    clientArea.Width = size.X;
                }
            }
            else
            {
                if (VerticalAlignment == Alignment.Stretch || !Height.IsAuto)
                {
                    clientArea.Height = size.Y;
                }
            }

            var csize = ComputeChildren(clientArea, false);

            if (orientation == Orientation.Horizontal)
            {
                if (HorizontalAlignment != Alignment.Stretch && Width.IsAuto)
                {
                    size.X = csize.X;
                }
            }
            else
            {
                if (VerticalAlignment != Alignment.Stretch && Height.IsAuto)
                {
                    size.Y = csize.Y;
                }
            }

            return size;
        }

        private Vector2 ComputeChildren(RectangleF clientArea, bool positionControl)
        {
            var width = clientArea.Width;
            var height = clientArea.Height;

            foreach (var child in children)
            {
                if (orientation == Orientation.Horizontal)
                    width -= child.Margin.Left + child.Margin.Right;
                else height -= child.Margin.Top + child.Margin.Bottom;
            }

            foreach (var child in children)
            {
                if (orientation == Orientation.Horizontal)
                {
                    clientArea.Height = width - child.Margin.Top - child.Margin.Bottom;
                    if (HorizontalAlignment != Alignment.Stretch && Width.IsAuto)
                    {
                        clientArea.Width = 0;
                    }
                    else
                    {
                        clientArea.Width = width;
                    }
                }
                else
                {
                    clientArea.Width = width - child.Margin.Left - child.Margin.Right;
                    if (VerticalAlignment != Alignment.Stretch && Height.IsAuto)
                    {
                        clientArea.Height = 0;
                    }
                    else
                    {
                        clientArea.Height = height;
                    }
                }

                var size = child.CalculateSize(clientArea, false);
                var sizeWithMargins = new Vector2(size.X + child.Margin.Left + child.Margin.Right, size.Y + child.Margin.Top + child.Margin.Bottom);

                if(positionControl)
                {
                    var position = child.CalculatePosition(clientArea, size);
                    child.PositionControl(position, size);
                }

                if (orientation == Orientation.Horizontal) clientArea.X += sizeWithMargins.X;
                else clientArea.Y += sizeWithMargins.Y;
            }

            return new Vector2(clientArea.X, clientArea.Y);
        }

        protected override void CalculateLayout()
        {
            var clientArea = new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight);
            var size = ComputeChildren(clientArea, true);

            if (orientation == Orientation.Horizontal)
            {
                if(size.X != ActualWidth)
                {
                    ShouldCalculateLayout = true;
                    Parent.InvalidateLayout();
                }
            }
            else
            {
                if (size.Y != ActualHeight)
                {
                    ShouldCalculateLayout = true;
                    Parent.InvalidateLayout();
                }
            }
        }

    }
}
