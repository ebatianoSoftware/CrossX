using CrossX.Forms.Converters;
using System;
using System.Collections.Generic;

namespace CrossX.Forms.Services
{
    internal class DefaultConverters : IDefaultConverters
    {
        private readonly Dictionary<Tuple<Type, Type>, IValueConverter> converters = new Dictionary<Tuple<Type, Type>, IValueConverter>();

        public IValueConverter FindConverter(Type from, Type to)
        {
            converters.TryGetValue(Tuple.Create(from, to), out var converter);
            return converter;
        }

        public void RegisterConverter<TFrom, TTo>(IValueConverter converter)
        {
            converters.Add(Tuple.Create(typeof(TFrom), typeof(TTo)), converter);
        }
    }
}
