using CrossX.Abstractions.Mvvm;
using CrossX.Framework.Graphics;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(Image), typeof(ImageDescriptor))]
    internal class ImageToDescriptorConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            return new ImageDescriptor((Image)value);
        }
    }
}
