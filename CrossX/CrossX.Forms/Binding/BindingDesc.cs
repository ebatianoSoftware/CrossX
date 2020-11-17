using CrossX.Forms.Converters;
using System.Reflection;

namespace CrossX.Forms.Binding
{
    public class BindingDesc
    {
        public BindingDesc(PropertyInfo targetProperty, IValueSource source, string name, IValueConverter converter)
        {
            TargetProperty = targetProperty;
            Source = source;
            Name = name;
            Converter = converter;
        }

        public PropertyInfo TargetProperty { get; }
        public IValueSource Source { get; }
        public string Name { get; }
        public IValueConverter Converter { get; }
    }
}
