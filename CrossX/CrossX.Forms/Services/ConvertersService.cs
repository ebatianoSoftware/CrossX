using CrossX.Forms.Converters;
using System;
using System.Collections.Generic;

namespace CrossX.Forms.Services
{
    internal class ConvertersService : IConverters
    {
        private readonly Dictionary<Tuple<Type, Type>, IValueConverter> converters = new Dictionary<Tuple<Type, Type>, IValueConverter>();
        private readonly Dictionary<string, IValueConverter> convertersByName = new Dictionary<string, IValueConverter>();

        public IValueConverter FindConverter(Type from, Type to)
        {
            converters.TryGetValue(Tuple.Create(from, to), out var converter);
            return converter;
        }

        public IValueConverter FindConverter(string name)
        {
            convertersByName.TryGetValue(name, out var converter);
            return converter;
        }

        public void RegisterConverter<TFrom, TTo>(IValueConverter converter)
        {
            converters.Add(Tuple.Create(typeof(TFrom), typeof(TTo)), converter);
        }

        public void RegisterConverter(string name, IValueConverter converter)
        {
            convertersByName.Add(name, converter);
        }
    }
}
