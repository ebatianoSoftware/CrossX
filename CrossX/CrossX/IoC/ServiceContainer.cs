using System;
using System.Collections.Generic;

namespace CrossX.IoC
{
    internal class ServiceContainer : IServicesProvider, IObjectFactory, IAbstractTypeMapping
    {
        private readonly Dictionary<Type, Type> typesMapping;
        private readonly Dictionary<Type, object> instances;
        private readonly IServicesProvider serviceProvider;

        public ServiceContainer(Dictionary<Type, Type> typesMapping, Dictionary<Type, object> instances, IServicesProvider serviceProvider)
        {
            this.typesMapping = typesMapping;
            this.instances = instances;
            this.serviceProvider = serviceProvider;
        }

        public object Create(Type type, params object[] parameters)
        {
            if (!FindMapping(type, out var implType)) implType = type;
            return DynamicActivator.New(implType, this, parameters);
        }

        public TObject Create<TObject>(params object[] parameters) => (TObject)Create(typeof(TObject), parameters);
        
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServicesProvider)) return this;
            if (serviceType == typeof(IObjectFactory)) return this;
            if (serviceType == typeof(IAbstractTypeMapping)) return this;

            if (instances.TryGetValue(serviceType, out var service)) return service;

            if( serviceProvider != null )
            {
                if (serviceProvider.TryResolveInstance(serviceType, out service)) return service;
            }

            if (!FindMapping(serviceType, out var implType)) throw new KeyNotFoundException();
            return DynamicActivator.New(implType, this);
        }

        public TService GetService<TService>() => (TService)GetService(typeof(TService));

        public bool TryResolveInstance(Type type, out object instance)
        {
            if (type == typeof(IServicesProvider) || type == typeof(IObjectFactory) || type == typeof(IAbstractTypeMapping))
            {
                instance = this;
                return true;
            }

            if (instances.TryGetValue(type, out instance)) return true;

            if (serviceProvider != null && serviceProvider.TryResolveInstance(type, out instance)) return true;
            return false;
        }

        public bool TryResolveInstance<TType>(out TType instance)
        {
            if (TryResolveInstance(typeof(TType), out var obj))
            {
                instance = (TType)obj;
                return true;
            }
            instance = default;
            return false;
        }

        public bool FindMapping(Type abstractType, out Type implementationType)
        {
            if (typesMapping.TryGetValue(abstractType, out implementationType)) return true;

            var mapper = serviceProvider?.GetService<IAbstractTypeMapping>();
            if (mapper != null && mapper.FindMapping(abstractType, out implementationType)) return true;

            return false;
        }
    }
}
