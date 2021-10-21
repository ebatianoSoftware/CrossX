using System.Reflection;

namespace CrossX.Framework.Core
{
    internal class DefaultViewLocator : IViewLocator
    {
        public (string path, Assembly assembly) LocateView(object viewModel)
        {
            var vmType = viewModel.GetType();
            var viewNamespace = vmType.Namespace.Replace("ViewModels", "Views");
            var viewName = vmType.Name.Replace("ViewModel", "View");
            return (viewNamespace + '.' + viewName, vmType.Assembly);
        }
    }
}
