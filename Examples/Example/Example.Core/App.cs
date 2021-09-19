using CrossX.Abstractions.Async;
using CrossX.Framework;
using CrossX.Framework.Core;
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

            sequencer = Services.GetService<ISequencer>();
            var sequence = sequencer.Run(ChangeBackgroundColor());

            sequencer.Run(Sequence.DelayAction(5, () => sequence.Cancel()));
        }

        private IEnumerable<Sequence> ChangeBackgroundColor()
        {
            yield return Sequence.WaitForNextFrame();

            var random = new Random();

            while(true)
            {
                MainView.BackgroundColor = new Color((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                RedrawService.RequestRedraw();
                yield return Sequence.WaitForSeconds(0.2);
            }
        }
    }
}