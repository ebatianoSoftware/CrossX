using CrossX.Abstractions.Windows;
using CrossX.Framework.Core;
using Example.Core.ViewModels;

namespace Example.Core
{
    public class App : Application
    {
        protected override void StartApp()
        {
            WindowsService.CreateWindow<MainWindowViewModel>(CreateWindowMode.MainWindow);
        }
    }
}