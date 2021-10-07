using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Windows;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xx.Definition;

namespace CrossX.Framework.Core
{
    public abstract class WindowService : IWindowsService
    {
        public Window MainWindow { get; protected set; }

        private readonly IObjectFactory objectFactory;
        private readonly IXxFileParser xxFileParser;
        private readonly IViewLocator viewLocator;

        protected WindowService(IObjectFactory objectFactory, IXxFileParser xxFileParser, IViewLocator viewLocator)
        {
            this.objectFactory = objectFactory;
            this.xxFileParser = xxFileParser;
            this.viewLocator = viewLocator;
        }

        public TViewModel CreateWindow<TViewModel>(CreateWindowMode createMode = CreateWindowMode.ChildToMain, TViewModel vm = null) where TViewModel : class
        {
            var window = Load(vm);
            ShowWindow(window, createMode);
            return (TViewModel)window.DataContext;
        }

        public Window Load<TViewModel>(TViewModel vm = null) where TViewModel : class
        {
            if(vm == null)
            {
                vm = objectFactory.Create<TViewModel>();
            }
            var (viewPath, assembly) = viewLocator.LocateView(vm);
            viewPath += ".xml";

            XxElement viewElement = xxFileParser.Parse(assembly, viewPath, true);

            try
            {
                var defObjectFactory = objectFactory.Create<XxDefinitionObjectFactory>();
                var window = defObjectFactory.CreateObject<Window>(viewElement);
                window.DataContext = vm;
                return window;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Task<TResult> ShowPopup<TResult, TViewModel>(TResult defaultResult = default) where TViewModel : class, IModalContext<TResult>
        {
            var window = Load<TViewModel>();
            ShowWindow(window, CreateWindowMode.Modal);

            var context = (IModalContext<TResult>)window.DataContext;

            var autoresetEvent = new AutoResetEvent(false);

            TResult result = default;
            bool resultIsValid = false;

            context.CloseWithResult += r =>
            {
                result = r;
                resultIsValid = true;
                autoresetEvent.Set();
                window.Close();
            };

            window.Disposed += () =>
            {
                autoresetEvent.Set();
            };

            return Task.Run(() =>
            {
                autoresetEvent.WaitOne();

                if (!resultIsValid) return defaultResult;
                return result;
            });
        }

        public abstract void ShowWindow(Window window, CreateWindowMode windowMode);
    }
}
