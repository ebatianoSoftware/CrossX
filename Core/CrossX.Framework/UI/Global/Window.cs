using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
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
    public class Window : BindingContext, IElementsContainer, IDisposable, IViewParent
    {
        public View RootView
        {
            get => rootView;
            private set
            {
                rootView = value;
            }
        }

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

        public object BindingContext { get => bindingContext; set => SetProperty(ref bindingContext, value); }

        Size minSize = Size.Empty;
        Size maxSize = new Size(100000, 100000);
        Size size = new Size(800, 600);
        private View rootView;
        private object bindingContext;
        private bool layoutInvalid;
        private readonly INativeWindow nativeWindow;
        private readonly IBindingService bindingService;

        public Window(IServicesProvider servicesProvider)
        {
            servicesProvider.TryResolveInstance(out nativeWindow);
            bindingService = servicesProvider.GetService<IBindingService>();
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            if (elements.Count() != 1) throw new InvalidOperationException("Window must have only one child - root view.");
            RootView = (View)elements.First();
            RootView.Parent = this;
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

        public void Update(float timeDelta)
        {
            if(layoutInvalid) RecalculateLayout();
            RootView?.Update(timeDelta);
        }

        public void Render(Canvas canvas)
        {
            canvas.SaveState();
            canvas.Transform(Matrix3x2.CreateScale(UiUnit.PixelsPerUnit));
            RootView?.Render(canvas);
            canvas.Restore();
        }

        public void Dispose()
        {
            RootView.Dispose();
            bindingService.RemoveBindings(this);
        }

        public void InvalidateLayout()
        {
            layoutInvalid = true;
        }

        private void RecalculateLayout()
        {
            layoutInvalid = false;
            var child = RootView;
            var size = child.CalculateSize(ScaledSize);
            var position = child.CalculatePosition(size, ScaledSize);
            child.Bounds = new RectangleF(position, size);
        }
    }
}
