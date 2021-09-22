using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Style), typeof(ImportElement))]
    public sealed class Styles: IElementsContainer
    {
        public Style[] Values { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            Values = elements.Where(o => o is Style).Cast<Style>().ToArray();
        }
    }
}
