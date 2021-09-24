﻿using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using CrossX.Framework;
using CrossX.Framework.Async;
using CrossX.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public ICommand TestCommand { get; }

        public ImageDescriptor Image { get => image; private set => SetProperty(ref image, value); }

        public string Stopwatch { get => stopwatch; private set => SetProperty(ref stopwatch, value); }

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

        public bool ShowProgress => !ShowButton;

        private ImageDescriptor image;
        private string stopwatch = "";
        private bool showButton = true;
        private readonly IObjectFactory objectFactory;
        private readonly ISystemDispatcher systemDispatcher;
        private readonly IDispatcher dispatcher;

        public MainWindowViewModel(
            IObjectFactory objectFactory,
            ISequencer sequencer,
            ISystemDispatcher systemDispatcher,
            IDispatcher dispatcher) : base(objectFactory)
        {
            TestCommand = new SyncCommand(Test);
            this.objectFactory = objectFactory;
            this.systemDispatcher = systemDispatcher;
            this.dispatcher = dispatcher;
            sequencer.Run(Count());
            Test();
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

                var image = await systemDispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        return objectFactory.Create<Image>(dataStream);
                    }
                    finally
                    {
                        dataStream.Dispose();
                    }
                });

                await Task.Delay(500);

                dispatcher.BeginInvoke(() =>
                {
                    var oldImage = Image;
                    Image = image;
                    oldImage.Image?.Dispose();
                    ShowButton = true;
                });
            }
            catch
            {

            }
        }

        protected override Task InitializeFirstPage()
        {
            return Navigation.Navigate<MainPageViewModel>(out var viewModel);
        }
    }
}
