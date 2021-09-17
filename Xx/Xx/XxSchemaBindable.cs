using System;

namespace Xx
{
    public class XxSchemaBindable : Attribute
    {
        public XxSchemaBindable(bool bindable)
        {
            Bindable = bindable;
        }

        public bool Bindable { get; }
    }
}
