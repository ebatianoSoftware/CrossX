using System;

namespace Xx
{
    public class XxSchemaExport : Attribute
    {
        public XxSchemaExport() { }

        public XxSchemaExport(params Type[] allowChildrenOfType)
        {
            ChildrenTypes = allowChildrenOfType;
        }

        public Type[] ChildrenTypes { get; }
    }
}
