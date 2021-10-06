using System;

namespace CrossX.Abstractions.Mvvm
{
    public class ImplicitValueConverterAttribute: Attribute
    {
        public ImplicitValueConverterAttribute(Type from, Type to)
        {
            From = from;
            To = to;
        }

        public Type From { get; }
        public Type To { get; }
    }
}
