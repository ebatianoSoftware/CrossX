using CrossX.Abstractions.IoC;
using CrossX.Framework.ApplicationDefinition;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.IoC;
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
            var builder = new ScopeBuilder(servicesProvider);
            builder.WithInstance(this).As<Application>().As(GetType());

            var elementTypeMapping = new ElementTypeMapping(GetType().Assembly);
            builder.WithInstance(elementTypeMapping).As<IElementTypeMapping>();

            var applicationDefinition = LoadApplicationDefinition(elementTypeMapping, servicesProvider.GetService<IObjectFactory>());
            builder.WithInstance(applicationDefinition).As<IAppValues>();

            BeforeInitServices?.Invoke(servicesProvider, builder);
            InitServices(servicesProvider, builder);
            AfterInitServices?.Invoke(servicesProvider, builder);

            Services = builder.Build();

            RedrawService = servicesProvider.GetService<IRedrawService>();
            ObjectFactory = Services.GetService<IObjectFactory>();
        }

        private ApplicationElement LoadApplicationDefinition(IElementTypeMapping elementTypeMapping, IObjectFactory objectFactory)
        {
            try
            {
                XxElement element;
                using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Assembly.GetName().Name + ".App.xml"))
                {
                    var parser = objectFactory.Create<XxFileParser>(elementTypeMapping);
                    element = parser.Parse(stream);
                }

                var defObjectFactory = objectFactory.Create<XxDefinitionObjectFactory>(elementTypeMapping);
                return defObjectFactory.CreateObject<ApplicationElement>(element);
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
            //var frameLayout = new FrameLayout
            //{
            //    BackgroundColor = Color.DarkSlateBlue,
            //    Padding = new Thickness(100, 0, 0, 0)
            //};

            //frameLayout.Children.Add(ObjectFactory.Create<Label>().Set(l =>
            //{
            //    l.Text = "\xffc2";
            //    l.FontFamily = "FluentSystemIcons-Regular";
            //    l.ForegroundColor = Color.White;
            //    l.BackgroundColor = Color.Green;
            //    l.FontSize = 128;
            //    l.Margin = new Thickness(0, 0, 100, 0);
            //    l.HorizontalAlignment = Alignment.Center;
            //    l.VerticalAlignment = Alignment.Center;
            //    l.HorizontalTextAlignment = Alignment.Center;
            //    l.VerticalTextAlignment = Alignment.Center;
            //    l.Height = 100;
            //    l.Width = 500;
            //    l.FontMeasure = FontMeasure.Extended;
            //}));
            //MainView = frameLayout;
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
                if(Window.RootView.ProcessGesture(gesture))
                {
                    RedrawService.RequestRedraw();
                }
            }
            else
            {
                RedrawService.RequestRedraw();
            }
        }
    }
}
