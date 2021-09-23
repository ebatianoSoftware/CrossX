using System.Reflection;

namespace CrossX.Framework.Binding
{

    public interface IBindingService
    {
        void AddBinding(object target, PropertyInfo property, string bindingString);
        void AddDataContextBinding(object target, object source);
        void RemoveBindings(object target);
        void RemoveBinding(object target, string propertyName);
    }
}
