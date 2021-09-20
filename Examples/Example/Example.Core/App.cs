using CrossX.Abstractions.Async;
using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using Example.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Example.Core
{
    public class App : Application
    {
        private ISequencer sequencer;
        protected override void StartApp()
        {
            Load<MainWindowViewModel>();

            sequencer = Services.GetService<ISequencer>();
            var sequence = sequencer.Run(ChangeBackgroundColor());

            //sequencer.Run(Sequence.DelayAction(10, () => sequence.Cancel()));
        }

        private IEnumerable<Sequence> ChangeBackgroundColor()
        {
            yield return Sequence.WaitForNextFrame();

            double r = Window.RootView.BackgroundColor.R;
            double g = Window.RootView.BackgroundColor.G;
            double b = Window.RootView.BackgroundColor.B;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            while(true)
            {
                yield return Sequence.WaitForSeconds(0.05);

                var totalSeconds = watch.Elapsed.TotalSeconds;

                r += Math.Sin(totalSeconds);
                g += Math.Cos(totalSeconds);
                b += Math.Cos(totalSeconds) + Math.Sin(totalSeconds);

                Window.RootView.BackgroundColor = new Color(
                    (byte)Math.Min(255, Math.Max(0, (int)r)),
                    (byte)Math.Min(255, Math.Max(0, (int)g)),
                    (byte)Math.Min(255, Math.Max(0, (int)b)));

                ((Label)(Window.RootView as ViewContainer).Children[0]).Text = $"{watch.Elapsed.Minutes:00}:{watch.Elapsed.Seconds:00}.{watch.Elapsed.Milliseconds:000}";

                RedrawService.RequestRedraw();
            }
        }
    }
}