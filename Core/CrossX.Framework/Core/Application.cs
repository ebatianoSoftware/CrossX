using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;
using CrossX.Framework.UI;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using System;
using System.Reflection;
using Xx.Toolkit;

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

        protected View MainView { get; private set; }

        void ICoreApplication.Initialize(IServicesProvider servicesProvider)
        {
            var builder = new ScopeBuilder().WithParent(servicesProvider);
            builder.WithInstance(this).As<Application>().As(GetType());

            var elementTypeMapping = new ElementTypeMapping(GetType().Assembly);
            builder.WithInstance(elementTypeMapping).As<IElementTypeMapping>();

            RedrawService = servicesProvider.GetService<IRedrawService>();

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);
            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();
            ObjectFactory = Services.GetService<IObjectFactory>();
        }

        void ICoreApplication.Run(Size size)
        {
            StartApp();
            MainView.Bounds = new RectangleF(0, 0, size.Width, size.Height);
        }

        void ICoreApplication.DoRender(Canvas canvas) => Render(canvas);

        void ICoreApplication.DoUpdate(TimeSpan ellapsedTime, Size size) => Update(ellapsedTime, size);

        protected abstract void StartApp();

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
            MainView?.Render(canvas);
        }

        protected virtual void InitServices(IServicesProvider systemServices, IScopeBuilder scopeBuilder)
        {

        }

        protected virtual (string path, Assembly assembly) LocateView(object viewModel)
        {
            var vmType = viewModel.GetType();
            var viewNamespace = vmType.Namespace.Replace("ViewModels", "Views");
            var viewName = vmType.Name.Replace("ViewModel", "View");
            return (viewNamespace + '.' + viewName, vmType.Assembly);
        }

        protected bool Load<TViewModel>(TViewModel vm = null) where TViewModel: class
        {
            if(vm == null)
            {
                vm = ObjectFactory.Create<TViewModel>();
            }
            var viewPath = LocateView(vm) + ".xml";

            var frameLayout = new FrameLayout
            {
                BackgroundColor = Color.DarkSlateBlue,
                Padding = new Thickness(100, 0, 0, 0)
            };

            frameLayout.Children.Add(ObjectFactory.Create<Label>().Set(l =>
            {
                l.Text = "\xe88a";
                l.FontFamily = "Material Icons";
                l.TextColor = Color.White;
                l.BackgroundColor = Color.Green;
                l.FontSize = 128;
                l.Margin = new Thickness(0, 0, 100, 0);
                l.HorizontalAlignment = Alignment.Center;
                l.VerticalAlignment = Alignment.Center;
                l.HorizontalTextAlignment = Alignment.Center;
                l.VerticalTextAlignment = Alignment.Center;
                l.Height = 100;
                l.Width = 500;
                l.FontMeasure = FontMeasure.Extended;
            }));
            MainView = frameLayout;
            return true;
        }
    }
}
