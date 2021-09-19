using CrossX.Framework.Navigation;
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

        private INavigationController navigationController;

        public NavigationFrame()
        {

        }

        private void OnNavigationRequested(object sender, NavigationRequest request)
        {
            
        }
    }
}
