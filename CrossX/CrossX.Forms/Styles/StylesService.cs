using CrossX.Forms.Xml;
using System;
using System.Collections.Generic;

namespace CrossX.Forms.Styles
{
    internal class StylesService : IStylesService
    {
        public void ApplyStyle(XNode node, string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterStyle(string name, IList<KeyValuePair<string, string>> values, IList<KeyValuePair<string, XNode>> content)
        {
            throw new NotImplementedException();
        }
    }
}
