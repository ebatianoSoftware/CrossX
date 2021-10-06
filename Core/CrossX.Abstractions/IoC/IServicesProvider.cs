using System;

namespace CrossX.Abstractions.IoC
{
    public interface IServicesProvider : IServiceProvider, IDisposable
    {
        TService GetService<TService>();
        bool TryResolveInstance(Type type, out object instance);
        bool TryResolveInstance<TType>(out TType instance);
    }
}
