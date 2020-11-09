using CrossX.Forms.Xml;
using CrossX.IO;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CrossX.Forms.Styles
{
    internal class StylesService : IStylesServiceEx
    {
        class Style
        {
            public IList<KeyValuePair<string, string>> Values;
            public IList<XNode> Content;
        }

        private readonly Dictionary<string, Style> styles = new Dictionary<string, Style>();
        private readonly IFilesRepository filesRepository;

        public StylesService(IFilesRepository filesRepository)
        {
            this.filesRepository = filesRepository;
        }

        public void ApplyStyle(XNode node)
        {
            if (node.HasAttribute("Class"))
            {
                var names = node.Attribute("Class").Split(',', ' ');
                node.RemoveAttribute("Class");

                foreach (var name in names)
                {
                    if (styles.TryGetValue('.' + name, out var style))
                    {
                        ApplyStyle(node, style);
                    }
                }
            }
            
            var ns = node.Namespace.Replace("clr-namespace:", "").Split(',');
            var tname = node.Tag;
            var typeName = ns[0] + '.' + tname;

            if(styles.TryGetValue(typeName, out var style2))
            {
                ApplyStyle(node, style2);
            }
        }

        private void ApplyStyle(XNode node, Style style)
        {
            foreach(var attr in style.Values)
            {
                if(!node.HasAttribute(attr.Key))
                {
                    node.AddAttribute(attr.Key, attr.Value);
                }
            }

            foreach(var cn in style.Content)
            {
                if( null == node.Nodes.Find( o=>o.Tag == cn.Tag && o.Namespace == cn.Namespace))
                {
                    var newNode = node.AddChildNode(cn.Tag, cn.Namespace);
                    CopyNode(newNode, cn);
                }
            }
        }

        private void CopyNode(XNode newNode, XNode node)
        {
            foreach(var attr in node.Attributes)
            {
                newNode.AddAttribute(attr, node.Attribute(attr));
            }

            foreach(var cn in node.Nodes)
            {
                var nn = newNode.AddChildNode(cn.Tag, cn.Namespace);
                CopyNode(nn, cn);
            }
        }

        public void LoadStyles(string path)
        {
            XNode node;
            using (var stream = filesRepository.Open(path))
            {
                var reader = XmlReader.Create(stream);
                node = XNode.ReadXml(reader);
            }

            if (node.Tag != "Styles") throw new InvalidDataException();

            foreach(var cn in node.Nodes)
            {
                if(cn.Tag == "Style")
                {
                    ParseStyle(cn);
                }
            }
        }

        private void ParseStyle(XNode node)
        {
            var key = node.Attribute("Key");
            if (string.IsNullOrEmpty(key)) throw new InvalidDataException();

            var values = new List<KeyValuePair<string, string>>();
            IList<XNode> content = null;

            foreach (var cn in node.Nodes)
            {
                switch(cn.Tag)
                {
                    case "Parameter":
                        values.Add(new KeyValuePair<string, string>(cn.Attribute("Name"), cn.Attribute("Value")));
                        break;

                    case "ContentElements":
                        content = cn.Nodes;
                        break;
                }
            }

            RegisterStyle(key, values, content);
        }

        private void RegisterStyle(string name, IList<KeyValuePair<string, string>> values, IList<XNode> content)
        {
            styles.Add(name, new Style
            {
                Values = values,
                Content = content
            });
        }
    }
}
