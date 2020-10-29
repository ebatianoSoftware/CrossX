//// MIT License - Copyright © ebatianoSoftware
//// This file is subject to the terms and conditions defined in
//// file 'LICENSE.txt', which is part of this source code package.

//using EbatianoSoftware.CrossX.Input;
//using EbatianoSoftware.CrossX.Input.Mouse;
//using EbatianoSoftware.CrossX.Primitives;
//using System;
//using Windows.Graphics.Display;
//using Windows.UI.Core;

//namespace EbatianoSoftware.CrossX.WindowsUniversal.Input
//{
//    internal class UwpMouse : IMouse
//    {
//        const uint MousePointerId = 1;
//        private readonly CoreWindow _window;

//        public MouseCaps Caps => MouseCaps.LButton | MouseCaps.RButton | MouseCaps.MButton;

//        private MouseButtons _previousButtons = 0;
//        private MouseButtons _currentButtons = 0;

//        public event MouseEvent ButtonDown;
//        public event MouseEvent ButtonUp;
//        public event MouseEvent MouseMove;
//        public event MouseEvent MouseEnter;
//        public event Action MouseLeave;

//        private double RawPixelsPerViewPixel => DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

//        public PointF Position
//        {
//            get
//            {
//                return PositionFromWindowPos(_window.PointerPosition);
//            }

//            set
//            {
//                var displayInformation = DisplayInformation.GetForCurrentView();

//                var pos = new Windows.Foundation.Point(value.X / displayInformation.RawPixelsPerViewPixel,
//                    value.Y / displayInformation.RawPixelsPerViewPixel);

//                _window.PointerPosition = pos;
//            }
//        }

//        private PointF PositionFromWindowPos(Windows.Foundation.Point pt)
//        {
//            return new PointF(pt.X- _window.Bounds.Left, pt.Y- _window.Bounds.Top) * RawPixelsPerViewPixel;   
//        }

//        public float WheelDelta => throw new NotImplementedException();

//        private MouseButtons _lastButton;

//        public UwpMouse(CoreWindow window)
//        {
//            _window = window;
//            _window.PointerEntered += OnPointerEntered;
//            _window.PointerExited += OnPointerExited;
//            _window.PointerMoved += OnPointerMoved;
//            _window.PointerPressed += OnPointerPressed;
//            _window.PointerReleased += OnPointerReleased;
//        }

//        private MouseButtons ButtonFromPointerArgs(PointerEventArgs args)
//        {
//            MouseButtons button = 0;
            
//            if (args.CurrentPoint.Properties.IsLeftButtonPressed)
//            {
//                button = MouseButtons.Left;
//            }
//            else if (args.CurrentPoint.Properties.IsRightButtonPressed)
//            {
//                button = MouseButtons.Right;
//            }
//            else if (args.CurrentPoint.Properties.IsMiddleButtonPressed)
//            {
//                button = MouseButtons.Middle;
//            }
//            return button;
//        }

//        private void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
//        {
//            if (args.CurrentPoint.PointerId != MousePointerId) return;
            
//            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
//            var button = _lastButton;
//            _currentButtons &= ~button;

//            ButtonUp?.Invoke(pos, button);
//            _lastButton = MouseButtons.None;
//        }

//        private void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
//        {
//            if (args.CurrentPoint.PointerId != MousePointerId) return;

//            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
//            var button = ButtonFromPointerArgs(args);

//            _lastButton = button;

//            _currentButtons |= button;
//            ButtonDown?.Invoke(pos, button);
//        }

//        private void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
//        {
//            if (args.CurrentPoint.PointerId != MousePointerId) return;

//            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
//            MouseMove?.Invoke(pos, _currentButtons);
//        }

//        private void OnPointerExited(CoreWindow sender, PointerEventArgs args)
//        {
//            if (args.CurrentPoint.PointerId != MousePointerId) return;

//            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
//            _lastButton = MouseButtons.None;
//            MouseLeave?.Invoke();
//        }

//        private void OnPointerEntered(CoreWindow sender, PointerEventArgs args)
//        {
//            if (args.CurrentPoint.PointerId != MousePointerId) return;

//            var pos = PositionFromWindowPos(args.CurrentPoint.Position);
//            _lastButton = MouseButtons.None;
//            MouseEnter?.Invoke(pos, MouseButtons.None);
//        }

//        public void Update()
//        {
//            _previousButtons = _currentButtons;
//        }

//        public KeyBtnState GetButtonState(MouseButtons button)
//        {
//            var state = _currentButtons.HasFlag(button) ? KeyBtnState.Down : KeyBtnState.Up;
//            state |= (_currentButtons & button) != (_previousButtons & button) ? KeyBtnState.JustSwitched : 0;
//            return state;
//        }
//    }
//}
