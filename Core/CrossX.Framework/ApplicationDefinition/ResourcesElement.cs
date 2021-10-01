using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(ResourceElement))]
    public sealed class ResourcesElement : IElementsContainer
    {
        private IAppValues appValues;

        public ResourcesElement(IAppValues appValues)
        {
            this.appValues = appValues;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach(var el in elements.Cast<ResourceElement>())
            {
                appValues.RegisterResource(el.Key, el.Value);
            }
        }
    }
}
