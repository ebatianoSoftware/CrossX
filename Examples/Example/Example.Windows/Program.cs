using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using CrossX.WindowsForms;
using System;

namespace Example.Windows
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XxRunner.Run(new App());
        }

        private class App : Application
        {
            protected override void StartApp(Size size)
            {
                var frameLayout = new FrameLayout
                {
                    BackgroundColor = Color.DarkSlateBlue,
                    Padding = new Thickness(100,0,0,0)
                };

                frameLayout.Children.Add( ObjectFactory.Create<Label>().Set( l=>
                {
                    l.Text = "To jest jaki� testowy tekst";
                    l.FontFamily = "Segoe UI";
                    l.TextColor = Color.White;
                    l.FontSize = 32;
                    l.Margin = new Thickness(0, 0, 100, 0);
                }));
                MainView = frameLayout;
            }

            protected override void Update(TimeSpan ellapsedTime, Size size)
            {
                base.Update(ellapsedTime, size);
                //RedrawService.RequestRedraw();
            }
        }
    }
}
