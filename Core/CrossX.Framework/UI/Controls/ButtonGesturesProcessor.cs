using CrossX.Framework.Input;
using System;
using System.Collections.Generic;

namespace CrossX.Framework.UI.Controls
{
    public class ButtonGesturesProcessor
    {
        private PointerId lockedPointer = PointerId.None;
        private HashSet<PointerId> hoverPointers = new HashSet<PointerId>();

        private readonly Action<ButtonState> setStateAction;
        private readonly Action clickAction;
        private readonly Action<Gesture> onDownAction;
        private readonly Action<Gesture> onMoveAction;
        private readonly Action<Gesture> onUpAction;
        private readonly Action<Gesture> onHoveredAction;
        private ButtonState currentState;

        private ButtonState CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                setStateAction?.Invoke(value);
            }
        }

        public ButtonGesturesProcessor(Action<ButtonState> setStateAction, Action clickAction = null, 
            Action<Gesture> onDownAction = null, Action<Gesture> onMoveAction = null, Action<Gesture> onUpAction = null,
            Action<Gesture> onHoveredAction = null)
        {
            this.setStateAction = setStateAction;
            this.clickAction = clickAction;
            this.onDownAction = onDownAction;
            this.onMoveAction = onMoveAction;
            this.onUpAction = onUpAction;
            this.onHoveredAction = onHoveredAction;
        }

        public bool ProcessGesture(Gesture gesture, RectangleF bounds, bool enabled)
        {
            if (gesture.PointerId.Kind == PointerKind.MouseMiddleButton || gesture.PointerId.Kind == PointerKind.MouseRightButton) return false;

            switch (gesture.GestureType)
            {
                case GestureType.PointerDown:
                    if (lockedPointer == PointerId.None && enabled)
                    {
                        if (bounds.Contains(gesture.Position))
                        {
                            lockedPointer = gesture.PointerId;
                            CurrentState = ButtonState.Pushed;
                            onDownAction?.Invoke(gesture);
                            return true;
                        }
                    }
                    break;

                case GestureType.PointerUp:
                    if (lockedPointer == gesture.PointerId)
                    {
                        lockedPointer = PointerId.None;
                        if (bounds.Contains(gesture.Position))
                        {
                            CurrentState = ButtonState.Hover;
                            clickAction?.Invoke();
                        }
                        else
                        {
                            setStateAction(ButtonState.Normal);
                        }
                        onUpAction?.Invoke(gesture);
                        return true;
                    }
                    break;

                case GestureType.PointerMove:
                    if (lockedPointer == PointerId.None)
                    {
                        if (bounds.Contains(gesture.Position))
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

                        if(newState == ButtonState.Hover)
                        {
                            onHoveredAction?.Invoke(gesture);
                        }

                        if (newState != CurrentState)
                        {
                            CurrentState = newState;
                            return true;
                        }
                    }
                    else if (lockedPointer == gesture.PointerId)
                    {
                        if(onMoveAction != null)
                        {
                            onMoveAction(gesture);
                        }
                        else if (bounds.Contains(gesture.Position))
                        {
                            CurrentState = ButtonState.Pushed;
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
                        CurrentState = ButtonState.Normal;
                        lockedPointer = PointerId.None;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public void Reset()
        {
            CurrentState = ButtonState.Normal;
            lockedPointer = PointerId.None;
        }
    }
}
