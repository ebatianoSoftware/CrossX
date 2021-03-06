using CrossX.Forms.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CrossX.Forms.Binding
{
    public class BindingService: IDisposable
    {
        private readonly object target;
        private readonly IConverters defaultConverters;
        private readonly List<BindingDesc> bindingDescs = new List<BindingDesc>();
        private object[] sources;
        private PropertyInfo[] sourceProperties;

        public BindingService(object target, IConverters defaultConverters)
        {
            this.target = target;
            this.defaultConverters = defaultConverters;
        }

        public void AddBinding(BindingDesc binding)
        {
            bindingDescs.Insert(0, binding);
        }

        public void Dispose()
        {
            DetachSources();
        }

        public bool Contains(string propertyName)
        {
            return bindingDescs.FirstOrDefault(o => o.TargetProperty.Name == propertyName) != null;
        }

        public bool TrySetValue(string propertyName, object value)
        {
            // TODO: Add converter support
            var desc = bindingDescs.FirstOrDefault(o => o.TargetProperty.Name == propertyName);
            if (desc == null) return false;

            if (!desc.TargetProperty.CanWrite) return false;

            var index = bindingDescs.IndexOf(desc);
            
            var prop = sourceProperties[index];
            if (prop == null || !prop.CanWrite) return false;

            if (prop.PropertyType != value.GetType()) return false;

            prop.SetValue(sources[index], value);
            return true;
        }

        private void DetachSources()
        {
            for (var idx = 0; idx < sources.Length; ++idx)
            {
                if (sources[idx] is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged -= Npc_PropertyChanged;
                }
                sources[idx] = null;
            }
        }

        public void RecreateValues()
        {
            if(sources == null)
            {
                sources = new object[bindingDescs.Count];
                sourceProperties = new PropertyInfo[bindingDescs.Count];
            }

            DetachSources();
            
            for(var idx =0; idx < bindingDescs.Count; ++idx)
            {
                var binding = bindingDescs[idx];
                sources[idx] = binding.Source.Resolve();

                if (sources[idx] is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += Npc_PropertyChanged;
                }

                if (string.IsNullOrEmpty(binding.Name))
                {
                    var value = sources[idx];
                    if (value != null)
                    {
                        if (ConvertValue(ref value, value.GetType(), binding.TargetProperty.PropertyType, binding.Converter))
                        {
                            binding.TargetProperty.SetValue(target, value);
                        }
                    }
                }
                else
                {
                    var propInfo = sources[idx]?.GetType().GetProperty(binding.Name);
                    sourceProperties[idx] = propInfo;

                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(sources[idx]);
                        if (ConvertValue(ref value, propInfo.PropertyType, binding.TargetProperty.PropertyType, binding.Converter))
                        {
                            binding.TargetProperty.SetValue(target, value);   
                        }
                    }
                }
            }
        }

        private void Npc_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            for (var idx = 0; idx < sources.Length; ++idx)
            {
                if(sources[idx] == sender)
                {
                    var binding = bindingDescs[idx];

                    if (binding.Name == args.PropertyName)
                    {
                        var propInfo = sourceProperties[idx];

                        if (propInfo != null)
                        {
                            var value = propInfo.GetValue(sender);
                            if (ConvertValue(ref value, propInfo.PropertyType, binding.TargetProperty.PropertyType, binding.Converter))
                            {
                                binding.TargetProperty.SetValue(target, value);
                            }
                        }
                    }
                }
            }
        }

        private bool ConvertValue(ref object value, Type from, Type to, IValueConverter bindedConverter)
        {
            if(bindedConverter != null)
            {
                value = bindedConverter.Convert(value);
                return true;
            }

            if (from != to && !to.IsAssignableFrom(from))
            {
                var converter = defaultConverters.FindConverter(from, to);
                if (converter == null) return false;

                value = converter.Convert(value);
            }
            return true;
        }
    }
}
