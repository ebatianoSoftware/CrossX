using CrossX.Abstractions.Async;
using CrossX.Abstractions.Input;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.Navigation;
using CrossX.Framework.Transforms;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xx.Definition;

namespace CrossX.Framework.UI.Containers
{
    public class NavigationFrame: View, IViewParent
    {
        private ICommand attachedToFrameCommand;
        private readonly IXxFileParser fileParser;
        private readonly IDispatcher dispatcher;
        private readonly IViewLocator viewLocator;
        private INavigationController navigationController;

        private View currentView = null;
        private bool layoutInvalid;

        public Window Window => Parent?.Window;
        public object NavigationController 
        {
            set
            {
                if(navigationController != null)
                {
                    navigationController.NavigationRequested -= OnNavigationRequested;
                }

                navigationController = value as INavigationController;

                if (navigationController != null)
                {
                    navigationController.NavigationRequested += OnNavigationRequested;
                    attachedToFrameCommand?.Execute(null);
                }
            }
        }

        public ICommand AttachedToFrameCommand
        {
            get => attachedToFrameCommand;
            set
            {
                attachedToFrameCommand = value;

                if (navigationController != null)
                {
                    attachedToFrameCommand.Execute(null);
                }
            }
        }


        public NavigationTransform NavigateToTransform { get; set; }
        public NavigationTransform NavigateFromTransform { get; set; }
        

        public NavigationFrame(IUIServices services, IXxFileParser fileParser, IDispatcher dispatcher, IViewLocator viewLocator) : base(services)
        {
            this.fileParser = fileParser;
            this.dispatcher = dispatcher;
            this.viewLocator = viewLocator;
        }

        private void OnNavigationRequested(object sender, NavigationRequest request)
        {
            var task = Task.Run(() =>
            {
               (var path, var assembly) = viewLocator.LocateView(request.ViewModel);
               path += ".xml";

               XxElement viewElement = fileParser.Parse(assembly, path, true);

               try
               {
                    var defObjectFactory = Services.ObjectFactory.Create<XxDefinitionObjectFactory>();
                    var page = defObjectFactory.CreateObject<Page>(viewElement);

                    dispatcher.BeginInvoke(() =>
                    {
                        page.RootView.DataContext = request.ViewModel;
                        page.RootView.Parent = this;
                        currentView?.Dispose();
                        currentView = page.RootView;
                        RecalculateLayout();
                    });
               }
               catch (Exception ex)
               {
                   Console.WriteLine(ex);
                   throw;
               }
            });
            request.AddNavigationTask(task);
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);
            currentView?.Render(canvas, opacity);
        }

        protected override bool OnProcesssUiKey(UiInputKey key)
        {
            if (currentView?.ProcessUiKey(key) == true) return true;
            return false;
        }

        public override void GetFocusables(IList<IFocusable> list)
        {
            currentView?.GetFocusables(list);
            base.GetFocusables(list);
        }

        protected override void OnUpdate(float time)
        {
            if(layoutInvalid)
            {
                RecalculateLayout();
            }
            base.OnUpdate(time);
            currentView?.Update(time);
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            return currentView?.PreviewGesture(gesture) ?? false;
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            return currentView?.ProcessGesture(gesture) ?? false;
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            return parentSize;
        }

        protected override void RecalculateLayout()
        {
            if (currentView == null) return;

            layoutInvalid = false;
            var child = currentView;
            var size = child.CalculateSize(Bounds.Size);
            var position = child.CalculatePosition(size, Bounds.Size);
            child.Bounds = new RectangleF(position, size);
            Invalidate();
        }

        public void InvalidateLayout()
        {
            layoutInvalid = true;
        }
    }
}
