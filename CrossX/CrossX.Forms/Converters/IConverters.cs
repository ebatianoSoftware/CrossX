using System;

namespace CrossX.Forms.Converters
{
    public interface IConverters
    {
        void RegisterConverter<TFrom, TTo>(IValueConverter converter);
        void RegisterConverter(string name, IValueConverter converter);

        IValueConverter FindConverter(Type from, Type to);
        IValueConverter FindConverter(string name);
    }
}
