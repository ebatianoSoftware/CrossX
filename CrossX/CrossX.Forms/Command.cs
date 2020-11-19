using System;
using System.Windows.Input;

namespace CrossX.Forms
{
    public class Command : ICommand
    {
        private readonly Action<object> onCommand;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action onCommand) : this(o => onCommand())
        {
        }

        public Command(Action onCommand, Func<bool> canExecute) : this( o=>onCommand(), o=>canExecute())
        {
        }

        public Command(Action<object> onCommand) : this(onCommand, null)
        { 
        }

        public Command(Action<object> onCommand, Func<object, bool> canExecute)
        {
            this.onCommand = onCommand;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            if(CanExecute(parameter))
            {
                onCommand?.Invoke(parameter);
            }
        }

        public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }
}
