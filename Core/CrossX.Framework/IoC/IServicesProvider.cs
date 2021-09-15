using System;

namespace CrossX.Framework.IoC
{
    public interface IServicesProvider : IServiceProvider, IDisposable
    {
        TService GetService<TService>();
        bool TryResolveInstance(Type type, out object instance);
        bool TryResolveInstance<TType>(out TType instance);
    }
}
