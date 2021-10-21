using System;
using System.Collections.Generic;
using System.Reflection;
using Xx.Xml;

namespace Xx.Definition
{
    public class XxElement
    {
        public Type Type { get; }
        public IEnumerable<XxElement> Children { get; }
        public IReadOnlyDictionary<PropertyInfo, object> Properties { get; }
        public IReadOnlyDictionary<string, string> Namespaces { get; }
        public Guid Guid { get; }

        public XxElement(XNode node, Type type, IEnumerable<XxElement> children, IReadOnlyDictionary<PropertyInfo, object> properties)
        {
            Type = type;
            Children = children;
            Properties = properties;
            Namespaces = node.Namespaces;
            Guid = Guid.NewGuid();
        }
    }
}
