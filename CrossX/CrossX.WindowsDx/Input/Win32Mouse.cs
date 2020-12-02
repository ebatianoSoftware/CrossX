using CrossX.Input;
using System;

using WinForm = System.Windows.Forms.Form;
using WinCursor = System.Windows.Forms.Cursor;
using WinControl = System.Windows.Forms.Control;
using WinMouseButtons = System.Windows.Forms.MouseButtons;

namespace CrossX.Windows.Input
{
    internal class Win32Mouse : ITouchPanel, IMouse
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct TPoint
        {
            public int X;
            public int Y;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ClientToScreen(System.IntPtr hWnd, ref TPoint lpPoint);

        private readonly WinForm form;

        public Vector2 Position
        {
            get
            {
                var pos = WinCursor.Position;
                pos = form.PointToClient(pos);
                return new Vector2(pos.X, pos.Y);
            }

            set
            {
                var tpoint = new TPoint { X = (int)value.X, Y = (int)value.Y };
                ClientToScreen(form.Handle, ref tpoint);
                WinCursor.Position = new System.Drawing.Point(tpoint.X, tpoint.Y);
            }
        }

        public float WheelDelta => 0;

        public MouseCaps Caps => MouseCaps.LButton | MouseCaps.MButton | MouseCaps.RButton;

        public CursorType Cursor 
        { 
            get => cursor;
            set
            {
                cursor = value;
                //WinCursor.Hide();
            }
        }
        public event Action<TouchPoint> PointerDown;
        public event Action<TouchPoint> PointerUp;
        public event Action<TouchPoint> PointerMove;
        public event Action<TouchPoint> PointerRemoved;
        public event Action<long, object> PointerCaptured;

        public event MouseEvent ButtonDown;
        public event MouseEvent ButtonUp;
        public event MouseEvent MouseMove;
        public event MouseEvent MouseEnter;
        public event Action MouseLeave;

        public void CapturePointer(long id, object capturedBy)
        {
            PointerCaptured?.Invoke(id, capturedBy);
        }

        private MouseButtons previousMouseButtons = MouseButtons.None;
        private CursorType cursor = CursorType.Arrow;
        private const long MouseTouchId = 1;

        public Win32Mouse(WinForm form)
        {
            this.form = form;

            this.form.MouseDown += OnMouseDown;
            this.form.MouseUp += OnMouseUp;
            this.form.MouseMove += OnMouseMove;
            this.form.MouseLeave += OnMouseLeave;
            this.form.MouseEnter += OnMouseEnter;
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            MouseEnter?.Invoke(Position, FromMouseButtons(WinControl.MouseButtons));
        }

        private void OnMouseLeave(object sender, EventArgs args)
        {
            MouseLeave?.Invoke();
            PointerRemoved?.Invoke(new TouchPoint { Id = MouseTouchId });
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs args)
        {
            MouseMove?.Invoke(new Vector2(args.X, args.Y), FromMouseButtons(args.Button));

            if (args.Button == WinMouseButtons.Left)
            {
                PointerMove?.Invoke(new TouchPoint
                {
                    Id = MouseTouchId,
                    Position = new Vector2(args.X, args.Y),
                    State = KeyBtnState.Down
                });
            }
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs args)
        {
            ButtonDown?.Invoke(new Vector2(args.X, args.Y), FromMouseButtons(args.Button));

            if (args.Button == WinMouseButtons.Left)
            {
                PointerDown?.Invoke(new TouchPoint
                {
                    Id = MouseTouchId,
                    Position = new Vector2(args.X, args.Y),
                    State = KeyBtnState.JustPressed
                });
            }
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs args)
        {
            ButtonUp?.Invoke(new Vector2(args.X, args.Y), FromMouseButtons(args.Button));

            if (args.Button == WinMouseButtons.Left)
            {
                var tp = new TouchPoint
                {
                    Id = MouseTouchId,
                    Position = new Vector2(args.X, args.Y),
                    State = KeyBtnState.JustReleased
                };

                PointerUp?.Invoke(tp);
                PointerRemoved?.Invoke(tp);
            }
        }

        public KeyBtnState GetButtonState(MouseButtons button)
        {
            bool isNowDown = FromMouseButtons(WinControl.MouseButtons).HasFlag(button);
            bool wasDown = previousMouseButtons.HasFlag(button);

            return isNowDown ? (wasDown ? KeyBtnState.Down : KeyBtnState.JustPressed)
                             : (wasDown ? KeyBtnState.JustReleased : KeyBtnState.Up);
        }

        private MouseButtons FromMouseButtons(WinMouseButtons btns)
        {
            var xxBtns = MouseButtons.None;

            if (btns.HasFlag(WinMouseButtons.Left)) xxBtns |= MouseButtons.Left;
            if (btns.HasFlag(WinMouseButtons.Right)) xxBtns |= MouseButtons.Right;
            if (btns.HasFlag(WinMouseButtons.Middle)) xxBtns |= MouseButtons.Middle;
            return xxBtns;
        }

        public void Update()
        {
            previousMouseButtons = FromMouseButtons(WinControl.MouseButtons);
        }
    }
}
