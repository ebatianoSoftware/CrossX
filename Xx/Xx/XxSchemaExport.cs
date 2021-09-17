using System;

namespace Xx
{
    public enum XxChildrenMode
    {
        Zero,
        MaxOne,
        OnlyOne,
        Multiple
    }

    public class XxSchemaExport : Attribute
    {
        public XxSchemaExport() { }

        public XxSchemaExport(XxChildrenMode childrenMode = XxChildrenMode.Zero, params Type[] childTypes)
        {
            ChildTypes = childTypes;
            ChildrenMode = childrenMode;
        }

        public Type[] ChildTypes { get; }
        public XxChildrenMode ChildrenMode { get; }
    }
}
