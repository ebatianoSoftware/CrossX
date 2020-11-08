using CrossX.Forms.Attributes;
using CrossX.Forms.Xml;
using CrossX.IO;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CrossX.Forms.View
{
    internal class NavigationView : INavigation
    {
        private readonly IFilesRepository filesRepository;
        private readonly IObjectFactory objectFactory;

        private List<View> views = new List<View>();

        private Stack<FormsViewModel> popupNavigations = new Stack<FormsViewModel>();
        private Stack<FormsViewModel> viewModels = new Stack<FormsViewModel>();

        public NavigationView(IFilesRepository filesRepository, IObjectFactory objectFactory)
        {
            this.filesRepository = filesRepository;
            this.objectFactory = objectFactory;
        }

        public void Navigate<TViewModel>(params object[] args) where TViewModel: FormsViewModel
        {
            var viewModel = objectFactory.Create<TViewModel>();
            viewModel.SetNavigation(this);

            viewModels.Push(viewModel);
            AddView(viewModel);
        }

        private void AddView(FormsViewModel vm)
        {
            var attr = vm.GetType().GetCustomAttribute<ViewAttribute>();
            if (attr is null) throw new InvalidOperationException();

            XNode node;
            using (var stream = filesRepository.Open(attr.Path))
            {
                var xmlReader = XmlReader.Create(stream);
                node = XNode.ReadXml(xmlReader);
            }

            if (node.Tag != "Page") throw new InvalidOperationException();

            var view = objectFactory.Create<View>(vm);
            view.Root = view.Load(node.Nodes[0]);

            foreach (var cv in views)
            {
                cv.Close();
            }
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
            for(var idx =0; idx < views.Count; )
            {
                if(views[idx].IsFinished)
                {
                    views.RemoveAt(idx);
                    continue;
                }
                views[idx].Update(frameTime);
                ++idx;
            }
        }

        public void NavigatePopup<TViewModel>(params object[] args) where TViewModel : FormsViewModel
        {
            popupNavigations.Push(views.Last().ViewModel);
            Navigate<TViewModel>(args);
        }

        public void FinishPopup()
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;
            while(viewModels.Peek() != noPopupVm)
            {
                NavigateBack();
            }
        }

        public void NavigateBack()
        {
            var noPopupVm = popupNavigations.Count > 0 ? popupNavigations.Pop() : null;
            viewModels.Pop();
            if (viewModels.Peek() == noPopupVm) popupNavigations.Pop();

        }
    }
}
