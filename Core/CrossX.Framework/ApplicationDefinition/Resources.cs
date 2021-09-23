using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Resource), typeof(ImportElement))]
    public sealed class Resources : IElementsContainer
    {
        private IAppValues appValues;

        public Resources(IAppValues appValues)
        {
            this.appValues = appValues;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach(var el in elements.Cast<Resource>())
            {
                appValues.RegisterResource(el.Key, el.Value);
            }
        }
    }
}
