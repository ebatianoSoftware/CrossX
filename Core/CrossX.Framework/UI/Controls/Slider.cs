using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Numerics;

namespace CrossX.Framework.UI.Controls
{
    public class Slider : View
    {
        private float maxValue = 100;
        private float minValue = 0;
        private float value = 50;
        private Color thumbColor = Color.White;
        private Color trackColor = Color.White;

        private Color thumbColorOver = Color.White;
        private Color trackColorOver = Color.White;

        private Color thumbColorPushed = Color.White;
        private Color trackColorPushed = Color.White;

        private Length trackThickness;
        private Drawable thumbDrawable;
        private Drawable trackDrawable;

        private bool enabled = true;
        private ButtonState currentState = ButtonState.Normal;
        private Vector2? pointerOffset;
        private float valueResolution;

        private ButtonGesturesProcessor buttonGesturesProcessor;
        private Color trackColorDisabled;
        private Color thumbColorDisabled;

        public float MaxValue { get => maxValue; set => SetPropertyAndRedraw(ref maxValue, value); }
        public float MinValue { get => minValue; set => SetPropertyAndRedraw(ref minValue, value); }

        [BindingMode(BindingMode.TwoWay)]
        public float Value { get => value; set => base.SetPropertyAndRedraw(ref this.value, value); }

        public bool Enabled { get => enabled; set => SetPropertyAndRedraw(ref enabled, value); }

        public Color ThumbColor { get => thumbColor; set => SetPropertyAndRedraw(ref thumbColor, value); }
        public Color TrackColor { get => trackColor; set => SetPropertyAndRedraw(ref trackColor, value); }

        public Color ThumbColorDisabled { get => thumbColorDisabled; set => SetPropertyAndRedraw(ref thumbColorDisabled, value); }
        public Color TrackColorDisabled { get => trackColorDisabled; set => SetPropertyAndRedraw(ref trackColorDisabled, value); }

        public Color ThumbColorOver { get => thumbColorOver; set => SetPropertyAndRedraw(ref thumbColorOver, value); }
        public Color TrackColorOver { get => trackColorOver; set => SetPropertyAndRedraw(ref trackColorOver, value); }

        public Color ThumbColorPushed { get => thumbColorPushed; set => SetPropertyAndRedraw(ref thumbColorPushed, value); }
        public Color TrackColorPushed { get => trackColorPushed; set => SetPropertyAndRedraw(ref trackColorPushed, value); }

        public Length TrackThickness { get => trackThickness; set => SetPropertyAndRedraw(ref trackThickness, value); }
        public Drawable ThumbDrawable { get => thumbDrawable; set => SetPropertyAndRedraw(ref thumbDrawable, value); }
        public Drawable TrackDrawable { get => trackDrawable; set => SetPropertyAndRedraw(ref trackDrawable, value); }

        public float ValueResolution
        {
            get => valueResolution;
            set
            {
                if (SetProperty(ref valueResolution, value))
                {
                    Value = SnapValue(Value);
                }
            }
        }

        private ButtonState CurrentState
        {
            get => currentState;
            set => SetPropertyAndRedraw(ref currentState, value);
        }

        public Slider(IUIServices services) : base(services)
        {
            buttonGesturesProcessor = new ButtonGesturesProcessor(
                state => CurrentState = state,
                onDownAction: g =>
                {
                    pointerOffset = null;
                    CalculateValue(g.Position);
                },
                onMoveAction: g => CalculateValue(g.Position),
                onUpAction: g => pointerOffset = null//,
                //onHoveredAction: g=> g.SetCursor = CursorType.Hand
                );
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);

            var bounds = ScreenBounds;

            var thumbSize = Math.Min(bounds.Height, bounds.Width);
            var area = Math.Max(bounds.Width, bounds.Height) - thumbSize;
            var offset = (Value - MinValue) / (MaxValue - MinValue) * area;

            var trackThickness = TrackThickness.Calculate(thumbSize);

            var trackBounds = bounds;
            var valueBounds = bounds;
            var thumbBounds = bounds;

            thumbBounds.Width = thumbBounds.Height = thumbSize;

            if (bounds.Width > bounds.Height)
            {
                trackBounds.Y = bounds.Center.Y - trackThickness / 2;
                trackBounds.Height = trackThickness;

                valueBounds = trackBounds;
                thumbBounds.X += offset;

                trackBounds.Left = thumbBounds.Right;
                valueBounds.Right = thumbBounds.Left;
            }
            else
            {
                trackBounds.X = bounds.Center.X - trackThickness / 2;
                trackBounds.Width = trackThickness;
                valueBounds = trackBounds;
                thumbBounds.Y += area - offset;

                trackBounds.Bottom = thumbBounds.Top;
                valueBounds.Top = thumbBounds.Bottom;
            }

            var thumbColor = ThumbColor;
            var trackColor = TrackColor;

            switch(CurrentState)
            {
                case ButtonState.Hover:
                    thumbColor = ThumbColorOver;
                    trackColor = TrackColorOver;
                    break;

                case ButtonState.Pushed:
                    thumbColor = ThumbColorPushed;
                    trackColor = TrackColorPushed;
                    break;
            }

            if(!Enabled)
            {
                thumbColor = ThumbColorDisabled;
                trackColor = TrackColorDisabled;
            }

            if (TrackDrawable != null)
            {
                TrackDrawable.Draw(canvas, trackBounds, trackColor * opacity);
                TrackDrawable.Draw(canvas, valueBounds, thumbColor * opacity);
            }
            else
            {
                canvas.FillRect(trackBounds, trackColor * opacity);
                canvas.FillRect(valueBounds, thumbColor * opacity);
            }

            if (ThumbDrawable != null)
            {
                ThumbDrawable.Draw(canvas, thumbBounds, thumbColor * opacity);
            }
            else
            {
                canvas.FillEllipse(thumbBounds, thumbColor);
            }
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            if (buttonGesturesProcessor.ProcessGesture(gesture, ScreenBounds, Enabled)) return true;

            return base.OnProcessGesture(gesture);
        }

        private void CalculateValue(Vector2 position)
        {
            var bounds = ScreenBounds;

            var thumbSize = Math.Min(bounds.Height, bounds.Width);
            var area = Math.Max(bounds.Width, bounds.Height) - thumbSize;
            var offset = (Value - MinValue) / (MaxValue - MinValue) * area;

            var thumbBounds = bounds;
            thumbBounds.Width = thumbBounds.Height = thumbSize;

            if (bounds.Width > bounds.Height)
            {
                thumbBounds.X += offset;
            }
            else
            {
                thumbBounds.Y += area - offset;
            }

            if (!pointerOffset.HasValue)
            {
                if (thumbBounds.Contains(position))
                {
                    pointerOffset = position - thumbBounds.TopLeft;
                }
                else
                {
                    pointerOffset = new Vector2(thumbSize / 2);
                }
            }

            position -= pointerOffset.Value;

            float valueNormalized;
            if (bounds.Width > bounds.Height)
            {
                valueNormalized = (position.X - bounds.X) / area;
            }
            else
            {
                valueNormalized = 1 - (position.Y - bounds.Y) / area;
            }

            valueNormalized = Math.Max(0, Math.Min(1, valueNormalized));

            Value = SnapValue(MinValue + valueNormalized * (MaxValue - MinValue));
            Services.RedrawService.RequestRedraw();
        }

        private float SnapValue(float value)
        {
            if (ValueResolution == 0) return value;

            var offset = value - MinValue + ValueResolution / 2;
            offset = offset - offset % ValueResolution;
            return MinValue + offset;
        }
    }
}
