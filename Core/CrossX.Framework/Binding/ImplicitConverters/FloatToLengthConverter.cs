using CrossX.Abstractions.Mvvm;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(System.Drawing.Color), typeof(Color))]
    internal class FloatToLengthConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            if (value is float f)
            {
                return new Length(f);
            }
            return null;
        }
    }

}
