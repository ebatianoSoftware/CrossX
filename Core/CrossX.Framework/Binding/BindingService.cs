using CrossX.Framework.Core;
using CrossX.Framework.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;

namespace CrossX.Framework.Binding
{
    internal class BindingService : IBindingService
    {
        private readonly List<BaseBinding> bindings = new List<BaseBinding>();
        private readonly IAppValues appValues;
        private readonly IConversionService conversionService;

        public BindingService(IAppValues appValues, IConversionService conversionService)
        {
            this.appValues = appValues;
            this.conversionService = conversionService;
        }

        private void AddBinding(BaseBinding binding)
        {
            lock (bindings)
            {
                for (var idx = 0; idx < bindings.Count;)
                {
                    if (bindings[idx].Target == binding.Target && bindings[idx].TargetProperty == binding.TargetProperty)
                    {
                        bindings[idx].Dispose();
                        bindings.RemoveAt(idx);
                        continue;
                    }
                    ++idx;
                }
                bindings.Add(binding);
            }
        }

        public void RemoveBindings(object target)
        {
            lock (bindings)
            {
                for (var idx = 0; idx < bindings.Count;)
                {
                    if (bindings[idx].Target == target)
                    {
                        bindings.RemoveAt(idx);
                        continue;
                    }
                    ++idx;
                }
            }
        }

        public void RemoveBinding(object target, string propertyName)
        {
            lock (bindings)
            {
                for (var idx = 0; idx < bindings.Count;)
                {
                    if (bindings[idx].Target == target && bindings[idx].TargetProperty.Name == propertyName)
                    {
                        bindings.RemoveAt(idx);
                        continue;
                    }
                    ++idx;
                }
            }
        }

        public void AddBinding(object target, PropertyInfo property, string bindingString)
        {
            var parts = bindingString.Split(' ');

            switch(parts[0])
            {
                case "Binding":
                    AddValueBinding(target, property, string.Join(" ", parts.Skip(1)));
                    break;

                case "Theme":
                    if (parts.Length > 1)
                    {
                        SetThemeValue(target, property, parts[1]);
                    }
                    break;

                case "Resource":
                    if (parts.Length > 1)
                    {
                        SetResourceValue(target, property, parts[1]);
                    }
                    break;
            }

        }

        private void SetResourceValue(object target, PropertyInfo property, string name)
        {
            var res = appValues.GetResource(name);
            if (res != null)
            {
                // TODO: Add build in conversion
                try
                {
                    property.SetValue(target, res);
                }
                catch { }
            }
        }

        private void SetThemeValue(object target, PropertyInfo property, string name)
        {
            var value = appValues.GetValue(name);
            if (value != null)
            {
                // TODO: Add build in conversion
                try
                {
                    property.SetValue(target, value);
                }
                catch { }
            }
        }

        private void AddValueBinding(object target, PropertyInfo property, string binding)
        {
            var parts = binding.Split(',').Select(o => o.Trim()).ToArray();
            var srcProperty = parts[0];

            var mode = property.GetCustomAttribute<BindingModeAttribute>()?.Mode ?? BindingMode.OneWay;
            AddBinding(new ContextBinding(target, property, nameof(UIBindingContext.DataContext), srcProperty, mode, conversionService));
        }

        public void AddDataContextBinding(object target, object source)
        {
            var targetProperty = target.GetType().GetProperty(nameof(UIBindingContext.DataContext), BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            if (targetProperty == null) return;

            AddBinding(new BaseBinding(target, targetProperty, source, nameof(UIBindingContext.DataContext), conversionService));
        }
    }
}
