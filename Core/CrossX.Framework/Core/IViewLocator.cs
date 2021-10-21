using System.Reflection;

namespace CrossX.Framework.Core
{
    public interface IViewLocator
    {
        (string path, Assembly assembly) LocateView(object viewModel);
    }
}
