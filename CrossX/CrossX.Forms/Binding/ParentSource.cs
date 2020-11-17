using CrossX.Forms.Controls;
using System;

namespace CrossX.Forms.Binding
{
    internal class ParentSource : IValueSource
    {
        private readonly Control control;
        private readonly Type parentType;

        public ParentSource(Control control, Type parentType)
        {
            this.control = control;
            this.parentType = parentType;
        }

        public ParentSource(Control control)
        {
            this.control = control;
            parentType = null;
        }

        public object Resolve()
        {
            var parent = control.Parent;
            if (parentType == null) return parent;

            while( !parentType.IsAssignableFrom(parent.GetType()) )
            {
                if(parent is Control control)
                {
                    parent = control.Parent;
                }
                else
                {
                    return null;
                }
            }
            return parent;
        }
    }
}
