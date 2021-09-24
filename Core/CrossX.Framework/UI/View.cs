using CrossX.Abstractions.Mvvm;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Numerics;
using Xx;

namespace CrossX.Framework.UI
{
    [XxSchemaBindable(true)]
    [XxSchemaExport]
    public abstract class View : UIBindingContext, IDisposable
    {
        private RectangleF bounds;
        private Alignment horizontalAlignment = Alignment.Stretch;
        private Alignment verticalAlignment = Alignment.Stretch;
        private Length width = Length.Auto;
        private Length height = Length.Auto;
        private Color backgroundColor = Color.Transparent;
        private Thickness margin = Thickness.Zero;
        private bool visible;
        
        private IViewParent parent;
        protected readonly IUIServices Services;

        public RectangleF ScreenBounds => Parent == null ? Bounds : Bounds.Offset(Parent.ScreenBounds.TopLeft);

        [XxSchemaIgnore]
        public RectangleF Bounds
        {
            get => bounds;

            set
            {
                if (bounds != value)
                {
                    bounds = value;
                    RaisePropertyChanged(nameof(ActualWidth));
                    RaisePropertyChanged(nameof(ActualHeight));
                    RecalculateLayout();
                    Services.RedrawService.RequestRedraw();
                }
            }
        }

        public Alignment HorizontalAlignment { get => horizontalAlignment; set => SetProperty(ref horizontalAlignment, value); }
        public Alignment VerticalAlignment { get => verticalAlignment; set => SetProperty(ref verticalAlignment, value); }
        public Length Width { get => width; set => SetProperty(ref width, value); }
        public Length Height { get => height; set => SetProperty(ref height, value); }
        public Thickness Margin { get => margin; set => SetProperty(ref margin, value); }
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        [XxSchemaBindable(false)]
        public Name Id { get; set; }

        [XxSchemaBindable(false)]
        public Classes Classes { get; set; }

        public bool Visible { get => visible; set => SetProperty(ref visible, value); }
        public float ActualWidth => Bounds.Width;
        public float ActualHeight => Bounds.Height;

        public IViewParent Parent
        { 
            get => parent;
            internal set
            {
                parent = value;
                if(DataContext == null && parent != null)
                {
                    Services.BindingService.AddDataContextBinding(this, parent);
                }
            }
        }

        protected View(IUIServices services)
        {
            Services = services;
        }

        public void Render(Canvas canvas)
        {
            OnRender(canvas);
        }

        public void Update(float time)
        {
            OnUpdate(time);
        }

        protected virtual void RecalculateLayout()
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
            var parentWidth = parentSize.Width - Margin.Left.Calculate() - Margin.Right.Calculate();
            var parentHeight = parentSize.Height - Margin.Top.Calculate() - Margin.Bottom.Calculate();

            var width = Width.IsAuto ? parentWidth : Width.Calculate(parentWidth);
            var height = Height.IsAuto ? parentHeight : Height.Calculate(parentHeight);
            return new SizeF(width, height);
        }

        public virtual Vector2 CalculatePosition(SizeF calculatedSize, SizeF parentSize)
        {
            var marginLeft = Margin.Left.Calculate();
            var marginRight = Margin.Right.Calculate();
            var marginTop = Margin.Top.Calculate();
            var marginBottom = Margin.Bottom.Calculate();

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

        public bool PreviewGesture(Gesture gesture)
        {
            if (OnPreviewGesture(gesture)) return true;
            return false;
        }

        protected virtual bool OnPreviewGesture(Gesture gesture)
        {
            return false;
        }

        public bool ProcessGesture(Gesture gesture)
        {
            if (OnProcessGesture(gesture)) return true;
            return ScreenBounds.Contains(gesture.Position);
        }

        protected virtual bool OnProcessGesture(Gesture gesture)
        {
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Services.BindingService.RemoveBindings(this);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(Width):
                case nameof(Height):
                case nameof(HorizontalAlignment):
                case nameof(VerticalAlignment):
                case nameof(Margin):
                    Parent?.InvalidateLayout();
                    break;
            }
        }
    }
}
