using CrossX.Abstractions.Mvvm;
using System.Collections;

namespace CrossX.Framework.Binding.ImplicitConverters
{
    [ImplicitValueConverter(typeof(IList), typeof(bool))]
    internal class ListToBoolConverter : IImplicitValueConverter
    {
        public object Convert(object value)
        {
            if (value is IList list)
            {
                return list.Count > 0;
            }
            return null;
        }
    }
}
