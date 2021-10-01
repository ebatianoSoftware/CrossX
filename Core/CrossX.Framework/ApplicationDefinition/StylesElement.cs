using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(StyleElement), typeof(ImportElement))]
    public sealed class StylesElement: IElementsContainer
    {
        public void InitChildren(IEnumerable<object> elements)
        {
        }
    }
}
