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
        private struct GlobalData
        {
            public Type[] KnownTypes;
            public Point Origin;
            public string Sheet;
            public Size FrameSize;
            public int FrameTime;
            public int ColumnCount;
        }

        public SpriteDefinition FromStream(Stream stream, params Type[] knownTypes)
        {
            var node = XNode.ReadXml(stream);
            if (node.Tag != "Sprite") throw new InvalidDataException();

            var attr = new XNodeAttributes(node);

            var globalData = new GlobalData
            {
                KnownTypes = knownTypes,
                ColumnCount = attr.AsInt32("ColumnCount", 1000),
                FrameTime = attr.AsInt32("FrameTime", 100)
            };

            if(node.HasAttribute("FrameSize"))
            {
                if (!attr.Parse2Int("FrameSize", out var w, out var h)) throw new InvalidDataException();
                globalData.FrameSize = new Size(w, h);
            }

            if (node.HasAttribute("Sheet"))
            {
                globalData.Sheet = node.Attribute("Sheet");
            }

            if(node.HasAttribute("Origin"))
            {
                if (!attr.Parse2Int("Origin", out var ox, out var oy)) throw new InvalidDataException();
                globalData.Origin = new Point(ox, oy);
            }

            var list = new List<SpriteSequence>();

            foreach(var cn in node.Nodes)
            {
                if (cn.Tag != "Sequence") throw new InvalidDataException();
                list.Add(ReadSequence(cn, globalData));
            }

            return new SpriteDefinition(list.ToArray());
        }

        private SpriteSequence ReadSequence(XNode node, GlobalData globalData)
        {
            var name = node.Attribute("Name");
            var spriteSheet = node.HasAttribute("Sheet") ? node.Attribute("Sheet") : globalData.Sheet;
            var attr = new XNodeAttributes(node);

            if (node.HasAttribute("Origin"))
            {
                
                if (!attr.Parse2Int("Origin", out var ox, out var oy)) throw new InvalidDataException();
                globalData.Origin = new Point(ox, oy);
            }

            if(node.HasAttribute("FrameTime"))
            {
                globalData.FrameTime = attr.AsInt32("FrameTime", 100);
            }

            var list = new List<SpriteFrame>();

            if(node.HasAttribute("Frames"))
            {
                foreach(var el in node.Attribute("Frames").Split(','))
                {
                    if(int.TryParse(el.Trim(), out var index))
                    {
                        list.Add(FromIndex(index, globalData));
                    }
                }

                return new SpriteSequence(name, spriteSheet, list.ToArray());
            }

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag != "Frame") throw new InvalidDataException();
                list.Add(ReadFrame(cn, globalData));
            }

            return new SpriteSequence(name, spriteSheet, list.ToArray());
        }

        private SpriteFrame FromIndex(int index, GlobalData globalData)
        {
            var x = (index % globalData.ColumnCount) * globalData.FrameSize.Width;
            var y = (index / globalData.ColumnCount) * globalData.FrameSize.Height;

            var w = globalData.FrameSize.Width;
            var h = globalData.FrameSize.Height;

            return new SpriteFrame(new Rectangle(x, y, w, h), globalData.Origin, globalData.FrameTime / 1000f, null);
        }

        private SpriteFrame ReadFrame(XNode node, GlobalData globalData)
        {
            var attr = new XNodeAttributes(node);
            if (!attr.Parse4Int("Source", out var x, out var y, out var w, out var h))
            {
                var index = attr.AsInt32("Index", -1);
                if(index == -1) throw new InvalidDataException();

                x = (index % globalData.ColumnCount) * globalData.FrameSize.Width;
                y = (index / globalData.ColumnCount) * globalData.FrameSize.Height;

                w = globalData.FrameSize.Width;
                h = globalData.FrameSize.Height;
            }

            int ox, oy;
            if (node.HasAttribute("Origin"))
            {
                if (!attr.Parse2Int("Origin", out ox, out oy)) throw new InvalidDataException();
            }
            else
            {
                ox = globalData.Origin.X;
                oy = globalData.Origin.Y;
            }

            var frameTime = (float)attr.AsInt32("Time", globalData.FrameTime) / 1000f;
            var list = new List<SpriteEvent>();

            foreach(var cn in node.Nodes)
            {
                if (cn.Tag != "Event") throw new InvalidDataException();
                list.Add(ParseEvent(cn, globalData));
            }

            return new SpriteFrame(new Rectangle(x, y, w, h), new Point(ox, oy), frameTime, list?.ToArray());
        }

        private SpriteEvent ParseEvent(XNode node, GlobalData globalData)
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
                    object parameterValue = ParseParameter(cn, globalData);

                    if(parameterValue != null)
                    {
                        parameters.Add(parameterId, parameterValue);
                    }
                }
            }
            return new SpriteEvent(id, parameters);
        }

        private object ParseParameter(XNode node, GlobalData globalData)
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

            if(type.StartsWith("Enum(") && globalData.KnownTypes != null)
            {
                type = type.Substring(5).TrimEnd(')');
                var enumType = globalData.KnownTypes.FirstOrDefault(o => o.Name.ToLowerInvariant() == type.ToLowerInvariant());

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
