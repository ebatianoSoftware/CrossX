using S2IoC;

namespace CrossX.Forms
{
    public interface IServicesInitializer
    {
        void InitializeServices(IScopeBuilder scopeBuilder, IServicesProvider platformServices);
    }
}
