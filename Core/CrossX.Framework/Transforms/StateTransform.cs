using Xx;

namespace CrossX.Framework.Transforms
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class StateTransform
    {
        public int EnterDuration { get; set; }
        public int LeaveDuration { get; set; }
    }
}
