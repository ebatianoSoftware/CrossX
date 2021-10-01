using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public sealed class ResourceElement: IElementsContainer
    {
        public Name Key { get; set; }
        public object Value { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            Value = elements.FirstOrDefault();
        }
    }
}