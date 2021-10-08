using CrossX.Abstractions.IoC;
using CrossX.Framework;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Global;
using CrossX.Skia;
using System;
using System.Numerics;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;

namespace CrossX.WindowsForms
{
    public partial class WindowHost : BaseHostForm
    {
        public readonly Window Window;
        private readonly ISkiaCanvas skiaCanvas;

        private CursorType cursorType;

        protected override CursorType CursorType
        {
            set
            {
                if (cursorType != value)
                {
                    Cursor = MapCursor(value);
                    cursorType = value;
                }
            }
        }

        protected override bool EnableManipulation {set => skglControl.Enabled = value; }

        public WindowHost(Window window, IObjectFactory objectFactory)
        {
            InitializeComponent();
            
            Window = window;
            BackColor = System.Drawing.Color.FromArgb(Window.BackgroundColor.R, Window.BackgroundColor.G, Window.BackgroundColor.B);
            skiaCanvas = objectFactory.Create<ISkiaCanvas>();

            FormBorderStyle = window.Desktop_HasCaption ? FormBorderStyle.Sizable : FormBorderStyle.None;
            ClientSize = new System.Drawing.Size(window.Desktop_InitialWidth.Pixels, window.Desktop_InitialHeight.Pixels);

            MaximizeBox = window.Desktop_CanMaximize;
            MinimizeBox = true;

            if (!window.Desktop_HasCaption)
            {
                ControlBox = false;
            }
            
            Text = Window.Title;

            var diff = Size - ClientSize;
            MinimumSize = new System.Drawing.Size(window.Desktop_MinWidth.Pixels + diff.Width, window.Desktop_MinHeight.Pixels + diff.Height);

            skglControl.Hide();
            skglControl.PaintSurface += SkglControl_PaintSurface;
            skglControl.SizeChanged += SkglControl_SizeChanged;
            skglControl.MouseMove += SkglControl_MouseMove;
            skglControl.MouseDown += SkglControl_MouseDown;
            skglControl.MouseUp += SkglControl_MouseUp;
            skglControl.MouseLeave += SkglControl_MouseLeave;

            SkglControl_SizeChanged(this, EventArgs.Empty);
            Shown += WindowHost_Shown;
        }

        private void WindowHost_Shown(object sender, EventArgs e)
        {
            skglControl.Show();
            Shown -= WindowHost_Shown;
        }

        public void Redraw() => skglControl.Invalidate();

        private void SkglControl_SizeChanged(object sender, EventArgs _)
        {
            Window.Size = new SizeF(skglControl.Size.Width / UiUnit.PixelsPerUnit, skglControl.Size.Height / UiUnit.PixelsPerUnit);
        }

        private void SkglControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs args)
        {
            skiaCanvas.Prepare(args.Surface.Canvas, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
            args.Surface.Canvas.Scale(UiUnit.PixelsPerUnit);
            Window.Render(skiaCanvas.Canvas);
            args.Surface.Canvas.ResetMatrix();
        }

        DrawingPoint lastMousePosition;
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
            if (!Capture) return;

            var locationX = Location.X + args.Location.X - lastMousePosition.X;
            var locationY = Location.Y + args.Location.Y - lastMousePosition.Y;

            Location = new DrawingPoint(locationX, locationY);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Capture = false;
        }

        private void SkglControl_MouseLeave(object sender, EventArgs args)
        {
            Window.OnPointerCancel(new PointerId(PointerKind.MouseLeftButton));
            Window.OnPointerCancel(new PointerId(PointerKind.MouseRightButton));
            Window.OnPointerCancel(new PointerId(PointerKind.MouseMiddleButton));
            Window.OnPointerCancel(new PointerId(PointerKind.MousePointer));
        }

        private void SkglControl_MouseUp(object sender, MouseEventArgs args)
        {
            var position = new Vector2(args.Location.X, args.Location.Y);
            PointerKind pointerKind = 0;
            switch (args.Button)
            {
                case MouseButtons.Left:
                    pointerKind = PointerKind.MouseLeftButton;
                    break;

                case MouseButtons.Right:
                    pointerKind = PointerKind.MouseRightButton;
                    break;

                case MouseButtons.Middle:
                    pointerKind = PointerKind.MouseMiddleButton;
                    break;
            }

            if (pointerKind != 0)
            {
                CursorType = Window.OnPointerUp(new PointerId(pointerKind), position);
            }
        }

        private void SkglControl_MouseDown(object sender, MouseEventArgs args)
        {
            var position = new Vector2(args.Location.X, args.Location.Y);
            PointerKind pointerKind = 0;
            switch (args.Button)
            {
                case MouseButtons.Left:
                    pointerKind = PointerKind.MouseLeftButton;
                    break;

                case MouseButtons.Right:
                    pointerKind = PointerKind.MouseRightButton;
                    break;

                case MouseButtons.Middle:
                    pointerKind = PointerKind.MouseMiddleButton;
                    break;
            }

            if (pointerKind != 0)
            {
                var cursorType = Window.OnPointerDown(new PointerId(pointerKind), position);
                CursorType = cursorType;
                if (cursorType == CursorType.NativeDrag)
                {
                    Capture = true;
                    lastMousePosition = args.Location;
                }
                else
                {
                    
                }
            }
        }

        private void SkglControl_MouseMove(object sender, MouseEventArgs args)
        {
            var position = new Vector2(args.Location.X, args.Location.Y);
            var button = args.Button;


            bool buttonPressed = false;
            if (button.HasFlag(MouseButtons.Left))
            {
                CursorType = Window.OnPointerMove(new PointerId(PointerKind.MouseLeftButton), position);
                buttonPressed = true;
            }

            if (button.HasFlag(MouseButtons.Right))
            {
                CursorType = Window.OnPointerMove(new PointerId(PointerKind.MouseRightButton), position);
                buttonPressed = true;
            }

            if (button.HasFlag(MouseButtons.Middle))
            {
                CursorType = Window.OnPointerMove(new PointerId(PointerKind.MouseMiddleButton), position);
                buttonPressed = true;
            }

            if (!buttonPressed)
            {
                CursorType = Window.OnPointerMove(new PointerId(PointerKind.MousePointer), position);
            }
        }

        protected override void OnHandleDestroyed(EventArgs args)
        {
            base.OnHandleDestroyed(args);
            Window.Dispose();
        }

        private Cursor MapCursor(CursorType cursorType)
        {
            if (skglControl.Enabled == false) return Cursors.Default;

            switch (cursorType)
            {
                case CursorType.Default:
                    return Cursors.Arrow;

                case CursorType.Cross:
                    return Cursors.Cross;

                case CursorType.Hand:
                    return Cursors.Hand;

                case CursorType.Move:
                    return Cursors.SizeAll;

                case CursorType.Wait:
                    return Cursors.WaitCursor;

                case CursorType.VSplit:
                    return Cursors.VSplit;

                case CursorType.HSplit:
                    return Cursors.HSplit;

                case CursorType.SizeNS:
                    return Cursors.SizeNS;

                case CursorType.SizeWE:
                    return Cursors.SizeWE;

                case CursorType.IBeam:
                    return Cursors.IBeam;

                case CursorType.NativeDrag:
                    return Cursors.SizeAll;
            }
            
            return Cursors.Default;
        }

        protected override void WndProc(ref Message m)
        {
            if (!Window.Desktop_CanResize)
            {
                switch (m.Msg)
                {
                    case 0x0084:
                        m.Result = (IntPtr)0x01;
                        return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
