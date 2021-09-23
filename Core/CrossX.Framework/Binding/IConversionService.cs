using System;

namespace CrossX.Framework.Binding
{
    public interface IConversionService
    {
        object Convert(object value, Type type);
    }
}
