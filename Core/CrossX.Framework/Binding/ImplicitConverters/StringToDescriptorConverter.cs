using CrossX.Abstractions.Mvvm;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(string), typeof(ImageDescriptor))]
    internal class StringToDescriptorConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            return new ImageDescriptor((string)value);
        }
    }
}
