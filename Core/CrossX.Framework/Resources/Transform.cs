using Xx;

namespace CrossX.Framework.Resources
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public sealed class Transform
    {
        public Name Key { get; set; }

        public int EnterDuration { get; set; }
        public int LeaveDuration { get; set; }
    }
}