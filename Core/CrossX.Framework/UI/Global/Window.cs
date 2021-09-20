using CrossX.Abstractions.IoC;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xx;
using Xx.Toolkit;

namespace CrossX.Framework.UI.Global
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class Window : IElementsContainer
    {
        public View RootView { get; private set; }

        public Length Desktop_MinWidth 
        { 
            set
            {
                if (nativeWindow == null) return;
                minSize.Width = (int)value.Calculate(1);
                nativeWindow.MinSize = minSize;
            }
        }

        public Length Desktop_MinHeight
        {
            set
            {
                if (nativeWindow == null) return;
                minSize.Height = (int)value.Calculate(1);
                nativeWindow.MinSize = minSize;
            }
        }

        public Length Desktop_MaxWidth
        {
            set
            {
                if (nativeWindow == null) return;
                maxSize.Width = (int)value.Calculate(1);
                nativeWindow.MaxSize = maxSize;
            }
        }

        public Length Desktop_MaxHeight
        {
            set
            {
                if (nativeWindow == null) return;
                maxSize.Height = (int)value.Calculate(1);
                nativeWindow.MaxSize = maxSize;
            }
        }

        public Length Desktop_InitialWidth
        {
            set
            {
                if (nativeWindow == null) return;
                size.Width = (int)value.Calculate(1);
                nativeWindow.Size = size;
            }
        }

        public Length Desktop_InitialHeight
        {
            set
            {
                if (nativeWindow == null) return;
                size.Height = (int)value.Calculate(1);
                nativeWindow.Size = size;
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
            get => RootView.Bounds.Size.Round();

            set
            {
                RootView.Bounds = new RectangleF(Vector2.Zero, value);
            }
        }

        public void Update(float timeDelta)
        {
            RootView?.Update(timeDelta);
        }

        public void Render(Canvas canvas)
        {
            RootView?.Render(canvas);
        }
    }
}
