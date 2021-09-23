using CrossX.Abstractions.Async;
using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.Graphics;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using Example.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Core
{
    public class App : Application
    {
        private ISequencer sequencer;
        protected override void StartApp()
        {
            Load<MainWindowViewModel>();

            sequencer = Services.GetService<ISequencer>();
            //var sequence = sequencer.Run(TestSequence());

            //sequencer.Run(Sequence.DelayAction(10, () => sequence.Cancel()));
        }


        protected override void Update(TimeSpan ellapsedTime, Size size)
        {
            base.Update(ellapsedTime, size);
            time += ellapsedTime.TotalSeconds;
            //RedrawService.RequestRedraw();
        }

        double time = 0.0001;

        private Dictionary<int, string> fpsStrings = new Dictionary<int, string>();
        protected override void Render(Canvas canvas)
        {
            base.Render(canvas);

            var fps = 1 / time;
            time = 0.0001;

            var font = Services.GetService<IFontManager>().FindFont("Consolas", 12, FontWeight.Normal, false);
            canvas.DrawText($"{fps}", font, new RectangleF(0, 0, 1000, 100), TextAlign.Left, Color.White, FontMeasure.Strict);
        }

        private IEnumerable<Sequence> TestSequence()
        {
            yield return Sequence.WaitForNextFrame();

            var random = new Random();
            while(true)
            {
                yield return Sequence.WaitForSeconds(1);
                ((ImageView)(Window.RootView as ViewContainer).Children.First(o => o is ImageView)).Source = $"https://picsum.photos/{random.Next(300,400)}/{random.Next(300, 400)}";
            }

            //double r = Window.RootView.BackgroundColor.R;
            //double g = Window.RootView.BackgroundColor.G;
            //double b = Window.RootView.BackgroundColor.B;

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            
            //while(true)
            //{
            //    yield return Sequence.WaitForSeconds(0.05);

            //    var totalSeconds = watch.Elapsed.TotalSeconds;

            //    r += Math.Sin(totalSeconds);
            //    g += Math.Cos(totalSeconds);
            //    b += Math.Cos(totalSeconds) + Math.Sin(totalSeconds);

            //    var offsetX = Math.Sin(totalSeconds * 2) * 150;
            //    var offsetY = Math.Cos(totalSeconds * 2) * 150;

            //    Window.RootView.BackgroundColor = new Color(
            //        (byte)Math.Min(255, Math.Max(0, (int)r)),
            //        (byte)Math.Min(255, Math.Max(0, (int)g)),
            //        (byte)Math.Min(255, Math.Max(0, (int)b)));

            //    ((Label)(Window.RootView as ViewContainer).Children[0]).Text = $"{watch.Elapsed.Minutes:00}:{watch.Elapsed.Seconds:00}.{watch.Elapsed.Milliseconds:000}";
            //    ((Label)(Window.RootView as ViewContainer).Children[0]).Margin = new Thickness(offsetX, offsetY, -offsetX, -offsetY);

            //    RedrawService.RequestRedraw();
            //}
        }
    }
}