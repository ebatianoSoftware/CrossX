using CrossX.Forms.Xml;
using System;

namespace CrossX.Forms.Helpers
{
    public static class XmlHelpers
    {
        public static Type TypeFromNode(XNode node)
        {
            var ns = node.Namespace.Replace("clr-namespace:", "").Split(',');
            var name = node.Tag;
            return Type.GetType(ns[0] + '.' + name + ',' + ns[1]);
        }
    }
}
