using CrossX.Core;
using XxIoC;
using Windows.ApplicationModel.Core;

namespace CrossX.UWP.UWP
{
    internal class ViewSource<TApp>: IFrameworkViewSource where TApp: IApp
    {
        private readonly ScopeBuilder scopeBuilder;
        private readonly object appParameters;

        public ViewSource(ScopeBuilder scopeBuilder, object appParameters)
        {
            this.scopeBuilder = scopeBuilder;
            this.appParameters = appParameters;
        }

        public IFrameworkView CreateView()
        {
            return new ApplicationView<TApp>(scopeBuilder, appParameters);
        }
    }
}
