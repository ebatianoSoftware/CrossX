using CrossX.IoC;

namespace CrossX.Forms
{
    public interface IServicesInitializer
    {
        void InitializeServices(ScopeBuilder scopeBuilder);
    }
}
