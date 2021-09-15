using CrossX.Framework.Graphics;
using CrossX.Framework.UI.Containers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CrossX.Framework.UI
{
    public abstract class View : INotifyPropertyChanged
    {
        private RectangleF bounds;
        private Alignment horizontalAlignment;
        private Alignment verticalAlignment;
        private Length width = Length.Auto;
        private Length height = Length.Auto;
        private Color backgroundColor = Color.Transparent;
        private Thickness margin = Thickness.Zero;

        public event PropertyChangedEventHandler PropertyChanged;

        public RectangleF ScreenBounds => Parent == null ? Bounds : Bounds.Offset(Parent.ScreenBounds.TopLeft);

        public RectangleF Bounds
        {
            get => bounds;
            set
            {
                if (bounds != value)
                {
                    bounds = value;
                    OnPropertyChanged(nameof(ActualWidth));
                    OnPropertyChanged(nameof(ActualHeight));
                    RecalculateLayout();
                }
            }
        }

        public Alignment HorizontalAlignment { get => horizontalAlignment; set => SetProperty(ref horizontalAlignment, value); }
        public Alignment VerticalAlignment { get => verticalAlignment; set => SetProperty(ref verticalAlignment, value); }

        public Length Width { get => width; set => SetProperty(ref width, value); }
        public Length Height { get => height; set => SetProperty(ref height, value); }
        public Thickness Margin { get => margin; set => SetProperty(ref margin, value); }
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        public float ActualWidth => Bounds.Width;
        public float ActualHeight => Bounds.Height;

        public ViewContainer Parent { get; internal set; }

        public void Render(Canvas canvas)
        {
            OnRender(canvas);
        }

        public void Update(float time)
        {
            OnUpdate(time);
        }

        public virtual void RecalculateLayout()
        {

        }

        protected virtual void OnRender(Canvas canvas)
        {
            if (BackgroundColor.A > 0)
            {
                canvas.FillRect(ScreenBounds, BackgroundColor);
            }
        }

        protected virtual void OnUpdate(float time)
        {

        }

        public virtual SizeF CalculateSize(SizeF parentSize)
        {
            float onePixelUnit = 1;

            var parentWidth = parentSize.Width - Margin.Left.Calculate(onePixelUnit) - Margin.Right.Calculate(onePixelUnit);
            var parentHeight = parentSize.Height - Margin.Top.Calculate(onePixelUnit) - Margin.Bottom.Calculate(onePixelUnit);

            var width = Width.IsAuto ? parentWidth : Width.Calculate(onePixelUnit, parentWidth);
            var height = Height.IsAuto ? parentHeight : Height.Calculate(onePixelUnit, parentHeight);
            return new SizeF(width, height);
        }

        public virtual Vector2 CalculatePosition(SizeF calculatedSize, SizeF parentSize)
        {
            float onePixelUnit = 1;

            var marginLeft = Margin.Left.Calculate(onePixelUnit);
            var marginRight = Margin.Right.Calculate(onePixelUnit);
            var marginTop = Margin.Top.Calculate(onePixelUnit);
            var marginBottom = Margin.Bottom.Calculate(onePixelUnit);

            float x = marginLeft;
            float y = marginTop;

            switch (HorizontalAlignment)
            {
                case Alignment.Center:
                    x = (parentSize.Width - marginLeft - marginRight - calculatedSize.Width) / 2 + marginLeft;
                    break;

                case Alignment.End:
                    x = parentSize.Width - marginRight - calculatedSize.Width;
                    break;
            }

            switch (VerticalAlignment)
            {
                case Alignment.Center:
                    y = (parentSize.Height - marginTop - marginBottom - calculatedSize.Height) / 2 + marginTop;
                    break;

                case Alignment.End:
                    y = parentSize.Height - marginBottom - calculatedSize.Height;
                    break;
            }

            return new Vector2(x, y);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return false;

            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
