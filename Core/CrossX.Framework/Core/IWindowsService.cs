using CrossX.Abstractions.IoC;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using Xx.Definition;

namespace CrossX.Framework.Core
{
    public interface IWindowsService
    {
        Window Load<TViewModel>(TViewModel vm = null) where TViewModel : class;
        void ShowWindow(Window window);
    }

    public abstract class WindowService : IWindowsService
    {
        private readonly IObjectFactory objectFactory;
        private readonly IXxFileParser xxFileParser;
        private readonly IViewLocator viewLocator;

        protected WindowService(IObjectFactory objectFactory, IXxFileParser xxFileParser, IViewLocator viewLocator)
        {
            this.objectFactory = objectFactory;
            this.xxFileParser = xxFileParser;
            this.viewLocator = viewLocator;
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

        public abstract void ShowWindow(Window window);
    }
}
