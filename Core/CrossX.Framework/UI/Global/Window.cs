using CrossX.Abstractions.IoC;
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
    public class Window : IElementsContainer
    {
        public View RootView { get; private set; }

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

        Size minSize = Size.Empty;
        Size maxSize = new Size(100000, 100000);
        Size size = new Size(800, 600);

        private readonly INativeWindow nativeWindow;

        public Window(IServicesProvider servicesProvider)
        {
            servicesProvider.TryResolveInstance(out nativeWindow);
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            if (elements.Count() != 1) throw new InvalidOperationException("Window must have only one child - root view.");
            RootView = (View)elements.First();
        }

        [XxSchemaIgnore]
        public Size Size
        {
            get => ((SizeF)((Vector2)RootView.Bounds.Size * UiUnit.PixelsPerUnit)).Round();

            set
            {
                RootView.Bounds = new RectangleF(Vector2.Zero, (Vector2)(SizeF)value / UiUnit.PixelsPerUnit);
            }
        }

        public void Update(float timeDelta)
        {
            RootView?.Update(timeDelta);
        }

        public void Render(Canvas canvas)
        {
            canvas.SaveState();
            canvas.Transform(Matrix3x2.CreateScale(UiUnit.PixelsPerUnit));
            RootView?.Render(canvas);
            canvas.Restore();
        }
    }
}
