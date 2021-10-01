using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xx;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    [ChildrenAsDefinitions]
    public sealed class StyleElement: IElementsContainer
    {
        public string Selector
        { 
            set
            {
                var key = keyValue;

                var filters = value.Split(':');
                var name = filters[0];

                if (filters.Length == 2)
                {
                    key.State = filters[1];
                }
                key.Name = name;
                keyValue = key;
            }
        }

        private SelectorKey keyValue;
        private readonly IAppValues appValues;

        public StyleElement(IAppValues appValues)
        {
            this.appValues = appValues;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            var element = elements.FirstOrDefault() as XxElement;

            if (element != null)
            {
                var key = keyValue;
                key.Type = element.Type;

                appValues.RegisterStyle(key, element);
            }
        }
    }
}
