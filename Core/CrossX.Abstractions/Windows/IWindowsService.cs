using System.Threading.Tasks;

namespace CrossX.Abstractions.Windows
{
    public interface IWindowsService
    {
        TViewModel CreateWindow<TViewModel>(CreateWindowMode createMode = CreateWindowMode.Modal, TViewModel vm = null) where TViewModel : class;
        Task<TResult> ShowPopup<TResult, TViewModel>(TResult defaultResult = default) where TViewModel : class, IModalContext<TResult>;
        void Exit();
    }
}
