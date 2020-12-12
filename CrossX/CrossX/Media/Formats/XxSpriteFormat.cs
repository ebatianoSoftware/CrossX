using CrossX.Data.Sprites;
using CrossX.Xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CrossX.Media.Formats
{
    public class XxSpriteFormat
    {
        public SpriteDefinition FromStream(Stream stream, params Type[] knownTypes)
        {
            var node = XNode.ReadXml(stream);
            if (node.Tag != "Sprite") throw new InvalidDataException();

            var list = new List<SpriteSequence>();

            foreach(var cn in node.Nodes)
            {
                if (cn.Tag != "Sequence") throw new InvalidDataException();
                list.Add(ReadSequence(cn, knownTypes));
            }

            return new SpriteDefinition(list.ToArray());
        }

        private SpriteSequence ReadSequence(XNode node, Type[] knownTypes)
        {
            var name = node.Attribute("Name");
            var spriteSheet = node.Attribute("Sheet");

            var list = new List<SpriteFrame>();
            foreach (var cn in node.Nodes)
            {
                if (cn.Tag != "Frame") throw new InvalidDataException();
                list.Add(ReadFrame(cn, knownTypes));
            }

            return new SpriteSequence(name, spriteSheet, list.ToArray());
        }

        private SpriteFrame ReadFrame(XNode node, Type[] knownTypes)
        {
            var attr = new XNodeAttributes(node);
            if (!attr.Parse4Int("Source", out var x, out var y, out var w, out var h)) throw new InvalidDataException();
            if (!attr.Parse2Int("Origin", out var ox, out var oy)) throw new InvalidDataException();
            var frameTime = (float)attr.AsInt32("Time", 100) / 1000f;

            var list = new List<SpriteEvent>();

            foreach(var cn in node.Nodes)
            {
                if (cn.Tag != "Event") throw new InvalidDataException();
                list.Add(ParseEvent(cn, knownTypes));
            }

            return new SpriteFrame(new Rectangle(x, y, w, h), new Point(ox, oy), frameTime, list?.ToArray());
        }

        private SpriteEvent ParseEvent(XNode node, Type[] knownTypes)
        {
            var id = node.Attribute("Id");

            Dictionary<string, object> parameters = null;
            var parametersNode = node.Nodes.FirstOrDefault(o => o.Tag == "Parameters");

            if(parametersNode != null)
            {    
                parameters = new Dictionary<string, object>();

                foreach(var cn in parametersNode.Nodes)
                {
                    string parameterId = cn.Tag;
                    object parameterValue = ParseParameter(cn, knownTypes);

                    if(parameterValue != null)
                    {
                        parameters.Add(parameterId, parameterValue);
                    }
                }
            }
            return new SpriteEvent(id, parameters);
        }

        private object ParseParameter(XNode node, Type[] knownTypes)
        {
            var attr = new XNodeAttributes(node);
            var type = node.Attribute("Type");

            switch (type)
            {
                case "String":
                    return attr.AsString("Value");
                case "Integer":
                    return attr.AsInt32("Value", 0);
                case "Float":
                    return (float)attr.AsDouble("Value", 0.0);
                case "Double":
                    return attr.AsDouble("Value", 0.0);
            }

            if(type.StartsWith("Enum(") && knownTypes != null)
            {
                type = type.Substring(5).TrimEnd(')');
                var enumType = knownTypes.FirstOrDefault(o => o.Name.ToLowerInvariant() == type.ToLowerInvariant());

                try
                {
                    return Enum.Parse(enumType, attr.AsString("Value"));
                }
                catch { }
            }

            return null;
        }
    }
}
