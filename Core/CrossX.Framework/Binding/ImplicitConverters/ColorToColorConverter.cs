using CrossX.Abstractions.Mvvm;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(System.Drawing.Color), typeof(Color))]
    internal class ColorToColorConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            if (value is System.Drawing.Color c)
            {
                return new Color(c.R, c.G, c.B, c.A);
            }
            return null;
        }
    }
}
