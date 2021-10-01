using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class LengthElement: IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public Length Value { get; set; }

        object IValueElement.Value => Value;
    }

    [XxSchemaExport]
    public class ThicknessElement : IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public Thickness Value { get; set; }

        object IValueElement.Value => Value;
    }
}
