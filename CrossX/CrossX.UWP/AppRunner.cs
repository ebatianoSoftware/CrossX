using CrossX.Core;
using CrossX.IoC;
using CrossX.UWP.UWP;
using Windows.ApplicationModel.Core;

namespace CrossX.UWP
{
    public class AppRunner<TApp> where TApp: class, IApp
    {
        private readonly IServiceProvider serviceProvider;

        public AppRunner(IServiceProvider serviceProvider = null)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run<TAppParameters>(TAppParameters appParameters)
        {
            var builder = new ScopeBuilder();

            var services = builder
                        .WithParent(serviceProvider)
                        .RegisterUwpTypes();

            CoreApplication.Run(new ViewSource<TApp>(builder, appParameters));
        }
    }
}
