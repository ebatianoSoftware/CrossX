using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(ColorElement), typeof(LengthElement), typeof(ImportElement))]
    public class ThemeElement : IElementsContainer
    {
        public IReadOnlyDictionary<string, object> Values { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            var values = new Dictionary<string, object>();

            foreach(var element in elements)
            {
                switch(element)
                {
                    case ColorElement ce:
                        values.Add(ce.Key, ce.Value);
                        break;

                    case LengthElement le:
                        values.Add(le.Key, le.Value);
                        break;
                }
            }

            Values = values;
        }
    }
}
