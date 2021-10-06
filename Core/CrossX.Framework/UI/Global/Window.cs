using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xx;

namespace CrossX.Framework.UI.Global
{
    public enum WindowState
    {
        Normal,
        Maximized,
        Fullscreen
    }

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
            set
            {
                if (nativeWindow == null) return;
                nativeWindow.Title = value;
            }
        }
        public Length Desktop_MinWidth
        {
            set
            {
                if (nativeWindow == null) return;
                minSize.Width = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.MinSize = minSize;
            }
        }

        public Length Desktop_MinHeight
        {
            set
            {
                if (nativeWindow == null) return;
                minSize.Height = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.MinSize = minSize;
            }
        }

        public Length Desktop_MaxWidth
        {
            set
            {
                if (nativeWindow == null) return;
                maxSize.Width = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.MaxSize = maxSize;
            }
        }

        public Length Desktop_MaxHeight
        {
            set
            {
                if (nativeWindow == null) return;
                maxSize.Height = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.MaxSize = maxSize;
            }
        }

        public Length Desktop_InitialWidth
        {
            set
            {
                if (nativeWindow == null) return;
                size.Width = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.Size = size;
            }
        }

        public Length Desktop_InitialHeight
        {
            set
            {
                if (nativeWindow == null) return;
                size.Height = (int)(value.Calculate() * UiUnit.PixelsPerUnit);
                nativeWindow.Size = size;
            }
        }

        public bool Desktop_CanMaximize
        {
            set
            {
                if (nativeWindow == null) return;
                nativeWindow.CanMaximize = value;
            }
        }

        public bool Desktop_CanResize
        {
            set
            {
                if (nativeWindow == null) return;
                nativeWindow.CanResize = value;
            }
        }

        public WindowState Desktop_StartMode
        {
            set
            {
                if (nativeWindow == null) return;
                nativeWindow.State = value;
            }
        }

        [XxSchemaBindable(true)]
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        Size minSize = Size.Empty;
        Size maxSize = new Size(100000, 100000);
        Size size = new Size(800, 600);
        private View rootView;
        private bool layoutInvalid;
        private readonly INativeWindow nativeWindow;
        private readonly IBindingService bindingService;
        private Color backgroundColor = Color.Black;

        private FrameLayout mainFrame;

        public Window(IServicesProvider servicesProvider)
        {
            servicesProvider.TryResolveInstance(out nativeWindow);
            bindingService = servicesProvider.GetService<IBindingService>();
            mainFrame = servicesProvider.GetService<IObjectFactory>().Create<FrameLayout>();
            mainFrame.Parent = this;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            if (elements.Count() != 1) throw new InvalidOperationException("Window must have only one child - root view.");
            RootView = (View)elements.First();

            mainFrame.Children.Add(RootView);
            RecalculateLayout();
        }

        [XxSchemaIgnore]
        public Size Size
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

        public SizeF ScaledSize => (Vector2)(SizeF)Size / UiUnit.PixelsPerUnit;

        public RectangleF ScreenBounds => new RectangleF(0, 0, ScaledSize.Width, ScaledSize.Height);

        bool IViewParent.DisplayVisible => true;

        public void Update(float timeDelta)
        {
            if(layoutInvalid) RecalculateLayout();
            mainFrame.Update(timeDelta);
        }

        public void Render(Canvas canvas)
        {
            canvas.SaveState();
            canvas.Transform(Matrix3x2.CreateScale(UiUnit.PixelsPerUnit));

            canvas.FillRect(ScreenBounds, BackgroundColor);

            mainFrame.Render(canvas);
            canvas.Restore();
        }

        public void Dispose()
        {
            mainFrame.Dispose();
            bindingService.RemoveBindings(this);
        }

        public void InvalidateLayout()
        {
            layoutInvalid = true;
        }

        private void RecalculateLayout()
        {
            layoutInvalid = false;
            mainFrame.Bounds = new RectangleF(Vector2.Zero, ScaledSize);
        }
    }
}
