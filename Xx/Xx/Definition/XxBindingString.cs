using System;

namespace Xx.Definition
{
    public class XxBindingString
    {
        public string Value { get; }

        public XxBindingString(string value)
        {
            Value = value;
        }
    }

    public class XxBindingStringAttribute: Attribute
    {

    }
}
