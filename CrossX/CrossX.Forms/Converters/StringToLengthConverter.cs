using CrossX.Forms.Values;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CrossX.Forms.Converters
{
    internal class StringToLengthConverter : IValueConverter
    {
        private static IEnumerable<string> SplitAndKeep(string s, params char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }

        public object Convert(object strValue)
        {
            if (strValue is string text)
            {
                if (text == "Auto") return Length.Auto;

                int sign = 1;

                float value = 0;
                float percent = 0;

                foreach (var symbol in SplitAndKeep(text.Replace(" ", ""), '+', '-'))
                {
                    if (symbol == "-")
                    {
                        sign = -1;
                    }
                    else if (symbol == "+")
                    {
                        sign = 1;
                    }
                    else if (symbol.EndsWith("%", StringComparison.InvariantCulture))
                    {
                        var sym = symbol.Trim('%');
                        if (int.TryParse(sym, out var per))
                        {
                            percent += sign * per / 100.0f;
                        }
                    }
                    else if (float.TryParse(symbol, NumberStyles.AllowDecimalPoint | NumberStyles.Float, CultureInfo.InvariantCulture, out var floatVal))
                    {
                        value += sign * floatVal;
                    }
                }
                return new Length(percent, value);
            }
            return Length.Auto;
        }
    }
}
