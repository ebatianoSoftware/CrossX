using CrossX.Core;
using CrossX.Forms.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.Forms.Services
{
    internal class XmlFlagsService : IXmlFlagsService
    {
        private readonly IFeaturesFlags featuresFlags;
        private List<string> attrToRemove = new List<string>();
        private List<Tuple<string, string>> attrToAdd = new List<Tuple<string, string>>();

        public XmlFlagsService(IFeaturesFlags featuresFlags)
        {
            this.featuresFlags = featuresFlags;
        }

        public void Apply(XNode node)
        {
            foreach(var attr in node.Attributes)
            {
                if(attr.StartsWith("flags:"))
                {
                    var value = node.Attribute(attr);

                    var parts = attr.Split(':');
                    var test = parts[1].Replace(" ", "").Split(',');
                    var name = parts[2].Trim();

                    attrToRemove.Add(attr);

                    if (featuresFlags.Flags.Intersect(test).Count() == test.Length)
                    {
                        attrToAdd.Add(Tuple.Create(name, value));
                    }
                }
            }

            foreach(var attr in attrToRemove)
            {
                node.RemoveAttribute(attr);
            }
            attrToRemove.Clear();

            foreach (var attr in attrToAdd)
            {
                node.AddAttribute(attr.Item1, attr.Item2);
            }
            attrToAdd.Clear();

            foreach (var cn in node.Nodes)
            {
                Apply(cn);
            }
        }
    }
}
