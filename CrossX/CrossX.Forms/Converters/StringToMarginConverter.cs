using CrossX.Forms.Values;
using System.Globalization;

namespace CrossX.Forms.Converters
{
    internal class StringToMarginConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
                var parts = str.Split(',');

                if (parts.Length == 1)
                {
                    float.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p1);
                    return new Margin(p1, p1, p1, p1);
                }

                if (parts.Length == 2)
                {
                    float.TryParse(parts[0], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p1);
                    float.TryParse(parts[1], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p2);
                    return new Margin(p1, p2, p1, p2);
                }

                if (parts.Length == 3)
                {
                    float.TryParse(parts[0], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p1);
                    float.TryParse(parts[1], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p2);
                    float.TryParse(parts[2], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p3);
                    return new Margin(p1, p2, p3, p2);
                }

                if (parts.Length > 3)
                {
                    float.TryParse(parts[0], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p1);
                    float.TryParse(parts[1], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p2);
                    float.TryParse(parts[2], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p3);
                    float.TryParse(parts[3], NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var p4);
                    return new Margin(p1, p2, p3, p4);
                }
            }
            return Margin.Zero;
        }
    }
}
