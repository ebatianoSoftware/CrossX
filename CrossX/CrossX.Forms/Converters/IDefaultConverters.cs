using System;

namespace CrossX.Forms.Converters
{
    public interface IDefaultConverters
    {
        void RegisterConverter<TFrom, TTo>(IValueConverter converter);
        IValueConverter FindConverter(Type from, Type to);
    }
}
