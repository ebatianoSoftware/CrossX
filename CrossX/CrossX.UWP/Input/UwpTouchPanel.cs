//using EbatianoSoftware.CrossX.Input;
//using EbatianoSoftware.CrossX.Input.Touch;
//using EbatianoSoftware.CrossX.Primitives;
//using System;
//using System.Collections.Generic;
//using Windows.Graphics.Display;
//using Windows.UI.Core;

//namespace EbatianoSoftware.CrossX.WindowsUniversal.Input
//{
//    internal class UwpTouchPanel : ITouchPanel
//    {
//        private readonly CoreWindow _window;

//        private readonly List<TouchPoint> _touchState = new List<TouchPoint>();

//        public event Action<TouchPoint> PointerDown;
//        public event Action<TouchPoint> PointerUp;
//        public event Action<TouchPoint> PointerMove;
//        public event Action<TouchPoint> PointerRemoved;
//        public event Action<long, object> PointerCaptured;

//        public void CapturePointer(long id, object capturedBy)
//        {
//            PointerCaptured?.Invoke(id, capturedBy);
//        }
//        private double RawPixelsPerViewPixel => DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

//        private PointF PositionFromWindowPos(Windows.Foundation.Point pt)
//        {
//            return new PointF(pt.X, pt.Y) * RawPixelsPerViewPixel;
//        }

//        public UwpTouchPanel(CoreWindow window)
//        {
//            _window = window;
//            _window.PointerEntered += OnPointerEntered;
//            _window.PointerExited += OnPointerExited;
//            _window.PointerMoved += OnPointerMoved;
//            _window.PointerPressed += OnPointerPressed;
//            _window.PointerReleased += OnPointerReleased;
//        }

//        private void OnPointerEntered(CoreWindow sender, PointerEventArgs args)
//        {
//            var pt = args.CurrentPoint;
//            if (pt.IsInContact)
//            {
//                var index = _touchState.FindIndex(o => o.Id == pt.PointerId);
                
//                if(index <= 0)
//                {
//                    index = _touchState.Count;
//                    _touchState.Add(new TouchPoint
//                    {
//                        Id = pt.PointerId,
//                        Position = PositionFromWindowPos(pt.Position),
//                        State = KeyBtnState.Down
//                    });
//                }
//            }
//        }

//        private void OnPointerExited(CoreWindow sender, PointerEventArgs args)
//        {
//            var pt = args.CurrentPoint;
//            var index = _touchState.FindIndex(o => o.Id == pt.PointerId);

//            if (index >= 0)
//            {
//                var ts = _touchState[index];
//                _touchState.RemoveAt(index);

//                PointerRemoved?.Invoke(ts);
//            }
//        }

//        private void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
//        {
//            var pt = args.CurrentPoint;
//            if (pt.IsInContact)
//            {
//                var index = _touchState.FindIndex(o => o.Id == pt.PointerId);

//                if (index <= 0)
//                {
//                    index = _touchState.Count;
//                    _touchState.Add(new TouchPoint { Id = pt.PointerId });
//                }

//                var ts = _touchState[index];
//                ts.State = KeyBtnState.Down;
//                ts.Position = PositionFromWindowPos(pt.Position);

//                _touchState[index] = ts;
//                PointerMove?.Invoke(ts);
//            }
//        }

//        private void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
//        {
//            var pt = args.CurrentPoint;
            
//            var index = _touchState.FindIndex(o => o.Id == pt.PointerId);

//            if (index <= 0)
//            {
//                index = _touchState.Count;
//                _touchState.Add(new TouchPoint { Id = pt.PointerId });
//            }

//            var ts = _touchState[index];
//            ts.State = KeyBtnState.JustPressed;
//            ts.Position = PositionFromWindowPos(pt.Position);
//            _touchState[index] = ts;

//            PointerDown?.Invoke(ts);
//        }

//        private void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
//        {
//            var pt = args.CurrentPoint;
            
//            var index = _touchState.FindIndex(o => o.Id == pt.PointerId);

//            if (index >= 0)
//            {
//                var ts = _touchState[index];
//                ts.State = KeyBtnState.JustReleased;
//                ts.Position = PositionFromWindowPos(pt.Position);

//                _touchState[index] = ts;

//                PointerUp?.Invoke(ts);
//            }
//        }

//        public void Update()
//        {
//            for(var idx = 0; idx < _touchState.Count;)
//            {
//                var ts = _touchState[idx];

//                if (ts.State == KeyBtnState.Up)
//                {
//                    PointerRemoved?.Invoke(ts);
//                    _touchState.RemoveAt(idx);
//                    continue;
//                }

//                if (ts.State == KeyBtnState.JustReleased)
//                {
//                    ts.State = KeyBtnState.Up;
//                }

//                if (ts.State == KeyBtnState.JustPressed)
//                {
//                    ts.State = KeyBtnState.Down;
//                }

//                _touchState[idx] = ts;
//                ++idx;
//            }
//        }
//    }
//}
