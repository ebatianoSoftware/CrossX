using CrossX.Abstractions.Async;
using CrossX.Abstractions.Input;
using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.XxTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Xx;

namespace CrossX.Framework.UI.Global
{

    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class Window : UIBindingContext, IElementsContainer, IDisposable, IViewParent
    {
        public View RootView
        {
            get => rootView;
            private set
            {
                rootView = value;
            }
        }

        public FrameLayout MainFrame => mainFrame;

        Window IViewParent.Window => this;

        [XxSchemaBindable(true)]
        public string Title
        {
            get => title; set => SetProperty(ref title, value);
        }

        public string TypeName { get; set; }

        // TODO: Make posibility to have rendered menu (not native one)
        [XxSchemaBindable(true)]
        public IList Menu { get => menu; set => SetProperty(ref menu, value); }

        public Length Desktop_MinWidth
        {
            get => desktop_MinWidth;
            set => SetProperty(ref desktop_MinWidth, value);
        }

        public Length Desktop_MinHeight
        {
            get => desktop_MinHeight; set => SetProperty(ref desktop_MinHeight, value);
        }

        public Length Desktop_MaxWidth
        {
            get => desktop_MaxWidth; set => SetProperty(ref desktop_MaxWidth, value);
        }

        public Length Desktop_MaxHeight
        {
            get => desktop_MaxHeight; set => SetProperty(ref desktop_MaxHeight, value);
        }

        public Length Desktop_InitialWidth
        {
            get => desktop_InitialWidth; set => SetProperty(ref desktop_InitialWidth, value);
        }

        public Length Desktop_InitialHeight
        {
            get => desktop_InitialHeight; set => SetProperty(ref desktop_InitialHeight, value);
        }

        public bool Desktop_CanMaximize
        {
            get => desktop_CanMaximize; set => SetProperty(ref desktop_CanMaximize, value);
        }

        public bool Desktop_CanResize
        {
            get => desktop_CanResize; set => SetProperty(ref desktop_CanResize, value);
        }

        public WindowState Desktop_StartMode
        {
            get => desktop_StartMode; set => SetProperty(ref desktop_StartMode, value);
        }

        public bool Desktop_EnableMouse { get; set; }

        public bool Desktop_HasCaption { get; set; } = true;

        [XxSchemaBindable(true)]
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        public event Action Disposed;

        public INativeWindow NativeWindow { get; internal set; }

        public IFocusable CurrentFocus
        {
            get => currentFocus; 
            
            set
            {
                if (currentFocus?.ResignFocus() ?? true)
                {
                    if (SetProperty(ref currentFocus, value))
                    {
                        Redraw();
                    }
                }
            }
        }

        public ICommand WindowDisposedCommand { get; set; }

        private View rootView;
        private bool layoutInvalid;
        private readonly IBindingService bindingService;
        private Color backgroundColor = Color.Black;

        protected FrameLayout mainFrame { get; private set; }
        private string title = "";

        private Length desktop_MinWidth;
        private Length desktop_MinHeight;
        private Length desktop_MaxWidth;
        private Length desktop_MaxHeight;
        private Length desktop_InitialWidth = new Length(1280);
        private Length desktop_InitialHeight = new Length(720);
        private bool desktop_CanMaximize = true;
        private bool desktop_CanResize = true;
        private WindowState desktop_StartMode;
        protected readonly IAppValues appValues;
        private readonly IDispatcher dispatcher;
        private readonly IUiInput uiInput;
        protected NativeWindow Popup { get; private set; }

        public Window(IServicesProvider servicesProvider)
        {
            bindingService = servicesProvider.GetService<IBindingService>();
            mainFrame = servicesProvider.GetService<IObjectFactory>().Create<FrameLayout>();
            mainFrame.Parent = this;
            appValues = servicesProvider.GetService<IAppValues>();
            uiInput = servicesProvider.GetService<IUiInput>();
            dispatcher = servicesProvider.GetService<IDispatcher>();
            ServicesProvider = servicesProvider;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            if (elements.Count() != 1) throw new InvalidOperationException("Window must have only one child - root view.");
            RootView = (View)elements.First();

            mainFrame.Children.Add(RootView);
            RecalculateLayout();
        }

        [XxSchemaIgnore]
        public SizeF Size
        {
            get => size;

            set
            {
                if (SetProperty(ref size, value))
                {
                    RecalculateLayout();
                }
            }
        }

        SizeF size = new SizeF(800, 600);
        private IList menu;
        private bool isDirty;
        private IFocusable currentFocus;

        public RectangleF ScreenBounds => new RectangleF(NativeWindow.Bounds.TopLeft, Size);

        bool IViewParent.DisplayVisible => true;

        public bool IsDirty
        {
            get => isDirty || (Popup?.Window?.IsDirty ?? false);
            protected set => isDirty = value;
        }

        public IServicesProvider ServicesProvider { get; }
        private readonly GestureProcessor gestureProcessor = new GestureProcessor();

        public void Close()
        {
            NativeWindow?.Close();
        }

        public virtual void Update(float timeDelta)
        {
            if (layoutInvalid) RecalculateLayout();

            mainFrame.Update(timeDelta);
            Popup?.Window?.Update(timeDelta);

            if (Popup == null)
            {
                foreach (var val in Enum.GetValues(typeof(UiInputKey)))
                {
                    var button = (UiInputKey)val;

                    if (uiInput.IsJustPressed(button))
                    {
                        if (CurrentFocus == null || !CurrentFocus.HandleUiKey(button))
                        {
                            mainFrame.ProcessUiKey(button);
                        }
                    }
                }
            }
        }

        public virtual void Render(Canvas canvas)
        {
            IsDirty = false;
            canvas.FillRect(ScreenBounds, BackgroundColor);
            mainFrame.Render(canvas);

            if (Popup != null)
            {
                if (appValues.GetValue("SystemPopupOverlayColor") is Color color)
                {
                    canvas.FillRect(ScreenBounds, color);
                }
                Popup.Window.Render(canvas);
            }
        }

        public void Dispose()
        {
            mainFrame.Dispose();
            bindingService.RemoveBindings(this);
            WindowDisposedCommand?.Execute(this);
            Disposed?.Invoke();
        }

        public void InvalidateLayout()
        {
            layoutInvalid = true;
            Popup?.Window?.InvalidateLayout();
        }

        public void Redraw() => IsDirty = true;

        private void RecalculateLayout()
        {
            layoutInvalid = false;
            mainFrame.Bounds = new RectangleF(Vector2.Zero, Size);
        }

        public CursorType OnPointerDown(PointerId pointerId, Vector2 position)
        {
            if (Popup != null) return Popup.Window.OnPointerDown(pointerId, position);

            var gesture = gestureProcessor.OnPointerDown(pointerId, position);
            PropagateGesture(gesture);
            return gesture.SetCursor;
        }

        public CursorType OnPointerMove(PointerId pointerId, Vector2 position)
        {
            if (Popup != null) return Popup.Window.OnPointerMove(pointerId, position);

            var gesture = gestureProcessor.OnPointerMove(pointerId, position);
            PropagateGesture(gesture);
            return gesture.SetCursor;
        }

        public CursorType OnPointerUp(PointerId pointerId, Vector2 position)
        {
            if (Popup != null) return Popup.Window.OnPointerUp(pointerId, position);

            var gesture = gestureProcessor.OnPointerUp(pointerId, position);
            PropagateGesture(gesture);
            return gesture.SetCursor;
        }

        public CursorType OnPointerCancel(PointerId pointerId)
        {
            if (Popup != null) return Popup.Window.OnPointerCancel(pointerId);

            var gesture = gestureProcessor.OnPointerCancel(pointerId);
            PropagateGesture(gesture);
            return gesture.SetCursor;
        }

        private void PropagateGesture(Gesture gesture)
        {
            if (gesture == null) return;

            if (!MainFrame.PreviewGesture(gesture))
            {
                MainFrame.ProcessGesture(gesture);
            }
        }

        internal void AddPopup(NativeWindow nativeWindow)
        {
            if (Popup != null) throw new InvalidOperationException();
            Popup = nativeWindow;
            Redraw();
        }

        internal void RemovePopup(NativeWindow nativeWindow)
        {
            if (Popup != nativeWindow) throw new InvalidOperationException();

            dispatcher.EnqueueAction(() =>
            {
                Popup?.Window?.Dispose();
                Popup = null;
                Redraw();
            });
        }

        public bool NavigateFocus(UiInputKey key)
        {
            switch(key)
            {
                case UiInputKey.Select:
                case UiInputKey.Menu:
                case UiInputKey.MenuOrBack:
                case UiInputKey.Back:
                    return false;
            }

            var list = new List<IFocusable>();
            mainFrame.GetFocusables(list);

            if (currentFocus == null)
            {
                CurrentFocus = list.FirstOrDefault();
                return true;
            }

            IEnumerable<IFocusable> focusables = list;

            var bounds = currentFocus.ScreenBounds;
            switch (key)
            {
                case UiInputKey.Up:
                    focusables = focusables.Where(o => o.ScreenBounds.Bottom < bounds.Top);
                    break;

                case UiInputKey.Down:
                    focusables = focusables.Where(o => o.ScreenBounds.Top > bounds.Bottom);
                    break;

                case UiInputKey.Left:
                    focusables = focusables.Where(o => o.ScreenBounds.Right < bounds.Left);
                    break;

                case UiInputKey.Right:
                    focusables = focusables.Where(o => o.ScreenBounds.Left > bounds.Right);
                    break;
            }

            focusables = focusables.OrderBy(o => (o.ScreenBounds.Center - bounds.Center).LengthSquared());

            var newFocus = focusables.FirstOrDefault();
            if(newFocus != null)
            {
                CurrentFocus = newFocus;
                return true;
            }
            return false;
        }
    }
}
