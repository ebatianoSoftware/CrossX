using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;

namespace CrossX.Framework.Core
{
    public interface ICoreApplication
    {
        void Run(Canvas canvas);
        void Render();
        void Initialize(IServicesProvider servicesProvider);
    }
}
