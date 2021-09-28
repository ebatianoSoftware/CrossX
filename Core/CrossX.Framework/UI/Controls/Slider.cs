using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Collections.Generic;
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

        public float MaxValue { get => maxValue; set => SetPropertyAndRedraw(ref maxValue, value); }
        public float MinValue { get => minValue; set => SetPropertyAndRedraw(ref minValue, value); }

        [BindingMode(BindingMode.TwoWay)]
        public float Value { get => value; set => base.SetPropertyAndRedraw(ref this.value, value); }

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        public Color ThumbColor { get => thumbColor; set => SetPropertyAndRedraw(ref thumbColor, value); }
        public Color TrackColor { get => trackColor; set => SetPropertyAndRedraw(ref trackColor, value); }

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

        private PointerId lockedPointer = PointerId.None;
        private HashSet<PointerId> hoverPointers = new HashSet<PointerId>();
        private bool enabled = true;

        private ButtonState currentState = ButtonState.Normal;

        private Vector2? pointerOffset;
        private float valueResolution;

        private ButtonState CurrentState
        {
            get => currentState;
            set
            {
                if (currentState != value)
                {
                    currentState = value;
                    Services.RedrawService.RequestRedraw();
                }
            }
        }

        public Slider(IUIServices services) : base(services)
        {
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
            var thumbBounds = bounds;

            thumbBounds.Width = thumbBounds.Height = thumbSize;

            if (bounds.Width > bounds.Height)
            {
                trackBounds.Y = bounds.Center.Y - trackThickness / 2;
                trackBounds.Height = trackThickness;
                thumbBounds.X += offset;
            }
            else
            {
                trackBounds.X = bounds.Center.X - trackThickness / 2;
                trackBounds.Width = trackThickness;
                thumbBounds.Y += area - offset;
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

            canvas.SaveState();
            if (ThumbDrawable != null)
            {
                ThumbDrawable.Draw(canvas, thumbBounds, thumbColor * opacity);
                ThumbDrawable.ClipShape(canvas, thumbBounds, ClipMode.Difference);
            }
            else
            {
                canvas.FillEllipse(thumbBounds, thumbColor);
                canvas.ClipRect(thumbBounds, new SizeF(thumbBounds.Width / 2, thumbBounds.Width / 2), ClipMode.Difference);
            }

            if (TrackDrawable != null)
            {
                TrackDrawable.Draw(canvas, trackBounds, trackColor * opacity);
            }
            else
            {
                canvas.FillRect(trackBounds, trackColor * opacity);
            }

            canvas.Restore();
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.PointerDown:
                    if (lockedPointer == PointerId.None && Enabled)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            pointerOffset = null;
                            lockedPointer = gesture.PointerId;
                            CurrentState = ButtonState.Pushed;
                            CalculateValue(gesture.Position);
                            return true;
                        }
                    }
                    break;

                case GestureType.PointerMove:
                    if (lockedPointer == PointerId.None)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            if (!hoverPointers.Contains(gesture.PointerId))
                            {
                                hoverPointers.Add(gesture.PointerId);
                            }
                        }
                        else
                        {
                            hoverPointers.Remove(gesture.PointerId);
                        }

                        ButtonState newState = hoverPointers.Count > 0 ? ButtonState.Hover : ButtonState.Normal;

                        if (newState != CurrentState)
                        {
                            CurrentState = newState;
                            return true;
                        }
                    }
                    else if (lockedPointer == gesture.PointerId)
                    {
                        CalculateValue(gesture.Position);
                        return true;
                    }
                    break;

                case GestureType.PointerUp:
                    if (lockedPointer == gesture.PointerId)
                    {
                        lockedPointer = PointerId.None;
                        pointerOffset = null;
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            CurrentState = ButtonState.Hover;
                        }
                        else
                        {
                            CurrentState = ButtonState.Normal;
                        }
                        return true;
                    }
                    break;

                case GestureType.CancelPointer:
                    if (lockedPointer == gesture.PointerId)
                    {
                        pointerOffset = null;
                        CurrentState = ButtonState.Normal;
                        lockedPointer = PointerId.None;
                        return true;
                    }
                    break;
            }

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
