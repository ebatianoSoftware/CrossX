using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xx;
using Xx.Definition;

namespace CrossX.Framework.UI
{
    [XxSchemaBindable(true)]
    [XxSchemaExport]
    public abstract class View : UIBindingContext, IDisposable, IStoreElement
    {
        private RectangleF bounds;
        private Alignment horizontalAlignment = Alignment.Stretch;
        private Alignment verticalAlignment = Alignment.Stretch;
        private Length width = Length.Auto;
        private Length height = Length.Auto;
        private Color backgroundColor = Color.Transparent;
        private Thickness margin = Thickness.Zero;
        private bool visible = true;
        private Drawable backgroundDrawable;

        private IViewParent parent;
        private float opacity = 1;
        private IReadOnlyDictionary<PropertyInfo, object> properties;

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
                    Invalidate();
                }
            }
        }

        public bool InputTransparent { get; set; }
        public bool NativeDraggable { get; set; }

        public Drawable BackgroundDrawable { get => backgroundDrawable; set => SetPropertyAndRedraw(ref backgroundDrawable, value); }
        public Alignment HorizontalAlignment { get => horizontalAlignment; set => SetProperty(ref horizontalAlignment, value); }
        public Alignment VerticalAlignment { get => verticalAlignment; set => SetProperty(ref verticalAlignment, value); }
        public Length Width { get => width; set => SetProperty(ref width, value); }
        public Length Height { get => height; set => SetProperty(ref height, value); }
        public Thickness Margin { get => margin; set => SetProperty(ref margin, value); }
        public Color BackgroundColor { get => backgroundColor; set => SetPropertyAndRedraw(ref backgroundColor, value); }

        public float Opacity { get => opacity; set => SetPropertyAndRedraw(ref opacity, value); }

        [XxSchemaBindable(false)]
        public Name Id { get; set; }

        [XxSchemaBindable(false)]
        public Classes Classes { set { } }

        public bool Visible { get => visible; set => SetPropertyAndRecalcLayout(ref visible, value); }

        public float ActualWidth => Bounds.Width;

        public float ActualHeight => Bounds.Height;

        public IViewParent Parent
        {
            get => parent;
            internal set
            {
                parent = value;
                if (DataContext == null && parent != null)
                {
                    Services.BindingService.AddDataContextBinding(this, parent);
                }
            }
        }

        public bool DisplayVisible => Visible && (Parent?.DisplayVisible ?? true);

        XxElement IStoreElement.Element { set => properties = value.Properties; }

        protected View(IUIServices services)
        {
            Services = services;
        }

        public void Render(Canvas canvas, float opacity = 1)
        {
            if (!Visible) return;
            OnRender(canvas, opacity * Opacity);
        }

        public void Update(float time)
        {
            OnUpdate(time);
        }

        protected virtual void RecalculateLayout()
        {

        }

        protected virtual void OnRender(Canvas canvas, float opacity)
        {
            if (BackgroundColor.A > 0)
            {
                if (BackgroundDrawable == null)
                {
                    canvas.FillRect(ScreenBounds, BackgroundColor * opacity);
                }
                else
                {
                    BackgroundDrawable.Draw(canvas, ScreenBounds, BackgroundColor * opacity);
                }
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
            if (InputTransparent) return false;
            if (!DisplayVisible || !Visible) return false;
            if (OnPreviewGesture(gesture)) return true;
            return false;
        }

        protected virtual bool OnPreviewGesture(Gesture gesture)
        {
            return false;
        }

        public bool ProcessGesture(Gesture gesture)
        {
            if (InputTransparent) return false;
            if (!DisplayVisible || !Visible) return false;
            if (OnProcessGesture(gesture)) return true;

            if(ScreenBounds.Contains(gesture.Position))
            {
                if(NativeDraggable)
                {
                    gesture.SetCursor = CursorType.NativeDrag;
                }

                if (BackgroundColor.A > 0)
                {
                    return true;
                }
            }
            return false;
        }
        protected virtual bool OnProcessGesture(Gesture gesture)
        {
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            Services.BindingService.RemoveBindings(this);
            Services.TooltipService.HideTooltip(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
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

        protected virtual bool SetPropertyAndRedraw<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (SetProperty(ref property, value, propertyName))
            {
                Invalidate();
                return true;
            }
            return false;
        }

        protected virtual bool SetPropertyAndRecalcLayout<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (SetProperty(ref property, value, propertyName))
            {
                Parent?.InvalidateLayout();
                Invalidate();
                return true;
            }
            return false;
        }

        protected void Invalidate() => Parent?.Window?.Redraw();
    }
}
