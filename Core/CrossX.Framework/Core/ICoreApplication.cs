using CrossX.Abstractions.IoC;
using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework.Core
{
    public interface ICoreApplication
    {
        void Run(Size size);
        void DoUpdate(TimeSpan ellapsedTime, Size size);
        void DoRender(Canvas canvas);
        void Initialize(IServicesProvider servicesProvider);
    }
}
