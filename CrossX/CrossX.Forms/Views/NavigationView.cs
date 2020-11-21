using CrossX.Forms.Attributes;
using CrossX.Forms.Styles;
using CrossX.Forms.Xml;
using CrossX.IO;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CrossX.Forms.Views
{
    internal class NavigationView : INavigation
    {
        private const string ViewBackNavigateTo = "ViewBackNavigateTo";
        private const string ViewNavigateFrom = "ViewNavigateFrom";
        private const string ViewNavigateTo = "ViewNavigateTo";
        private const string ViewBackNavigateFrom = "ViewBackNavigateFrom";

        private readonly IFilesRepository filesRepository;
        private readonly IObjectFactory objectFactory;
        private readonly IStylesServiceEx stylesService;
        private List<View> views = new List<View>();

        private Stack<FormsViewModel> popupNavigations = new Stack<FormsViewModel>();
        private Stack<FormsViewModel> viewModels = new Stack<FormsViewModel>();

        private Dictionary<Type, XNode> preloadedNodes = new Dictionary<Type, XNode>();

        public NavigationView(IFilesRepository filesRepository, IObjectFactory objectFactory, IStylesServiceEx stylesService)
        {
            this.filesRepository = filesRepository;
            this.objectFactory = objectFactory;
            this.stylesService = stylesService;
        }

        public void Navigate<TViewModel>(NavigationParameters parameters = null, params object[] args) where TViewModel : FormsViewModel
        {
            var viewModel = objectFactory.Create<TViewModel>();
            viewModel.SetNavigation(this);

            viewModels.Push(viewModel);

            views.LastOrDefault()?.Close(parameters?.NavigationFromEvent ?? ViewNavigateFrom);
            AddView(viewModel, parameters?.NavigationToEvent ?? ViewNavigateTo);
        }

        private XNode LoadViewForVm(FormsViewModel vm)
        {
            if (preloadedNodes.TryGetValue(vm.GetType(), out var node)) return node;

            var attr = vm.GetType().GetCustomAttribute<ViewAttribute>();
            if (attr is null) throw new InvalidOperationException();

            using (var stream = filesRepository.Open(attr.Path))
            {
                var xmlReader = XmlReader.Create(stream);
                node = XNode.ReadXml(xmlReader);
                stylesService.ApplyStyle(node);
            }
            if (node.Tag != "Page") throw new InvalidOperationException();

            preloadedNodes.Add(vm.GetType(), node);
            return node;
        }

        private void AddView(FormsViewModel vm, string @event)
        {
            var node = LoadViewForVm(vm);

            var view = objectFactory.Create<View>(vm);
            view.LoadView(node, @event);
            views.Add(view);
        }

        public void Draw(TimeSpan frameTime)
        {
            for (var idx = 0; idx < views.Count; ++idx)
            {
                views[idx].Draw(frameTime);
            }
        }

        public void Update(TimeSpan frameTime)
        {
            for(var idx =0; idx < views.Count; ++idx)
            {
                views[idx].Update(frameTime);
            }

            for(var idx =0; idx < views.Count;)
            {
                if (views[idx].IsFinished)
                {
                    views.RemoveAt(idx);
                    continue;
                }
                idx++;
            }
        }

        public void NavigatePopup<TViewModel>(NavigationParameters parameters = null, params object[] args) where TViewModel : FormsViewModel
        {
            popupNavigations.Push(views.Last().ViewModel);

            var viewModel = objectFactory.Create<TViewModel>();
            viewModel.SetNavigation(this);


            viewModels.Push(viewModel);
            AddView(viewModel, parameters?.NavigationToEvent ?? ViewNavigateTo);
        }

        public void FinishPopup(string closeEvent = null)
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;

            while(viewModels.Peek() != noPopupVm)
            {
                NavigateBackVm();
            }
            views.LastOrDefault()?.Close(closeEvent ?? ViewBackNavigateFrom);
        }

        public void NavigateBack(NavigationParameters parameters = null)
        {
            views.LastOrDefault()?.Close(parameters?.NavigationFromEvent ?? ViewBackNavigateFrom);
            NavigateBackVm();
            AddView(viewModels.Peek(), parameters?.NavigationToEvent ?? ViewBackNavigateTo);
        }

        private void NavigateBackVm()
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;
            viewModels.Pop();
            if (viewModels.Peek() == noPopupVm) popupNavigations.Pop();
        }

        public void Clear(string closeEvent = null)
        {
            popupNavigations.Clear();
            viewModels.Clear();
            foreach(var view in views)
            {
                view.Close(closeEvent ?? ViewNavigateFrom);
            }
        }
    }
}
