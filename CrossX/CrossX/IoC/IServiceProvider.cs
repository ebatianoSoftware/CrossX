using System;

namespace CrossX.IoC
{
    public interface IServiceProvider
    {
        object GetService(Type serviceType);
        TService GetService<TService>();
        bool TryResolveInstance(Type type, out object instance);
        bool TryResolveInstance<TType>(out TType instance);
    }
}
