using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrossX.Abstractions.Mvvm
{
    public abstract class BindingContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return false;
            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }

    public abstract class UIBindingContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private object dataContext;
        public object DataContext { get => dataContext; set => SetProperty(ref dataContext, value); }

        protected virtual void OnPropertyChanging(string propertyName)
        {

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {

        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return false;
            OnPropertyChanging(propertyName);
            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
