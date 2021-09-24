using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{

    public class ProgressBar : View
    {
        private float maxValue = 100;
        private float progress = 0;
        private ProgressDisplayMode mode;
        private Color foregroundColor;

        public float MaxValue { get => maxValue; set => SetPropertyAndRedraw(ref maxValue, value); }
        public float Progress { get => progress; set => SetPropertyAndRedraw(ref progress, value); }

        public Color ForegroundColor { get => foregroundColor; set => SetPropertyAndRedraw(ref foregroundColor, value); }

        public Length IndeterminateSpeed { get => indeterminateSpeed; set => indeterminateSpeed = value; }

        public ProgressDisplayMode Mode { get => mode; set => SetPropertyAndRedraw(ref mode, value); }

        private float step = 0;
        private Length indeterminateSpeed = new Length(256);

        public ProgressBar(IUIServices services) : base(services)
        {
        }

        protected override void OnRender(Canvas canvas)
        {
            base.OnRender(canvas);

            switch(Mode)
            {
                case ProgressDisplayMode.Determinate:
                    DrawDeterminate(canvas);
                    break;

                case ProgressDisplayMode.Indeterminate:
                    DrawIndeterminate(canvas, false);
                    break;

                case ProgressDisplayMode.Query:
                    DrawIndeterminate(canvas, true);
                    break;

                case ProgressDisplayMode.Buffer:
                    DrawBuffer(canvas);
                    break;
            }
            
        }

        private void DrawBuffer(Canvas canvas)
        {
            var size = ScreenBounds.Height;
            var count = ScreenBounds.Width / size / 2 + 2;

            var offset = step % size * 2;

            canvas.SaveState();
            canvas.ClipRect(ScreenBounds);

            var bounds = ScreenBounds;
            bounds.Width = size;
            bounds.X -= offset;

            
            for(var idx =0; idx < count; ++idx)
            {
                canvas.FillEllipse(bounds, ForegroundColor);
                bounds.X += size * 2;
            }

            canvas.Restore();
        }

        private void DrawIndeterminate(Canvas canvas, bool reverse)
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
            canvas.FillRect(bounds, ForegroundColor);
        }

        private void DrawDeterminate(Canvas canvas)
        {
            var factor = Progress / MaxValue;

            if (factor > 0)
            {
                var bounds = ScreenBounds;
                bounds.Width *= factor;
                canvas.FillRect(bounds, ForegroundColor);
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
