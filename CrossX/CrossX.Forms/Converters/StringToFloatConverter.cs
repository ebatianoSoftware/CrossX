using System.Globalization;

namespace CrossX.Forms.Converters
{
    internal class StringToFloatConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
                float.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                return result;
            }
            return 0.0f;
        }
    }
}
