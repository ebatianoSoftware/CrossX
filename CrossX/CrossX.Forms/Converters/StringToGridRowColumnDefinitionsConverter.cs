using CrossX.Forms.Controls;
using System;
using System.Globalization;

namespace CrossX.Forms.Converters
{
    internal class StringToGridRowColumnDefinitionsConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
                var parts = str.Split(',');
                var result = new GridLength[parts.Length];

                for(var idx =0; idx < parts.Length; ++idx)
                {
                    result[idx] = ParseGridLength(parts[idx]);
                }

                return result;
            }
            return new GridLength[0];
        }

        private GridLength ParseGridLength(string str)
        {
            if (str == "Auto") return new GridLength(GridLengthMode.Auto, 0);

            if(str.EndsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Trim('*');
                if (string.IsNullOrWhiteSpace(str)) return new GridLength(GridLengthMode.Star, 1);
                float.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var value);
                return new GridLength(GridLengthMode.Star, value);
            }

            float.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var value2);
            return new GridLength(GridLengthMode.Value, value2);
        }
    }
}
