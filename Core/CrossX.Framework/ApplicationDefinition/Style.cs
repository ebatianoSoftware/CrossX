using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Setter))]
    public sealed class Style
    {
        public string Selector { get; set; }
    }
}
