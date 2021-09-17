using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using System;

namespace Example.Core
{
    public class App : Application
    {
        protected override void StartApp(Size size)
        {
            var frameLayout = new FrameLayout
            {
                BackgroundColor = Color.DarkSlateBlue,
                Padding = new Thickness(100, 0, 0, 0)
            };

            frameLayout.Children.Add(ObjectFactory.Create<Label>().Set(l =>
            {
                l.Text = "\xe88a";
                l.FontFamily = "Material Icons";
                l.TextColor = Color.White;
                l.BackgroundColor = Color.Green;
                l.FontSize = 128;
                l.Margin = new Thickness(0, 0, 100, 0);
                l.HorizontalAlignment = Alignment.Center;
                l.VerticalAlignment = Alignment.Center;
                l.HorizontalTextAlignment = Alignment.Center;
                l.VerticalTextAlignment = Alignment.Center;
                l.Height = 100;
                l.Width = 500;
                l.FontMeasure = FontMeasure.Extended;
            }));
            MainView = frameLayout;
        }

        protected override void Update(TimeSpan ellapsedTime, Size size)
        {
            base.Update(ellapsedTime, size);
        }
    }
}
