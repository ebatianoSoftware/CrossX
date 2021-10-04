using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public class TestButtonData
        {
            public string Text { get; }
            public ICommand ClickCommand { get; }

            public TestButtonData(string text, ICommand clickCommand)
            {
                Text = text;
                ClickCommand = clickCommand;
            }
        }

        public ICommand TestCommand { get; }

        public ImageDescriptor Image { get => image; private set => SetProperty(ref image, value); }

        public string Stopwatch { get => stopwatch; private set => SetProperty(ref stopwatch, value); }

        public float SliderValue { get => sliderValue; set => SetProperty(ref sliderValue, value); }

        public ObservableCollection<TestButtonData> Items { get; } = new ObservableCollection<TestButtonData>();

        public bool ShowButton
        {
            get => showButton;
            set
            {
                if (SetProperty(ref showButton, value))
                {
                    RaisePropertyChanged(nameof(ShowProgress));
                }
            }
        }

        public int RadioValue { get => radioValue; set => SetProperty(ref radioValue, value); }

        public bool ShowTest
        {
            get => showTest;
            set => SetProperty(ref showTest, value);
        }

        public bool ShowProgress => !ShowButton;

        private ImageDescriptor image;
        private string stopwatch = "";
        private bool showButton = true;
        private bool showTest = true;
        private float sliderValue = 0.2f;
        private int radioValue = 1;
        private readonly IObjectFactory objectFactory;
        private readonly ISystemDispatcher systemDispatcher;
        private readonly IDispatcher dispatcher;

        private ICommand addButtonTest;

        public MainWindowViewModel(
            IObjectFactory objectFactory,
            ISequencer sequencer,
            ISystemDispatcher systemDispatcher,
            IDispatcher dispatcher) : base(objectFactory)
        {
            TestCommand = new SyncCommand(Test);

            addButtonTest = new SyncCommand(AddButtonTest, CanExecuteAddButtonTest);

            this.objectFactory = objectFactory;
            this.systemDispatcher = systemDispatcher;
            this.dispatcher = dispatcher;

            for(var idx =0; idx < 20; ++idx)
            {
                Items.Add(new TestButtonData($"Test {idx}", addButtonTest));
            }

            sequencer.Run(Count());
            Test();
        }

        private bool CanExecuteAddButtonTest(object parameter)
        {
            if (parameter == null) return false;

            var oldTest = (TestButtonData)parameter;
            if (oldTest.Text.EndsWith("1")) return false;
            return true;
        }

        private void AddButtonTest(object parameter)
        {
            var oldTest = (TestButtonData)parameter;
            Items.Insert(0, new TestButtonData(oldTest.Text, addButtonTest));
        }

        private IEnumerable<Sequence> Count()
        {
            DateTime last = DateTime.MinValue;
            while (true)
            {
                var time = DateTime.Now;
                if (last.Minute != time.Minute) Test();

                if (last.Second != time.Second)
                {
                    last = time;
                    Stopwatch = $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}";
                }
                yield return Sequence.WaitForSeconds(0.1);
            }
        }

        private async void Test()
        {
            if (ShowButton == false) return;
            try
            {
                ShowButton = false;

                await Task.Run(async () =>
               {
                   var request = WebRequest.Create("https://picsum.photos/3840/2160");

                   Stream dataStream = null;

                   using (var response = await request.GetResponseAsync())
                   {
                       dataStream = new MemoryStream();
                       using (var respStream = response.GetResponseStream())
                       {
                           await respStream.CopyToAsync(dataStream);
                       }
                       dataStream.Seek(0, SeekOrigin.Begin);
                   }

                   try
                   {
                       var image = objectFactory.Create<Image>(dataStream);

                       dispatcher.BeginInvoke(() =>
                       {
                           var oldImage = Image;
                           Image = image;
                           ShowButton = true;

                           oldImage.Image?.Dispose();
                       });
                   }
                   catch
                   {
                       ShowButton = true;
                   }
                   finally
                   {
                       dataStream.Dispose();
                   }
               });
            }
            catch
            {
                ShowButton = true;
            }
        }

        protected override Task InitializeFirstPage()
        {
            return Navigation.Navigate<MainPageViewModel>(out var viewModel);
        }
    }
}
