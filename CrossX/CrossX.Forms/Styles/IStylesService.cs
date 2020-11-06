using CrossX.Forms.Xml;
using System.Collections.Generic;

namespace CrossX.Forms.Styles
{
    internal interface IStylesService
    {
        void RegisterStyle(string name, IList<KeyValuePair<string, string>> values, IList<KeyValuePair<string, XNode>> content);
        void ApplyStyle(XNode node, string name);
    }
}
