using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.UI;
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
                    l.Text = "To jest jakiú testowy tekst ∆∆ç”";
                    l.FontFamily = "Segoe UI";
                    l.TextColor = Color.White;
                    l.BackgroundColor = Color.Green;
                    l.FontSize = 32;
                    l.Margin = new Thickness(0, 0, 100, 0);
                    l.HorizontalAlignment = Alignment.Center;
                    l.VerticalAlignment = Alignment.Center;
                    l.HorizontalTextAlignment = Alignment.Start;
                    l.VerticalTextAlignment = Alignment.End;
                    l.Height = 100;
                    l.Width = 500;
                    l.FontMeasure = FontMeasure.Extended;
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
