using CrossX.Abstractions.Mvvm;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    internal class IntToFloatConverter : IImplicitValueConverter
    {
        [ImplicitValueConverter(typeof(int), typeof(float))]
        public object Convert(object value)
        {
            if(value is int i)
            {
                float ret = i;
                return ret;
            }
            return null;
        }
    }
}
