using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(ColorElement), typeof(LengthElement), typeof(ImportElement))]
    public class ThemeElement : IElementsContainer
    {
        private readonly IAppValues appValues;

        public ThemeElement(IAppValues appValues)
        {
            this.appValues = appValues;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach(var el in elements.Where( o=> o is IValueElement ).Cast<IValueElement>())
            {
                appValues.RegisterValue(el.Key, el.Value);
            }
        }
    }
}
