using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;
using Xx.Definition;

namespace CrossX.Framework.UI.Templates
{

    [ChildrenAsDefinitions]
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class DataTemplateElement: IElementsContainer
    {
        public XxElement Element { get; private set; }
        public string DataType { get; set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            Element = (XxElement)elements.First();
        }
    }
}
