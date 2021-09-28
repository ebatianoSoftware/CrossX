using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    public interface IValueElement
    {
        string Key { get; }
        object Value { get; }
    }

    [XxSchemaExport]
    public class ColorElement: IValueElement
    {
        public string Key { get; set; }

        [XxSchemaBindable]
        public Color Value { get; set; }

        object IValueElement.Value => Value;
    }

    [XxSchemaExport]
    public class LengthElement: IValueElement
    {
        public string Key { get; set; }

        [XxSchemaBindable]
        public Length Value { get; set; }

        object IValueElement.Value => Value;
    }
}
