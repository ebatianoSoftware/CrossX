using System.Windows.Input;

namespace CrossX.Abstractions.Menu
{
    public interface ICommandContainer
    {
        ICommand Command { get; }
    }
}
