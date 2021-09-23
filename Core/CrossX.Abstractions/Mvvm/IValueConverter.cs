using System;

namespace CrossX.Abstractions.Mvvm
{
    public interface IValueConverter
    {
        object Convert(object value);
    }

    public class ValueConverterAttribute: Attribute
    {
        public ValueConverterAttribute(Type from, Type to)
        {
            From = from;
            To = to;
        }

        public Type From { get; }
        public Type To { get; }
    }
}
