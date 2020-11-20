using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CrossX.Forms.Converters
{

    internal class StringToColorConverter : IValueConverter
    {
        public static readonly StringToColorConverter Instance = new StringToColorConverter();

        private static readonly Dictionary<string, Color4> PredefinedColors = new Dictionary<string, Color4>();

        private StringToColorConverter() { }

        static StringToColorConverter()
        {
            var fields = typeof(Color4).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Color4))
                {
                    var color = (Color4)field.GetValue(null);
                    PredefinedColors.Add(field.Name.ToLowerInvariant(), color);
                }
            }
        }

        public object Convert(object value)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text)) return null;

                if (!text.StartsWith("#", StringComparison.InvariantCulture))
                {
                    if (PredefinedColors.TryGetValue(text.ToLowerInvariant(), out var color))
                    {
                        return color;
                    }
                }

                var colorText = text.TrimStart('#');
                if (!uint.TryParse(colorText, NumberStyles.HexNumber, null, out var uintColor)) return null;

                var alpha = colorText.Length > 6 ? (int)(uintColor >> 24) & 0xff : 255;

                return Color4.FromNonPremultiplied((int)(uintColor >> 16) & 0xff, (int)(uintColor >> 8) & 0xff,
                    (int)(uintColor) & 0xff, alpha);
            }
            return Color4.Transparent;
        }
    }
}
