using CrossX.Abstractions.Async;
using CrossX.Framework;
using CrossX.Framework.Core;
using CrossX.Framework.UI.Containers;
using CrossX.Framework.UI.Controls;
using Example.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace Example.Core
{
    public class App : Application
    {
        private ISequencer sequencer;
        protected override void StartApp()
        {
            Load<MainWindowViewModel>();

            //sequencer = Services.GetService<ISequencer>();
            //var sequence = sequencer.Run(ChangeBackgroundColor());

            //sequencer.Run(Sequence.DelayAction(10, () => sequence.Cancel()));
        }

        private IEnumerable<Sequence> ChangeBackgroundColor()
        {
            yield return Sequence.WaitForNextFrame();

            var random = new Random();

            var index = 0;
            while(true)
            {
                Window.RootView.BackgroundColor = new Color((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                RedrawService.RequestRedraw();
                yield return Sequence.WaitForSeconds(0.1);
                ((Label)(Window.RootView as ViewContainer).Children[0]).Text = $"{(char)(21 + index++)}" + (index%2==0 ? "#" : "");
            }
        }
    }
}