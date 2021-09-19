using System;

namespace CrossX.Framework.Navigation
{
    internal interface INavigationController
    {
        event EventHandler<NavigationRequest> NavigationRequested;
    }
}
