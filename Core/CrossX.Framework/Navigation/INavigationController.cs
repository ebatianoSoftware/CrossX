using CrossX.Abstractions.Navigation;
using System;

namespace CrossX.Framework.Navigation
{
    internal interface INavigationController: INavigation
    {
        event EventHandler<NavigationRequest> NavigationRequested;
    }
}
