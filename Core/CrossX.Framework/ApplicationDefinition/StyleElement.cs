using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;
using Xx.Definition;

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
                key.Name = value;
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

                if (key.Name == null) key.Name = "";
                appValues.RegisterStyle(key, element);
            }
        }
    }
}
