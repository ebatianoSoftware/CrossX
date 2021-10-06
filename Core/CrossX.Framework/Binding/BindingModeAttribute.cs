using System;

namespace CrossX.Framework.Binding
{
    public class BindingModeAttribute: Attribute 
    { 
        public BindingModeAttribute(BindingMode mode)
        {
            Mode = mode;
        }

        public BindingMode Mode { get; }
    }
}
