using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Resource), typeof(ImportElement))]
    public sealed class Resources : IElementsContainer
    {
        public Resource[] Values { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            Values = elements.Where(o => o is Resource).Cast<Resource>().ToArray();
        }
    }
}
