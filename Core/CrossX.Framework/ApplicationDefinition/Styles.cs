using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Style), typeof(ImportElement))]
    public sealed class Styles: IElementsContainer
    {
        public void InitChildren(IEnumerable<object> elements)
        {
        }
    }
}
