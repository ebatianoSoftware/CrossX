using CrossX.Forms.Values;
using System;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public abstract class Control : ObservableDataModel
    {
        public Length Width { get => width; set => SetProperty(ref width, value); }
        public Length Height { get => height; set => SetProperty(ref height, value); }

        public Alignment HorizontalAlignment { get => horizontalAlignment; set => SetProperty(ref horizontalAlignment, value); }
        public Alignment VerticalAlignment { get => verticalAlignment; set => SetProperty(ref verticalAlignment, value); }

        public Margin Margin { get => margin; set => SetProperty(ref margin, value); }

        public float ActualWidth { get => actualWidth; private set => SetProperty(ref actualWidth, value); }
        public float ActualHeight { get => actualHeight; private set => SetProperty(ref actualHeight, value); }

        public float ActualX { get => actualX; set => SetProperty(ref actualX, value); }
        public float ActualY { get => actualY; set => SetProperty(ref actualY, value); }

        public bool ShouldCalculateLayout { get; private set; }
        private IControlParent parent;

        private Length width;
        private Length height;
        private float actualWidth;
        private float actualHeight;
        private float actualX;
        private float actualY;
        private Alignment horizontalAlignment;
        private Alignment verticalAlignment;
        private Margin margin;

        protected Control(IControlParent parent)
        {
            this.parent = parent;
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case nameof(Width):
                case nameof(Height):
                case nameof(Margin):
                case nameof(HorizontalAlignment):
                case nameof(VerticalAlignment):
                    ShouldCalculateLayout = true;
                    break;
            }
        }

        internal void BeforeUpdate()
        {
            if (ShouldCalculateLayout)
            {
                CalculateLayout();
                ShouldCalculateLayout = false;
            }
        }

        protected virtual void CalculateLayout()
        {
            var size = CalculateSize(parent.ClientArea);
            var position = CalculatePosition(parent.ClientArea, size);

            PositionControl(position, size);
        }

        private RectangleF ClientAreaWithMargin(RectangleF clientArea)
        {
            clientArea.X += margin.Left;
            clientArea.Y += margin.Top;
            clientArea.Width -= margin.Left + margin.Right;
            clientArea.Height -= margin.Top + margin.Bottom;
            return clientArea;
        }

        public Vector2 CalculatePosition(RectangleF clientArea, Vector2 size)
        {
            clientArea = ClientAreaWithMargin(clientArea);

            float px = clientArea.X;
            float py = clientArea.Y;

            switch (horizontalAlignment)
            {
                case Alignment.End:
                    px = clientArea.Right - size.X;
                    break;

                case Alignment.Center:
                    px = clientArea.X + (clientArea.Width - size.X) / 2;
                    break;
            }

            switch (verticalAlignment)
            {
                case Alignment.End:
                    py = clientArea.Bottom - size.Y;
                    break;

                case Alignment.Center:
                    py = clientArea.Y + (clientArea.Height - size.Y) / 2;
                    break;
            }

            return new Vector2(px, py);
        }

        public virtual Vector2 CalculateSize(RectangleF clientArea)
        {
            clientArea = ClientAreaWithMargin(clientArea);

            var pw = width.Value + clientArea.Width * width.Percent;
            var ph = height.Value + clientArea.Height * height.Percent;

            if (horizontalAlignment == Alignment.Stretch) pw = clientArea.Width;
            if (verticalAlignment == Alignment.Stretch) ph = clientArea.Height;

            return new Vector2(pw, ph);
        }

        public void PositionControl(Vector2 position, Vector2 size, bool resetCalculateFlag = true)
        {
            ActualX = position.X;
            ActualY = position.Y;
            ActualWidth = size.X;
            ActualHeight = size.Y;

            if (resetCalculateFlag)
            {
                ShouldCalculateLayout = false;
            }
        }

        public virtual void Update(TimeSpan frameTime)
        {
        }

        public virtual void Draw(TimeSpan frameTime)
        {
        }
    }
}
