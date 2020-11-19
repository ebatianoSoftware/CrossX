using CrossX.Forms.Binding;
using CrossX.Forms.Values;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public abstract class Control : ObservableDataModel, IDisposable, IObjectWithDataContext
    {
        public Color4 Background { get => background; set => SetProperty(ref background, value); }
        private Color4 background = Color4.Transparent;

        public Length Width { get => width; set => SetProperty(ref width, value); }
        public Length Height { get => height; set => SetProperty(ref height, value); }
        public Alignment HorizontalAlignment { get => horizontalAlignment; set => SetProperty(ref horizontalAlignment, value); }
        public Alignment VerticalAlignment { get => verticalAlignment; set => SetProperty(ref verticalAlignment, value); }
        public Margin Margin { get => margin; set => SetProperty(ref margin, value); }
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }

        public float ActualWidth { get => actualWidth; private set => SetProperty(ref actualWidth, value); }
        public float ActualHeight { get => actualHeight; private set => SetProperty(ref actualHeight, value); }
        public float ActualX { get => actualX; private set => SetProperty(ref actualX, value); }
        public float ActualY { get => actualY; private set => SetProperty(ref actualY, value); }
        public virtual object DataContext
        {
            get => dataContext;
            set
            {
                if (SetProperty(ref dataContext, value))
                {
                    RecreateBindings();
                }
            }
        }

        public bool ShouldCalculateLayout { get; protected set; }
        public IControlParent Parent { get; }

        private Length width = Length.Auto;
        private Length height = Length.Auto;
        private float actualWidth;
        private float actualHeight;
        private float actualX;
        private float actualY;
        private Alignment horizontalAlignment = Alignment.Stretch;
        private Alignment verticalAlignment = Alignment.Stretch;
        private Margin margin = Margin.Zero;

        private Dictionary<string, object> customProperties;
        private object dataContext;
        private bool isVisible = true;

        public BindingService BindingService { get; }

        protected RectangleF ClientArea => new RectangleF(actualX, actualY, actualWidth, actualHeight);

        protected Control(IControlParent parent)
        {
            Parent = parent;
            BindingService = parent.ObjectFactory.Create<BindingService>(this);
        }

        public virtual void RecreateBindings()
        {
            BindingService.RecreateValues();
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case nameof(ActualWidth):
                case nameof(ActualHeight):
                case nameof(ActualX):
                case nameof(ActualY):
                case nameof(Width):
                case nameof(Height):
                case nameof(Margin):
                case nameof(HorizontalAlignment):
                case nameof(VerticalAlignment):
                    ShouldCalculateLayout = true;
                    break;
            }
        }
        public virtual void BeforeUpdate()
        {
            if (ShouldCalculateLayout)
            {
                CalculateLayout();
                ShouldCalculateLayout = false;
            }
        }

        public void InvalidateLayout()
        {
            ShouldCalculateLayout = true;
        }

        protected virtual void CalculateLayout()
        {
            ShouldCalculateLayout = false;
        }

        protected RectangleF ClientAreaWithMargin(RectangleF clientArea)
        {
            clientArea.X += margin.Left;
            clientArea.Y += margin.Top;
            clientArea.Width -= margin.Left + margin.Right;
            clientArea.Height -= margin.Top + margin.Bottom;
            return clientArea;
        }

        public void CalculateSizeWithMargins(RectangleF clientArea, out Vector2 size, out Vector2 sizeWithMargins)
        {
            size = CalculateSize(clientArea, true);
            sizeWithMargins = new Vector2(size.X + margin.Left + margin.Right, size.Y + margin.Top + margin.Bottom);
        }

        public virtual Vector2 CalculatePosition(RectangleF clientArea, Vector2 size)
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

        public virtual Vector2 CalculateSize(RectangleF clientArea, bool includeMargins)
        {
            if (includeMargins)
            {
                clientArea = ClientAreaWithMargin(clientArea);
            }

            var pw = width.Value + clientArea.Width * width.Percent;
            var ph = height.Value + clientArea.Height * height.Percent;

            if (horizontalAlignment == Alignment.Stretch && clientArea.Width >=0 ) pw = clientArea.Width;
            if (verticalAlignment == Alignment.Stretch && clientArea.Height >= 0) ph = clientArea.Height;

            pw = Math.Max(0, pw);
            ph = Math.Max(0, ph);

            return new Vector2(pw, ph);
        }

        public void PositionControl(Vector2 position, Vector2 size)
        {
            ActualX = position.X;
            ActualY = position.Y;
            ActualWidth = size.X;
            ActualHeight = size.Y;
        }

        internal void SetCustomProperty(string name, object value)
        {
            if (customProperties == null)
            {
                customProperties = new Dictionary<string, object>();
            }
            customProperties[name] = value;
        }

        public TProperty GetCustomProperty<TProperty>(string name)
        {
            if (customProperties == null) return default;
            if (customProperties.TryGetValue(name, out var obj) && obj is TProperty) return (TProperty)obj;
            return default;
        }

        public virtual void Update(TimeSpan frameTime) { }

        public void Draw(TimeSpan frameTime, Color4 tintColor)
        {
            if (!IsVisible) return;
            OnDraw(frameTime, tintColor);
        }

        protected virtual void OnDraw(TimeSpan frameTime, Color4 tintColor)
        {
            if (background.A > 0)
            {
                Parent.PrimitiveBatch.DrawRect(new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight), background);
            }
        }

        public bool ProcessTouch(long id, TouchEvent evnt, Vector2 position)
        {
            if (!IsVisible && evnt != TouchEvent.Remove && evnt != TouchEvent.Up)
            {
                return false;
            }

            return OnTouch(id, evnt, position);
        }

        public virtual void OnPointerCaptured(long id, object capturedBy)
        {

        }

        protected virtual bool OnTouch(long id, TouchEvent evnt, Vector2 position)
        {
            return false;
        }

        public virtual void Dispose()
        {
            BindingService.Dispose();
        }
    }
}
