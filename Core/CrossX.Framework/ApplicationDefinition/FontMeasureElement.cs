using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class FontMeasureElement : IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public FontMeasure Value { get; set; }

        object IValueElement.Value => Value;
    }
}
