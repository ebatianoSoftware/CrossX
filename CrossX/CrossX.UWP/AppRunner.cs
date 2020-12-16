using CrossX.Core;
using CrossX.UWP.UWP;
using CrossX.DxCommon;
using Windows.ApplicationModel.Core;
using XxIoC;

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
                .RegisterDirectXServices()
                .RegisterDirectXTypes();

            if(serviceProvider != null)
            {
                builder.WithParent(serviceProvider);
            }
            
            CoreApplication.Run(new ViewSource<TApp>(builder, appParameters));
        }
    }
}
