using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xx.Definition
{
    public class XxElement
    {
        public Type Type { get; }
        public IEnumerable<XxElement> Children { get; }
        public IReadOnlyDictionary<PropertyInfo, object> Properties { get; }

        public XxElement(Type type, IEnumerable<XxElement> children, IReadOnlyDictionary<PropertyInfo, object> properties)
        {
            Type = type;
            Children = children;
            Properties = properties;
        }
    }
}
