﻿using CrossX.Abstractions.IoC;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Core;
using CrossX.Framework.IoC;
using CrossX.Framework.UI.Global;
using CrossX.Skia;
using SkiaSharp.Views.Desktop;
using System;
using System.Windows.Forms;
using CrossX.Framework.Input;
using System.Numerics;

using DrawingSize = System.Drawing.Size;
using DrawingRectangle = System.Drawing.Rectangle;

namespace CrossX.WindowsForms
{
    internal partial class MainForm : Form, INativeWindow
    {
        private readonly ISkiaCanvas skiaCanvas;
        private readonly ICoreApplication app;
        private readonly ISystemDispatcher systemDispatcher;
        
        private FormBorderStyle lastFormBorderStyle = FormBorderStyle.Sizable;
        private DrawingRectangle lastNormalPosition;
        private FormWindowState lastNotFullscreenWindowState;

        public MainLoop MainLoop { get; }

        string INativeWindow.Title
        {
            set => systemDispatcher.BeginInvoke(() =>
            {
                Text = value;
            });
        }

        Size INativeWindow.MinSize
        {
            set => systemDispatcher.BeginInvoke(() =>
                {
                    var diff = Size - ClientSize;
                    MinimumSize = new DrawingSize(value.Width + diff.Width, value.Height + diff.Height);
                });
        }

        Size INativeWindow.MaxSize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     var diff = Size - ClientSize;
                     base.MaximumSize = new DrawingSize(value.Width + diff.Width, value.Height + diff.Height);
                 });
        }

        Size INativeWindow.Size
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     if(State == Framework.UI.Global.WindowState.Normal)
                     {
                         ClientSize = new DrawingSize(value.Width, value.Height);
                     }
                     lastNormalPosition = Bounds;
                 });
        }

        bool INativeWindow.CanResize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     FormBorderStyle = value ? FormBorderStyle.Sizable : FormBorderStyle.Fixed3D;
                     lastFormBorderStyle = FormBorderStyle;
                 });
        }

        bool INativeWindow.CanMaximize
        {
            set => systemDispatcher.BeginInvoke(() =>
                 {
                     MaximizeBox = value;
                 });
        }

        public WindowState State
        {
            get
            {
                if(FormBorderStyle == FormBorderStyle.None)
                {
                    return Framework.UI.Global.WindowState.Fullscreen;
                }

                if (WindowState == FormWindowState.Maximized)
                {
                    return Framework.UI.Global.WindowState.Maximized;
                }
                return Framework.UI.Global.WindowState.Normal;
            }

            set
            {
                
                systemDispatcher.BeginInvoke(() =>
                {
                    switch (value)
                    {
                        case Framework.UI.Global.WindowState.Fullscreen:
                            EnterFullscreen();
                            break;

                        case Framework.UI.Global.WindowState.Maximized:
                            LeaveFullscreen();
                            WindowState = FormWindowState.Maximized;
                            break;

                        case Framework.UI.Global.WindowState.Normal:
                            LeaveFullscreen();
                            WindowState = FormWindowState.Normal;
                            break;
                    }
                });
            }
        }

        CursorType INativeWindow.Cursor
        {
            set
            {
                if (cursorType != value)
                {
                    cursorType = value;
                    cursor = MapCursor(value);
                    systemDispatcher.BeginInvoke(() =>
                    {
                        Cursor = cursor;
                    });
                }
            }
        }

        private Cursor cursor = Cursors.Default;
        private CursorType cursorType = CursorType.Default;

        public MainForm(ICoreApplication app, IServicesProvider servicesProvider = null)
        {
            Visible = false;
            InitializeComponent();
            Visible = false;

            var scopeBuilder = new ScopeBuilder(servicesProvider);
            scopeBuilder.WithSkia();
            scopeBuilder.WithInstance(this).As<INativeWindow>();

            float dpi = 96;
            using (var gr = CreateGraphics())
            {
                dpi = gr.DpiX;
            }

            MainLoop = new MainLoop(app, skglControl.Invalidate, scopeBuilder, dpi);

            var services = scopeBuilder.Build();
            var factory = services.GetService<IObjectFactory>();
            systemDispatcher = services.GetService<ISystemDispatcher>();
            skiaCanvas = factory.Create<ISkiaCanvas>();

            skglControl.PaintSurface += SkglControl_PaintSurface;
            app.Initialize(services);
            this.app = app;
            
            MainLoop.Initialize();
            
            skglControl.KeyDown += SkglControl_KeyDown;
            skglControl.MouseMove += SkglControl_MouseMove;
            skglControl.MouseDown += SkglControl_MouseDown;
            skglControl.MouseUp += SkglControl_MouseUp;
            skglControl.MouseLeave += SkglControl_MouseLeave;

            app.MainWindowReady.WaitOne();
            skglControl.Invalidate();
            BringToFront();
            Visible = true;
        }

        private void SkglControl_MouseLeave(object sender, EventArgs args)
        {
            MainLoop.Dispatcher.BeginInvoke(() =>
            {
                app.OnPointerCancel(new PointerId(PointerKind.MouseLeftButton));
                app.OnPointerCancel(new PointerId(PointerKind.MouseRightButton));
                app.OnPointerCancel(new PointerId(PointerKind.MouseMiddleButton));
                app.OnPointerCancel(new PointerId(PointerKind.MousePointer));
            });
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
                MainLoop.Dispatcher.BeginInvoke(() =>
                {
                    app.OnPointerUp(new PointerId(pointerKind), position);
                });
            }
            Cursor = cursor;
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
                MainLoop.Dispatcher.BeginInvoke(() =>
                {
                    app.OnPointerDown(new PointerId(pointerKind), position);
                });
            }
            Cursor = cursor;
        }

        private void SkglControl_MouseMove(object sender, MouseEventArgs args)
        {
            var position = new Vector2(args.Location.X, args.Location.Y);
            var button = args.Button;

            MainLoop.Dispatcher.BeginInvoke(() =>
            {
                bool buttonPressed = false;
                if (button.HasFlag(MouseButtons.Left))
                {
                    app.OnPointerMove(new PointerId(PointerKind.MouseLeftButton), position);
                    buttonPressed = true;
                }

                if (button.HasFlag(MouseButtons.Right))
                {
                    app.OnPointerMove(new PointerId(PointerKind.MouseRightButton), position);
                    buttonPressed = true;
                }

                if (button.HasFlag(MouseButtons.Middle))
                {
                    app.OnPointerMove(new PointerId(PointerKind.MouseMiddleButton), position);
                    buttonPressed = true;
                }

                if (!buttonPressed)
                {
                    app.OnPointerMove(new PointerId(PointerKind.MousePointer), position);
                }
            });
            Cursor = cursor;
        }

        private void SkglControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs args)
        {
            skiaCanvas.Prepare(args.Surface.Canvas, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
            MainLoop.OnPaintSurface(skiaCanvas.Canvas);
        }

        protected override void OnHandleDestroyed(EventArgs args)
        {
            base.OnHandleDestroyed(args);
            MainLoop.Finish();
            skiaCanvas.Canvas.Dispose();
            app.Dispose();
        }

        private void SkglControl_KeyDown(object sender, KeyEventArgs args)
        {
            OnKeyDown(args);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if(args.KeyCode == Keys.F11)
            {
                if(State == Framework.UI.Global.WindowState.Fullscreen)
                {
                    LeaveFullscreen();
                }
                else
                {
                    EnterFullscreen();
                }
            }
        }

        private void EnterFullscreen()
        {
            if (WindowState == FormWindowState.Normal && FormBorderStyle != FormBorderStyle.None)
            {
                lastNormalPosition = Bounds;
            }

            if(FormBorderStyle != FormBorderStyle.None)
            {
                lastNotFullscreenWindowState = WindowState;
            }

            lastFormBorderStyle = FormBorderStyle;

            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.FromHandle(Handle).Bounds;
        }

        private void LeaveFullscreen()
        {
            FormBorderStyle = lastFormBorderStyle;
            Bounds = lastNormalPosition;
            WindowState = lastNotFullscreenWindowState;
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
