using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
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
