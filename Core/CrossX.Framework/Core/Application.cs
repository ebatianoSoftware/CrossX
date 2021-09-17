using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;
using CrossX.Framework.UI;
using System;

namespace CrossX.Framework.Core
{
    public abstract class Application: ICoreApplication
    {
        public delegate void InitServicesDelegate(IServicesProvider systemServices, IScopeBuilder scopeBuilder);
        protected IServicesProvider Services { get; private set; }
        protected IObjectFactory ObjectFactory { get; private set; }

        public event InitServicesDelegate BeforeInitServices;
        public event InitServicesDelegate AfterInitServices;

        protected IRedrawService RedrawService { get; private set; }

        protected View MainView { get; set; }

        void ICoreApplication.Initialize(IServicesProvider servicesProvider)
        {
            var builder = new ScopeBuilder().WithParent(servicesProvider);
            builder.WithInstance(this).As<Application>().As(GetType());

            RedrawService = servicesProvider.GetService<IRedrawService>();

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);
            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();
            ObjectFactory = Services.GetService<IObjectFactory>();
        }

        void ICoreApplication.Run(Size size)
        {
            StartApp(size);
            MainView.Bounds = new RectangleF(0, 0, size.Width, size.Height);
        }

        void ICoreApplication.DoRender(Canvas canvas) => Render(canvas);

        void ICoreApplication.DoUpdate(TimeSpan ellapsedTime, Size size) => Update(ellapsedTime, size);

        protected abstract void StartApp(Size size);

        protected virtual void Update(TimeSpan ellapsedTime, Size size)
        {
            var bounds = new RectangleF(0, 0, size.Width, size.Height);
            if (MainView.Bounds != bounds)
            {
                MainView.Bounds = bounds;
                RedrawService.RequestRedraw();
            }
            MainView.Update((float)ellapsedTime.TotalSeconds);
        }

        protected virtual void Render(Canvas canvas)
        {
            MainView.Render(canvas);
        }

        protected virtual void InitServices(IServicesProvider systemServices, IScopeBuilder scopeBuilder)
        {

        }
    }
}
