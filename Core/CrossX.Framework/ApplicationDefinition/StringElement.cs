using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class StringElement : IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public string Value { get; set; }

        object IValueElement.Value => Value;
    }
}
