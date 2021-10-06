using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Xx.Definition;
using Xx.Xml;

namespace Xx.Toolkit
{
    public class XxFileParser
    {
        private readonly IElementTypeMapping elementTypeMapping;

        public XxFileParser(IElementTypeMapping elementTypeMapping)
        {
            this.elementTypeMapping = elementTypeMapping;
        }

        public XxElement Parse(Stream stream)
        {
            var node = XNode.ReadXml(stream);
            return Parse(node);
        }

        public XxElement Parse(XNode node)
        {
            var type = elementTypeMapping.FindElement(node.Namespace, node.Tag);
            if (type == null) throw new InvalidDataException($"Unknown type {node.Namespace}:{node.Tag}");

            var propertyValues = ParsePropertyValues(node, type);

            List<XxElement> children = null;

            if (node.Nodes.Count > 0)
            {
                children = new List<XxElement>();
                foreach (var child in node.Nodes)
                {
                    children.Add(Parse(child));
                }
            }

            return new XxElement(node, type, children?.ToArray(), propertyValues);
        }

        private IReadOnlyDictionary<PropertyInfo, object> ParsePropertyValues(XNode node, Type type)
        {
            Dictionary<PropertyInfo, object> propertiesValues = new Dictionary<PropertyInfo, object>();

            foreach (var attr in node.Attributes)
            {
                var attrValue = node.Attribute(attr);
                if (attrValue == null) continue;

                var propertyInfo = type.GetProperty(attr.Replace('.', '_'), BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                if (propertyInfo == null) continue;

                object value = null;

                if (attrValue.StartsWith('{') && attrValue.EndsWith('}'))
                {
                    value = new XxBindingString(attrValue.Trim('{', '}'));
                }
                else
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        value = attrValue;
                    }
                    else if ( propertyInfo.PropertyType.IsEnum)
                    {
                        Enum.TryParse(propertyInfo.PropertyType, attrValue, out value);
                    }
                    else
                    {
                        var parseMethod = propertyInfo.PropertyType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(IFormatProvider) }, Array.Empty<ParameterModifier>());
                        if (parseMethod == null)
                        {
                            parseMethod = propertyInfo.PropertyType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, Array.Empty<ParameterModifier>());
                            if (parseMethod != null)
                            {
                                value = parseMethod.Invoke(null, new object[] { attrValue });
                            }
                        }
                        else
                        {
                            value = parseMethod.Invoke(null, new object[] { attrValue, CultureInfo.InvariantCulture });
                        }
                    }
                }

                if (value != null)
                {
                    propertiesValues.Add(propertyInfo, value);
                }
            }
            return propertiesValues;
        }
    }
}
