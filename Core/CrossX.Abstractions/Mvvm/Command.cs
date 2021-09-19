using System;
using System.Windows.Input;

namespace CrossX.Abstractions.Mvvm
{
    public class SyncCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public SyncCommand(Action execute) : this(o => execute()) { }

        public SyncCommand(Action<object> execute)
        {
            this.execute = execute;
        }

        public SyncCommand(Action execute, Func<bool> canExecute) : this(execute)
        {
            this.canExecute = o => canExecute();
        }

        public SyncCommand(Action<object> execute, Func<object, bool> canExecute) : this(execute)
        {
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => execute?.Invoke(parameter);

        public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
