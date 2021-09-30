using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Controls;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Numerics;
using Xx;

namespace CrossX.Framework.UI.Containers
{
    [XxSchemaExport(XxChildrenMode.Multiple)]
    public class SplitLayout : View, IElementsContainer, IViewParent
    {
        private bool layoutInvalid;
        private View firstView;
        private View secondView;
        private Orientation orientation;
        private Length splitPosition;
        private bool enableManipulation;
        private Length splitterSize;
        private RectangleF splitterBounds;

        private RectangleF firstBounds;
        private RectangleF secondBounds;
        private Drawable splitterDrawable;
        private Color splitterColor;
        private Color splitterColorOver;
        private Color splitterColorPushed;
        private ButtonState currentState = ButtonState.Normal;
        private Vector2 downPosition;
        private Length firstMinSize = Length.Zero;
        private Length secondMinSize = Length.Zero;

        // TODO: Extract logic of button to some processor

        private PointerId lockedPointer = PointerId.None;
        private HashSet<PointerId> hoverPointers = new HashSet<PointerId>();

        public Orientation Orientation { get => orientation; set => SetPropertyAndRecalcLayout(ref orientation, value); }

        public bool EnableManipulation { get => enableManipulation; set => SetPropertyAndRedraw(ref enableManipulation, value); }

        public Drawable SplitterDrawable { get => splitterDrawable; set => SetPropertyAndRedraw(ref splitterDrawable, value); }

        public Color SplitterColor { get => splitterColor; set => SetPropertyAndRedraw(ref splitterColor, value); }
        public Color SplitterColorOver { get => splitterColorOver; set => SetPropertyAndRedraw(ref splitterColorOver, value); }
        public Color SplitterColorPushed { get => splitterColorPushed; set => SetPropertyAndRedraw(ref splitterColorPushed, value); }

        public Length FirstMinSize { get => firstMinSize; set => SetPropertyAndRecalcLayout(ref firstMinSize, value); }
        public Length SecondMinSize { get => secondMinSize; set => SetPropertyAndRecalcLayout(ref secondMinSize, value); }

        public Length SplitterSize
        {
            get => splitterSize;
            set
            {
                if (SetProperty(ref splitterSize, value))
                {
                    InvalidateLayout();
                }
            }
        }

        public Length SplitPosition
        {
            get => splitPosition;

            set
            {
                if (SetProperty(ref splitPosition, value))
                {
                    InvalidateLayout();
                }
            }
        }

        private ButtonState CurrentState
        {
            get => currentState;
            set => SetPropertyAndRedraw(ref currentState, value);
        }

        public SplitLayout(IUIServices services) : base(services)
        {

        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach (var element in elements)
            {
                if (element is View view)
                {
                    if (firstView == null)
                    {
                        firstView = view;
                        view.Parent = this;
                    }
                    else if (secondView == null)
                    {
                        secondView = view;
                        view.Parent = this;
                        break;
                    }
                }
            }
        }

        public void InvalidateLayout() => layoutInvalid = true;

        protected override void OnUpdate(float time)
        {
            if (layoutInvalid)
            {
                RecalculateLayout();
                Services.RedrawService.RequestRedraw();
            }

            firstView?.Update(time);
            secondView?.Update(time);

            base.OnUpdate(time);
        }

        protected override void RecalculateLayout() => RecalculateLayout(true);
        private void RecalculateLayout(bool checkMin)
        {
            var refSize = Orientation == Orientation.Vertical ? ScreenBounds.Height : ScreenBounds.Width;

            var splitPosition = SplitPosition.Calculate(refSize);
            var splitterSize = SplitterSize.Calculate(refSize);

            splitPosition -= splitterSize / 2;

            var bounds1 = Bounds;
            var bounds2 = Bounds;

            if (Orientation == Orientation.Horizontal)
            {
                bounds1.Width = splitPosition;
                bounds2.Left = bounds1.Right + splitterSize;
            }
            else
            {
                bounds1.Height = splitPosition;
                bounds2.Top = bounds1.Bottom + splitterSize;
            }

            var bounds1Length = Orientation == Orientation.Vertical ? bounds1.Height : bounds1.Width;
            var bounds2Length = Orientation == Orientation.Vertical ? bounds2.Height : bounds2.Width;

            if (SecondMinSize.Calculate(refSize) + FirstMinSize.Calculate(refSize) + splitterSize > Bounds.Width)
            {
                checkMin = false;
            }

            if (checkMin && bounds2Length < SecondMinSize.Calculate(refSize))
            {
                SplitPosition = SplitPositionFromValue(refSize - SecondMinSize.Calculate(refSize) - splitterSize / 2);
                RecalculateLayout(false);
                return;
            }

            if (checkMin && bounds1Length < FirstMinSize.Calculate(refSize))
            {
                SplitPosition = SplitPositionFromValue(FirstMinSize.Calculate(refSize) + splitterSize / 2);
                RecalculateLayout(false);
                return;
            }

            splitterBounds = Bounds;

            if (Orientation == Orientation.Horizontal)
            {
                splitterBounds.X = bounds1.Right;
                splitterBounds.Width = splitterSize;
            }
            else
            {
                splitterBounds.Y = bounds1.Bottom;
                splitterBounds.Height = splitterSize;
            }

            var topLeft = Bounds.TopLeft;

            firstBounds = bounds1.Offset(-topLeft);
            secondBounds = bounds2.Offset(-topLeft);
            splitterBounds = splitterBounds.Offset(-topLeft);

            if (firstView != null)
            {
                firstView.Bounds = firstBounds.Deflate(firstView.Margin);
            }

            if (secondView != null)
            {
                secondView.Bounds = secondBounds.Deflate(secondView.Margin);
            }
        }

        private Length SplitPositionFromValue(float value)
        {
            var refSize = Orientation == Orientation.Vertical ? ScreenBounds.Height : ScreenBounds.Width;
            var percent = 100 * value / refSize;
            return new Length(percent, Length.Type.Percent);
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);

            var splitterColor = SplitterColor;

            switch (CurrentState)
            {
                case ButtonState.Hover:
                    splitterColor = SplitterColorOver;
                    break;

                case ButtonState.Pushed:
                    splitterColor = SplitterColorPushed;
                    break;
            }

            var topLeft = ScreenBounds.TopLeft;

            if (SplitterDrawable != null)
            {
                SplitterDrawable.Draw(canvas, splitterBounds.Offset(topLeft), splitterColor * opacity);
            }
            else
            {
                canvas.FillRect(splitterBounds.Offset(topLeft), splitterColor * opacity);
            }

            canvas.SaveState();
            canvas.ClipRect(firstBounds.Offset(topLeft));
            firstView?.Render(canvas, opacity);
            canvas.Restore();
            canvas.SaveState();
            canvas.ClipRect(secondBounds.Offset(topLeft));
            secondView?.Render(canvas, opacity);
            canvas.Restore();
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            if (firstView?.PreviewGesture(gesture) == true) return true;
            if (secondView?.PreviewGesture(gesture) == true) return true;

            return false;
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            if (gesture.PointerId.Kind == PointerKind.MouseMiddleButton || gesture.PointerId.Kind == PointerKind.MouseRightButton) return false;

            var splitterBounds = this.splitterBounds.Offset(ScreenBounds.TopLeft);

            switch (gesture.GestureType)
            {
                case GestureType.PointerDown:
                    if (lockedPointer == PointerId.None && EnableManipulation)
                    {
                        if (splitterBounds.Contains(gesture.Position))
                        {
                            downPosition = gesture.Position;
                            lockedPointer = gesture.PointerId;
                            CurrentState = ButtonState.Pushed;
                            SetCursorOverSplitter(gesture);
                            return true;
                        }
                    }
                    break;

                case GestureType.PointerUp:
                    if (lockedPointer == gesture.PointerId)
                    {
                        lockedPointer = PointerId.None;
                        if (splitterBounds.Contains(gesture.Position))
                        {
                            CurrentState = ButtonState.Hover;
                            SetCursorOverSplitter(gesture);
                        }
                        else
                        {
                            CurrentState = ButtonState.Normal;
                        }
                        return true;
                    }
                    break;

                case GestureType.PointerMove:
                    if (lockedPointer == PointerId.None && EnableManipulation)
                    {
                        if (splitterBounds.Contains(gesture.Position))
                        {
                            if (!hoverPointers.Contains(gesture.PointerId))
                            {
                                hoverPointers.Add(gesture.PointerId);
                            }
                            SetCursorOverSplitter(gesture);
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
                        var offset = gesture.Position - downPosition;
                        downPosition = gesture.Position;

                        float offsetValue = Orientation == Orientation.Horizontal ? offset.X : offset.Y;

                        var refSize = Orientation == Orientation.Vertical ? ScreenBounds.Height : ScreenBounds.Width;
                        var splitPosition = SplitPosition.Calculate(refSize);

                        splitPosition += offsetValue;
                        SplitPosition = SplitPositionFromValue(splitPosition);

                        SetCursorOverSplitter(gesture);

                        return true;
                    }
                    else if (gesture.PointerId.Kind == PointerKind.MousePointer)
                    {
                        if (lockedPointer != PointerId.None)
                        {
                            SetCursorOverSplitter(gesture);
                        }
                    }
                    break;

                case GestureType.CancelPointer:
                    if (lockedPointer == gesture.PointerId)
                    {
                        CurrentState = ButtonState.Normal;
                        lockedPointer = PointerId.None;
                        return true;
                    }
                    break;
            }


            if (firstView?.ProcessGesture(gesture) == true) return true;
            if (secondView?.ProcessGesture(gesture) == true) return true;

            return false;
        }

        private void SetCursorOverSplitter(Gesture gesture)
        {
            gesture.SetCursor = Orientation == Orientation.Vertical ? CursorType.HSplit : CursorType.VSplit;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            firstView?.Dispose();
            secondView?.Dispose();
        }
    }
}
