using CrossX.Framework;
using CrossX.Framework.Core;
using Example.Core.ViewModels;
using System;

namespace Example.Core
{
    public class App : Application
    {
        protected override void StartApp() => Load<MainWindowViewModel>();

        protected override void Update(TimeSpan ellapsedTime, Size size)
        {
            base.Update(ellapsedTime, size);
            //RedrawService.RequestRedraw();
        }
    }
}