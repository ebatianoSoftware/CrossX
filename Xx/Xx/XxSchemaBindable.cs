using System;

namespace Xx
{
    public class XxSchemaBindable : Attribute
    {
        public XxSchemaBindable(bool bindable = true)
        {
            Bindable = bindable;
        }

        public bool Bindable { get; }
    }
}
