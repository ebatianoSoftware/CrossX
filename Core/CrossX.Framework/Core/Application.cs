using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;

namespace CrossX.Framework.Core
{
    public abstract class Application: ICoreApplication
    {
        public delegate void InitServicesDelegate(IServicesProvider systemServices, IScopeBuilder scopeBuilder);
        protected IServicesProvider Services { get; private set; }
        protected IObjectFactory ObjectFactory { get; private set; }
        protected Canvas Canvas { get; private set; }

        public event InitServicesDelegate BeforeInitServices;
        public event InitServicesDelegate AfterInitServices;

        void ICoreApplication.Initialize(IServicesProvider servicesProvider)
        {
            var builder = new ScopeBuilder().WithParent(servicesProvider);

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);
            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();
            ObjectFactory = Services.GetService<IObjectFactory>();
        }

        void ICoreApplication.Run(Canvas canvas)
        {
            Canvas = canvas;
            StartApp();
        }

        void ICoreApplication.Render()
        {

        }

        protected abstract void StartApp();

        protected virtual void InitServices(IServicesProvider systemServices, IScopeBuilder scopeBuilder)
        {

        }
    }
}
