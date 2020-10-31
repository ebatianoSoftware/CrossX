using CrossX.Core;
using CrossX.IoC;
using CrossX.UWP.UWP;
using Windows.ApplicationModel.Core;

namespace CrossX.UWP
{
    public class AppRunner<TApp> where TApp: class, IApp
    {
        private readonly IServicesProvider serviceProvider;

        public AppRunner(IServicesProvider serviceProvider = null)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run<TAppParameters>(TAppParameters appParameters)
        {
            var builder = new ScopeBuilder()
                .WithParent(serviceProvider)
                .RegisterUwpServices()
                .RegisterUwpTypes();
            
            CoreApplication.Run(new ViewSource<TApp>(builder, appParameters));
        }
    }
}
