using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    public interface IValueElement
    {
        ThemeValueKey Key { get; }
        object Value { get; }
    }

    [XxSchemaExport]
    public class ColorElement: IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public Color Value { get; set; }

        [XxSchemaBindable]
        public Color MixBase { get; set; } = Color.Transparent;

        public float Opacity { get; set; } = 1;

        public float Mix { get; set; } = 1;

        object IValueElement.Value => MixBase.Mix(Value, Mix) * Opacity;
    }
}
