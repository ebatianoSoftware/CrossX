using CrossX.Abstractions.IoC;
using System;

namespace CrossX.Framework.Core
{
    public interface ICoreApplication: IDisposable
    {
        void Load();
        IServicesProvider Initialize(IServicesProvider servicesProvider);

        event InitServicesDelegate BeforeInitServices;
        event InitServicesDelegate AfterInitServices;
    }
}
