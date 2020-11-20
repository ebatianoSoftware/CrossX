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

        public void Navigate<TViewModel>(params object[] args) where TViewModel: FormsViewModel
        {
            var viewModel = objectFactory.Create<TViewModel>();
            viewModel.SetNavigation(this);

            viewModels.Push(viewModel);

            views.LastOrDefault()?.Close(false);
            AddView(viewModel, false);
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

        private void AddView(FormsViewModel vm, bool fromBackNavigation)
        {
            var node = LoadViewForVm(vm);

            var view = objectFactory.Create<View>(vm);
            view.LoadView(node, fromBackNavigation);
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

        public void NavigatePopup<TViewModel>(params object[] args) where TViewModel : FormsViewModel
        {
            popupNavigations.Push(views.Last().ViewModel);

            var viewModel = objectFactory.Create<TViewModel>();
            viewModel.SetNavigation(this);

            viewModels.Push(viewModel);
            AddView(viewModel, false);
        }

        public void FinishPopup()
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;

            while(viewModels.Peek() != noPopupVm)
            {
                NavigateBackVm();
            }
            views.LastOrDefault()?.Close(true);
        }

        public void NavigateBack()
        {
            views.LastOrDefault()?.Close(true);
            NavigateBackVm();
            AddView(viewModels.Peek(), true);
        }

        private void NavigateBackVm()
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;
            viewModels.Pop();
            if (viewModels.Peek() == noPopupVm) popupNavigations.Pop();
        }
    }
}
