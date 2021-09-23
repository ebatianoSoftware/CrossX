using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Example.Core.ViewModels
{
    public class MainWindowViewModel : NavigationFrameViewModel
    {
        public ICommand TestCommand { get; }

        public string ImageUrl { get; private set; } = "https://picsum.photos/400/300";

        public MainWindowViewModel(IObjectFactory objectFactory) : base(objectFactory)
        {
            TestCommand = new SyncCommand(Test);
        }

        Random rand = new Random();
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
