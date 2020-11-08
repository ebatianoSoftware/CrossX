using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrossX.Forms
{
    public abstract class ObservableDataModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void FirePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected void FirePropertyChanging(string name) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));

        protected virtual void SetProperty<T>(ref T property, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(value, property)) return;

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            OnPropertyChanged(name);
        }

        protected virtual void OnPropertyChanged(string name)
        {
        }
    }
}
