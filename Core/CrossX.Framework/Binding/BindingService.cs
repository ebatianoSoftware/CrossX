using System.Collections.Generic;
using System.Reflection;

namespace CrossX.Framework.Binding
{
    internal class BindingService : IBindingService
    {
        private readonly List<BaseBinding> bindings = new List<BaseBinding>();
        public void AddBinding(BaseBinding binding)
        {
            for (var idx = 0; idx < bindings.Count;)
            {
                if(bindings[idx].Target == binding.Target && bindings[idx].TargetProperty == binding.TargetProperty)
                {
                    bindings[idx].Dispose();
                    bindings.RemoveAt(idx);
                    continue;
                }
                ++idx;
            }
            bindings.Add(binding);
        }

        public void RemoveBindings(object target)
        {
            for(var idx =0; idx < bindings.Count;)
            {
                if(bindings[idx].Target == target)
                {
                    bindings.RemoveAt(idx);
                    continue;
                }
                ++idx;
            }
        }

        public void AddBinding(object target, PropertyInfo property, string bindingString)
        {

        }

        public void AddDataContextBinding(object target, object source, string dataContextPropertyName)
        {
            var targetProperty = target.GetType().GetProperty(dataContextPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            if (targetProperty == null) return;

            AddBinding(new BaseBinding(target, targetProperty, source, dataContextPropertyName));
        }
    }
}
