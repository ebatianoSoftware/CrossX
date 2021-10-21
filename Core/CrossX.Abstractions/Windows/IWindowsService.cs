using System.Threading.Tasks;

namespace CrossX.Abstractions.Windows
{
    public interface IWindowsService
    {
        TViewModel CreateWindow<TViewModel>(CreateWindowMode createMode = CreateWindowMode.Modal, TViewModel vm = null, params object[] parameters) where TViewModel : class;
        Task<TResult> ShowPopup<TResult, TViewModel>(TResult defaultResult = default, TViewModel viewModel = null) where TViewModel : class, IModalContext<TResult>;
        void Exit();
    }
}
