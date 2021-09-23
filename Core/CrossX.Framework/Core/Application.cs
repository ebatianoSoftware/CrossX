using CrossX.Abstractions.IoC;
using CrossX.Framework.ApplicationDefinition;
using CrossX.Framework.Binding;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.IoC;
using CrossX.Framework.UI;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Numerics;
using System.Reflection;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.Core
{
    public abstract class Application: ICoreApplication
    {
        public delegate void InitServicesDelegate(IServicesProvider systemServices, IScopeBuilder scopeBuilder);
        protected IServicesProvider Services { get; private set; }
        protected IObjectFactory ObjectFactory { get; private set; }

        private readonly GestureProcessor gestureProcessor = new GestureProcessor();

        public event InitServicesDelegate BeforeInitServices;
        public event InitServicesDelegate AfterInitServices;

        protected IRedrawService RedrawService { get; private set; }
        protected Window Window { get; private set; }

        void ICoreApplication.Initialize(IServicesProvider servicesProvider)
        {
            var assembly = GetType().Assembly;
            var builder = new ScopeBuilder(servicesProvider);
            var elementTypeMapping = new ElementTypeMapping(assembly);
            var appValues = new AppValues();
            var conversionService = new ConversionService(assembly);
            var bindingService = new BindingService(appValues, conversionService);

            LoadApplicationDefinition(appValues, bindingService, elementTypeMapping, servicesProvider.GetService<IObjectFactory>());

            builder
                .WithInstance(this).As<Application>().As(GetType())
                .WithInstance(elementTypeMapping).As<IElementTypeMapping>()
                .WithInstance(appValues).As<IAppValues>()
                .WithInstance(bindingService).As<IBindingService>()
                .WithType<UIServices>().As<IUIServices>().AsSingleton();

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);
            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();

            RedrawService = servicesProvider.GetService<IRedrawService>();
            ObjectFactory = Services.GetService<IObjectFactory>();
        }

        private void LoadApplicationDefinition(IAppValues appValues, IBindingService bindingService, IElementTypeMapping elementTypeMapping, IObjectFactory objectFactory)
        {
            try
            {
                XxElement element;
                using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Assembly.GetName().Name + ".App.xml"))
                {
                    var parser = objectFactory.Create<XxFileParser>(elementTypeMapping);
                    element = parser.Parse(stream);
                }

                var scopeBuilder = objectFactory.Create<IScopeBuilder>();

                scopeBuilder
                    .WithInstance(elementTypeMapping).As<IElementTypeMapping>()
                    .WithInstance(appValues).As<IAppValues>()
                    .WithInstance(bindingService).As<IBindingService>();

                var services = scopeBuilder.Build();
                var defObjectFactory = services.GetService<IObjectFactory>().Create<XxDefinitionObjectFactory>();

                defObjectFactory.CreateObject<ApplicationElement>(element);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        void ICoreApplication.Run()
        {
            StartApp();
        }

        void ICoreApplication.DoRender(Canvas canvas) => Render(canvas);

        void ICoreApplication.DoUpdate(TimeSpan ellapsedTime, Size size) => Update(ellapsedTime, size);

        protected abstract void StartApp();

        protected virtual void Update(TimeSpan ellapsedTime, Size size)
        {
            if (Window.Size != size)
            {
                Window.Size = size;
                RedrawService.RequestRedraw();
            }
            Window.Update((float)ellapsedTime.TotalSeconds);
        }

        protected virtual void Render(Canvas canvas)
        {
            Window?.Render(canvas);
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
            var (viewPath, assembly) = LocateView(vm);
            viewPath += ".xml";

            XxElement viewElement;

            try
            {
                using (var stream = assembly.GetManifestResourceStream(viewPath))
                {
                    var parser = ObjectFactory.Create<XxFileParser>();
                    viewElement = parser.Parse(stream);
                }

                var defObjectFactory = ObjectFactory.Create<XxDefinitionObjectFactory>();
                Window = defObjectFactory.CreateObject<Window>(viewElement);
                Window.DataContext = vm;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return true;
        }

        void ICoreApplication.OnPointerDown(PointerId pointerId, Vector2 position)
        {
            var gesture = gestureProcessor.OnPointerDown(pointerId, position);
            PropagateGesture(gesture);
        }

        void ICoreApplication.OnPointerUp(PointerId pointerId, Vector2 position)
        {
            var gesture = gestureProcessor.OnPointerUp(pointerId, position);
            PropagateGesture(gesture);
        }

        void ICoreApplication.OnPointerMove(PointerId pointerId, Vector2 position)
        {
            var gesture = gestureProcessor.OnPointerMove(pointerId, position);
            PropagateGesture(gesture);
        }

        void ICoreApplication.OnPointerCancel(PointerId pointerId)
        {
            var gesture = gestureProcessor.OnPointerCancel(pointerId);
            PropagateGesture(gesture);
        }

        private void PropagateGesture(Gesture gesture)
        {
            if (gesture == null) return;
            if(!Window.RootView.PreviewGesture(gesture))
            {
                Window.RootView.ProcessGesture(gesture);
            }
        }

        public void Dispose() => Window.Dispose();
    }
}
