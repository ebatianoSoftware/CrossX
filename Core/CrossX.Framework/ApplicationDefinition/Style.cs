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
    public sealed class Style: IElementsContainer
    {
        public string Selector
        { 
            set
            {
                var key = Key;

                var filters = value.Split(':');
                var name = filters[0];

                if (filters.Length == 2)
                {
                    key.State = filters[1];
                }
                key.Name = name;
                Key = key;
            }
        }

        public SelectorKey Key { get; private set; }
        public XxElement Element { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            var element = elements.FirstOrDefault() as XxElement;

            if (element != null)
            {
                var key = Key;
                key.Type = element.Type;
                Element = element;
                Key = key;
            }
        }
    }
}
