using CrossX.Framework.Graphics;
using CrossX.Framework.Navigation;
using CrossX.Framework.Transforms;
using System.Windows.Input;

namespace CrossX.Framework.UI.Containers
{
    public class NavigationFrame: View
    {
        public ICommand AttachedToFrameCommand { get; set; }
        public object NavigationController 
        {
            set
            {
                if(navigationController != null)
                {
                    navigationController.NavigationRequested -= OnNavigationRequested;
                }

                navigationController = value as INavigationController;

                if (navigationController != null)
                {
                    navigationController.NavigationRequested += OnNavigationRequested;
                }
            }
        }

        public NavigationTransform NavigateToTransform { get; set; }
        public NavigationTransform NavigateFromTransform { get; set; }

        private INavigationController navigationController;

        public NavigationFrame(IRedrawService redrawService): base(redrawService)
        {

        }

        private void OnNavigationRequested(object sender, NavigationRequest request)
        {
            
        }
    }
}
