using Xx;

namespace CrossX.Framework.Resources
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Setter))]
    public sealed class Style
    {
        public string Selector { get; set; }
    }
}
