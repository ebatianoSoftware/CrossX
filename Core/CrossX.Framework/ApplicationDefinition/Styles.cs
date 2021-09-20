using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Style), typeof(ImportElement))]
    public sealed class Styles
    {
        public string Selector { get; set; }
    }
}
