// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Xx.Xml
{
    public class XNode
    {
        public readonly string Tag;
        public string Value;
        public readonly string Namespace;
        public readonly int LineNumber;

        public List<XNode> Nodes { get; private set; }

        readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

        readonly XNode parent;

        internal XNode(string tag)
        {
            parent = null;
            Tag = tag;
            Nodes = new List<XNode>();
        }

        public void RemoveAttribute(string key)
        {
            attributes.Remove(key);
        }

        public XNode(XNode parent, string tag)
        {
            this.parent = parent;
            Tag = tag;
            Nodes = new List<XNode>();
        }

        public XNode(XNode parent, string tag, string @namespace)
        {
            this.parent = parent;
            Namespace = @namespace;
            Tag = tag;
            Nodes = new List<XNode>();
        }

        public XNode AddChildNode(string tag, string @namespace)
        {
            var node = new XNode(this, tag, @namespace);
            Nodes.Add(node);
            return node;
        }

        private XNode(XNode parent, int lineNumber)
        {
            this.parent = parent;
            LineNumber = lineNumber;
            Nodes = new List<XNode>();
        }

        private XNode(XmlReader reader, XNode parent, Dictionary<string, string> namespaces)
        {
            namespaces = new Dictionary<string, string>(namespaces);

            this.parent = parent;

            while (reader.NodeType != XmlNodeType.Element)
            {
                if (reader.EOF || reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.Read();
                    }

                    Tag = null;
                    return;
                }
                reader.Read();
            }

            Tag = reader.LocalName;
            Namespace = reader.NamespaceURI;

            if (reader is IXmlLineInfo xmlInfo)
            {
                LineNumber = xmlInfo.LineNumber;
            }
            else
            {
                LineNumber = 0;
            }

            var hasChildren = !reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                // Read all attributes and add them to dictionary.
                while (reader.MoveToNextAttribute())
                {
                    string key = reader.Name;
                    if (key.StartsWith("xmlns:", StringComparison.InvariantCulture))
                    {
                        key = key.Substring(6);
                        namespaces.Add(key, reader.Value);
                    }
                    else if (key != "xmlns")
                    {
                        if (key.Contains(":"))
                        {
                            string[] vals = key.Split(':');
                            key = string.Format("{0}:{1}", namespaces[vals[0]], vals[1]);
                        }

                        attributes.Add(key, reader.Value);
                    }
                }
            }
            else
            {
                reader.Read();
            }

            Nodes = new List<XNode>();

            while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.Text)
            {
                if (reader.EOF || reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
                reader.Read();
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
                Value = reader.Value;
            }

            if (hasChildren)
            {
                while (true)
                {
                    XNode node = new XNode(reader, this, namespaces);

                    if (node.Tag != null)
                    {
                        Nodes.Add(node);
                    }
                    else
                    {
                        break;
                    }

                    if (reader.EOF)
                    {
                        return;
                    }
                }
            }
        }

        public void AddAttribute(string name, string value)
        {
            if (value == null) return;
            attributes.Add(name, value);
        }

        public void WriteAttribute(string name, string value)
        {
            if (value == null) return;
            attributes[name] = value;
        }

        public XNode EnucleateAttributes(string attribPrefix)
        {
            var node = new XNode(parent, LineNumber);
            int prefixLength = attribPrefix.Length;

            foreach (var attrib in attributes)
            {
                if (attrib.Key.StartsWith(attribPrefix, StringComparison.InvariantCulture))
                {
                    node.attributes.Add(attrib.Key.Substring(prefixLength), attrib.Value);
                }
            }

            return node;
        }

        public static XNode ReadXml(XmlReader reader)
        {
            return new XNode(reader, null, new Dictionary<string, string>());
        }

        public static XNode ReadXml(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return ReadXml(stream);
            }
        }

        public static XNode ReadXml(Stream stream)
        {
            var reader = XmlReader.Create(stream);
            return ReadXml(reader);
        }

        public XNode this[string nodeName]
        {
            get
            {
                for (int idx = 0; idx < Nodes.Count; ++idx)
                {
                    if (Nodes[idx].Tag == nodeName)
                    {
                        return Nodes[idx];
                    }
                }

                throw new Exception("Cannot find xml node: " + nodeName);
            }
        }

        public string Attribute(string name)
        {
            return attributes.TryGetValue(name, out var value) ? value : null;
        }

        public bool HasAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        public string NodeInfo => $"{LineNumber}";

        public string NodeError(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return string.Format("{0}: error: {1}", NodeInfo, message);
        }

        public IEnumerable<string> Attributes
        {
            get
            {
                return attributes.Keys;
            }
        }

        public void Write(Stream stream)
        {
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;

            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                Write(xmlWriter);
            }
        }
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement(Tag, Namespace);

            foreach (var attr in attributes)
            {
                writer.WriteAttributeString(attr.Key, attr.Value);
            }

            if (!string.IsNullOrWhiteSpace(Value))
            {
                writer.WriteString(Value);
            }

            foreach (var node in Nodes)
            {
                node.Write(writer);
            }

            writer.WriteEndElement();
        }
    }
}
