using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class FontWeightElement : IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public FontWeight Value { get; set; }

        object IValueElement.Value => Value;
    }
}
