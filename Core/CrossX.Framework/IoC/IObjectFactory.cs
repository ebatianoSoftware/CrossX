using System;

namespace CrossX.Framework.IoC
{
    public interface IObjectFactory
    {
        object Create(Type type, params object[] parameters);
        TObject Create<TObject>(params object[] parameters);
    }
}
