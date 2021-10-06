using CrossX.Abstractions.IoC;
using CrossX.Abstractions.Navigation;
using CrossX.Framework.Graphics;
using CrossX.Framework.Navigation;

namespace CrossX.Framework.Core
{
    public static class InitServices
    {
        public static IScopeBuilder WithCrossTypes(this IScopeBuilder scopeBuilder)
        {
            return scopeBuilder
                .WithType<NavigationImpl>().As<INavigation>()
                .WithType<ImageCache>().As<IImageCache>().AsSingleton();
        }
    }
}
