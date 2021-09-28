using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{

    public class ProgressBar : View
    {
        private float maxValue = 100;
        private float value = 0;
        private ProgressDisplayMode mode;
        private Color foregroundColor;

        public float MaxValue { get => maxValue; set => SetPropertyAndRedraw(ref maxValue, value); }
        public float Value { get => value; set => base.SetPropertyAndRedraw(ref this.value, value); }

        public Color ForegroundColor { get => foregroundColor; set => SetPropertyAndRedraw(ref foregroundColor, value); }

        public Length IndeterminateSpeed { get => indeterminateSpeed; set => indeterminateSpeed = value; }

        public ProgressDisplayMode Mode { get => mode; set => SetPropertyAndRedraw(ref mode, value); }

        private float step = 0;
        private Length indeterminateSpeed = new Length(256);

        public ProgressBar(IUIServices services) : base(services)
        {
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);

            switch(Mode)
            {
                case ProgressDisplayMode.Determinate:
                    DrawDeterminate(canvas, opacity);
                    break;

                case ProgressDisplayMode.Indeterminate:
                    DrawIndeterminate(canvas, opacity, false);
                    break;

                case ProgressDisplayMode.Query:
                    DrawIndeterminate(canvas, opacity, true);
                    break;

                case ProgressDisplayMode.Buffer:
                    DrawBuffer(canvas, opacity);
                    break;
            }
        }

        private void DrawBuffer(Canvas canvas, float opacity)
        {
            var size = ScreenBounds.Height;
            var count = ScreenBounds.Width / size / 2 + 2;

            var offset = step % size * 2;

            canvas.SaveState();
            canvas.ClipRect(ScreenBounds, SizeF.Zero);

            var bounds = ScreenBounds;
            bounds.Width = size;
            bounds.X -= offset;

            
            for(var idx =0; idx < count; ++idx)
            {
                canvas.FillEllipse(bounds, ForegroundColor * opacity);
                bounds.X += size * 2;
            }

            canvas.Restore();
        }

        private void DrawIndeterminate(Canvas canvas, float opacity, bool reverse)
        {
            var bounds = ScreenBounds;
            bounds.Width /= 2;

            var maxOffset = bounds.Width + ScreenBounds.Width;
            var offset = step % maxOffset;

            if(reverse)
            {
                offset = maxOffset - offset;
            }

            bounds.X += offset - bounds.Width;

            bounds = bounds.Intersect(ScreenBounds);
            canvas.FillRect(bounds, ForegroundColor * opacity);
        }

        private void DrawDeterminate(Canvas canvas, float opacity)
        {
            var factor = Value / MaxValue;

            if (factor > 0)
            {
                var bounds = ScreenBounds;
                bounds.Width *= factor;
                canvas.FillRect(bounds, ForegroundColor * opacity);
            }
        }

        protected override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            if (Visible && Mode != ProgressDisplayMode.Determinate)
            {
                step += time * IndeterminateSpeed.Calculate(ScreenBounds.Width);
                Services.RedrawService.RequestRedraw();
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch(propertyName)
            {
                case nameof(Visible):
                    step = 0;
                    break;
            }
        }
    }
}
