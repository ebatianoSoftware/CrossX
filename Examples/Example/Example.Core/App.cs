using CrossX.Framework.Core;
using Example.Core.ViewModels;

namespace Example.Core
{
    public class App : Application
    {
        protected override void StartApp() => Load<MainWindowViewModel>();
    }
}