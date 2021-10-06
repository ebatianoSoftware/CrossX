using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.UI.Global
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class Page : IElementsContainer
    {
        public View RootView { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            if (elements.Count() != 1) throw new InvalidOperationException("Page must have only one child - root view.");
            RootView = (View)elements.First();
        }

        [XxSchemaIgnore]
        public RectangleF Location
        {
            get => RootView.Bounds;

            set
            {
                RootView.Bounds = value;
            }
        }
    }
}
