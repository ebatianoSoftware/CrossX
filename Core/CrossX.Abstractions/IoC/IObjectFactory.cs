using System;

namespace CrossX.Abstractions.IoC
{
    public interface IObjectFactory
    {
        object Create(Type type, params object[] parameters);
        TObject Create<TObject>(params object[] parameters);
    }
}
