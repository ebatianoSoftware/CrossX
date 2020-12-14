// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace CrossX.WindowsUniversal.Input
{
    internal class UwpMouse : IMouse
    {
        const uint MousePointerId = 1;
        private readonly CoreWindow coreWindow;

        public MouseCaps Caps => MouseCaps.LButton | MouseCaps.RButton | MouseCaps.MButton;

        private MouseButtons previousButtons = 0;
        private MouseButtons currentButtons = 0;

        public event MouseEvent ButtonDown;
        public event MouseEvent ButtonUp;
        public event MouseEvent MouseMove;
        public event MouseEvent MouseEnter;
        public event Action MouseLeave;

        private readonly Dictionary<CursorType,CoreCursor> cursors = new Dictionary<CursorType,CoreCursor>();

        private double RawPixelsPerViewPixel => DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        public Vector2 Position
        {
            get
            {
                return PositionFromWindowPos(coreWindow.PointerPosition);
            }

            set
            {
                var displayInformation = DisplayInformation.GetForCurrentView();

                var pos = new Windows.Foundation.Point(value.X / displayInformation.RawPixelsPerViewPixel,
                    value.Y / displayInformation.RawPixelsPerViewPixel);

                coreWindow.PointerPosition = pos;
            }
        }

        private Vector2 PositionFromWindowPos(Windows.Foundation.Point pt)
        {
            return new Vector2((float)(pt.X - coreWindow.Bounds.Left), (float)(pt.Y - coreWindow.Bounds.Top)) * (float)RawPixelsPerViewPixel;
        }

        public float WheelDelta { get; private set; }
        public CursorType Cursor 
        { 
            get => cursor;
            set
            {
                if (cursor == value) return;

                cursor = value;

                if (value == CursorType.None)
                {
                    coreWindow.PointerCursor = null;
                }
                else
                {
                    if(!cursors.TryGetValue(value, out var coreCursor))
                    {
                        coreCursor = new CoreCursor((CoreCursorType)value, 0);
                        cursors.Add(value, coreCursor);
                    }

                    coreWindow.PointerCursor = coreCursor;
                }
            }
        }

        private MouseButtons lastButton;
        private CursorType cursor;

        public UwpMouse(CoreWindow window)
        {
            coreWindow = window;
            coreWindow.PointerEntered += OnPointerEntered;
            coreWindow.PointerExited += OnPointerExited;
            coreWindow.PointerMoved += OnPointerMoved;
            coreWindow.PointerPressed += OnPointerPressed;
            coreWindow.PointerReleased += OnPointerReleased;
            coreWindow.PointerWheelChanged += CoreWindow_PointerWheelChanged;
        }

        private void CoreWindow_PointerWheelChanged(CoreWindow sender, PointerEventArgs args)
        {
            WheelDelta += args.CurrentPoint.Properties.MouseWheelDelta;
        }

        private MouseButtons ButtonFromPointerArgs(PointerEventArgs args)
        {
            MouseButtons button = 0;

            if (args.CurrentPoint.Properties.IsLeftButtonPressed)
            {
                button = MouseButtons.Left;
            }
            else if (args.CurrentPoint.Properties.IsRightButtonPressed)
            {
                button = MouseButtons.Right;
            }
            else if (args.CurrentPoint.Properties.IsMiddleButtonPressed)
            {
                button = MouseButtons.Middle;
            }
            return button;
        }

        private void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.PointerId != MousePointerId) return;

            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
            var button = lastButton;
            currentButtons &= ~button;

            ButtonUp?.Invoke(pos, button);
            lastButton = MouseButtons.None;
        }

        private void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.PointerId != MousePointerId) return;

            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
            var button = ButtonFromPointerArgs(args);

            lastButton = button;

            currentButtons |= button;
            ButtonDown?.Invoke(pos, button);
        }

        private void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.PointerId != MousePointerId) return;

            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
            MouseMove?.Invoke(pos, currentButtons);
        }

        private void OnPointerExited(CoreWindow sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.PointerId != MousePointerId) return;

            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
            lastButton = MouseButtons.None;
            MouseLeave?.Invoke();
        }

        private void OnPointerEntered(CoreWindow sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.PointerId != MousePointerId) return;

            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
            lastButton = MouseButtons.None;
            MouseEnter?.Invoke(pos, MouseButtons.None);
        }

        public void Update()
        {
            previousButtons = currentButtons;
            WheelDelta = 0;
        }

        public KeyBtnState GetButtonState(MouseButtons button)
        {
            var state = currentButtons.HasFlag(button) ? KeyBtnState.Down : KeyBtnState.Up;
            state |= (currentButtons & button) != (previousButtons & button) ? KeyBtnState.JustSwitched : 0;
            return state;
        }
    }
}
