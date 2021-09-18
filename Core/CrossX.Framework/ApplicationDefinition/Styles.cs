using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Style))]
    public sealed class Styles
    {
        public string Selector { get; set; }
    }
}
