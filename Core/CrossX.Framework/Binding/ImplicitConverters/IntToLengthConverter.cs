using CrossX.Abstractions.Mvvm;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(int), typeof(Length))]
    internal class IntToLengthConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            if (value is int i)
            {
                return new Length(i);
            }
            return null;
        }
    }
}
