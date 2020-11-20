using System;

namespace CrossX.Forms.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value)
        {
            return "true".Equals(value as string, StringComparison.OrdinalIgnoreCase);
        }
    }
}
