using CrossX.Forms.Controls;
using CrossX.Forms.Values;

namespace CrossX.Forms.Converters
{
    internal class StringToIntConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if(value is string str)
            {
                int.TryParse(str, out var result);
                return result;
            }
            return 0;
        }
    }

    internal class StringToFloatConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
                float.TryParse(str, out var result);
                return result;
            }
            return 0.0f;
        }
    }

    internal class StringToLengthConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
            }
            return Length.Auto;
        }
    }

    internal class StringToMarginConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
            }
            return Margin.Zero;
        }
    }

    internal class StringToGridRowColumnDefinitionsConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {

            }
            return new GridLength[0];
        }
    }

    internal class StringToColorConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string str)
            {
                
            }
            return Color4.Transparent;
        }
    }
}
