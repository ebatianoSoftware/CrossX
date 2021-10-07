using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Global;
using CrossX.Skia;
using System;
using System.Numerics;
using System.Windows.Forms;

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

        protected override bool EnableManipulation { set => skglControl.Enabled = value; }

        public WindowHost(Window window, IObjectFactory objectFactory, CreateWindowMode createMode)
        {
            InitializeComponent();
            Window = window;

            skiaCanvas = objectFactory.Create<ISkiaCanvas>();

            //if (createMode == CreateWindowMode.Modal || createMode == CreateWindowMode.ChildToMain)
            //{
            //    FormBorderStyle = window.Desktop_CanResize ? FormBorderStyle.SizableToolWindow : FormBorderStyle.FixedToolWindow;
            //}
            //else
            {
                FormBorderStyle = window.Desktop_CanResize ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            }

            ClientSize = new System.Drawing.Size(window.Desktop_InitialWidth.Pixels, window.Desktop_InitialHeight.Pixels);

            MaximizeBox = window.Desktop_CanMaximize;
            MinimizeBox = true;

            Text = Window.Title;

            var diff = Size - ClientSize;
            MinimumSize = new System.Drawing.Size(window.Desktop_MinWidth.Pixels + diff.Width, window.Desktop_MinHeight.Pixels + diff.Height);

            skglControl.PaintSurface += SkglControl_PaintSurface;
            skglControl.SizeChanged += SkglControl_SizeChanged;
            skglControl.MouseMove += SkglControl_MouseMove;
            skglControl.MouseDown += SkglControl_MouseDown;
            skglControl.MouseUp += SkglControl_MouseUp;
            skglControl.MouseLeave += SkglControl_MouseLeave;

            window.CloseNativeWindow += Window_CloseNativeWindow;

            SkglControl_SizeChanged(this, EventArgs.Empty);
        }

        private void Window_CloseNativeWindow()
        {
            Close();
            DestroyHandle();
            Dispose();
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
                CursorType = Window.OnPointerDown(new PointerId(pointerKind), position);
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
            switch (cursorType)
            {
                case CursorType.Default:
                    return Cursors.Default;

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
            }
            return Cursors.Default;
        }
    }
}
