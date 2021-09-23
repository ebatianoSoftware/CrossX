using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public ICommand TestCommand { get; }

        public string ImageUrl { get; private set; } = "https://picsum.photos/400/300";

        public string Stopwatch { get; private set; } = "00:00.000";

        public MainWindowViewModel(IObjectFactory objectFactory, ISequencer sequencer) : base(objectFactory)
        {
            TestCommand = new SyncCommand(Test);
            this.sequencer = sequencer;

            sequencer.Run(Count());
        }

        private IEnumerable<Sequence> Count()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                var ellapsed = stopwatch.Elapsed;
                Stopwatch = $"{ellapsed.Minutes:00}:{ellapsed.Seconds:00}.{ellapsed.Milliseconds:000}";
                RaisePropertyChanged(nameof(Stopwatch));
                yield return Sequence.WaitForSeconds(0.1);
            }
        }

        Random rand = new Random();
        private readonly ISequencer sequencer;

        private void Test()
        {
            ImageUrl = "";
            RaisePropertyChanged(nameof(ImageUrl));

            ImageUrl = $"https://picsum.photos/{rand.Next(300,400)}/{rand.Next(300, 400)}";
            RaisePropertyChanged(nameof(ImageUrl));
        }

        protected override Task InitializeFirstPage()
        {
            return Navigation.Navigate<MainPageViewModel>(out var viewModel);
        }
    }
}
