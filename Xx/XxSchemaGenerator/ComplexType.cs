using System;
using Xx;

namespace xxsgen
{
    internal class ComplexType
    {
        public Type Type;
        public string Namespace;
        public bool Exportable;
        public string Name;
        public Attribute[] Attributes;
        public XxChildrenMode ChildrenMode;
        public ComplexType[] ChildrenTypes;
        public ComplexType BaseType;

        internal bool IsDerrivedFrom(ComplexType type)
        {
            if (this == type) return true;
            if (BaseType == type) return true;
            if (BaseType != null) return BaseType.IsDerrivedFrom(type);
            return false;
        }
    }
}
