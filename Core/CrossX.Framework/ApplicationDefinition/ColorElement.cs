using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class ColorElement
    {
        public string Key { get; set; }
        public Color Value { get; set; }
    }

    [XxSchemaExport]
    public class LengthElement
    {
        public string Key { get; set; }
        public Length Value { get; set; }
    }
}
