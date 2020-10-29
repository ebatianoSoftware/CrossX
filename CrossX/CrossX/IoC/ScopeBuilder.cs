using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.IoC
{
    public class ScopeBuilder: IServicesProvider
    {
        public class Registration
        {
            private HashSet<Type> asTypes = new HashSet<Type>();
            public readonly object Instance;
            public readonly Type Type;
            public bool Singleton { get; private set; }

            public Registration(object instance, Type type)
            {
                Instance = instance;
                Type = type;
                Singleton = instance != null;
            }

            public void As<TType>() => asTypes.Add(typeof(TType));
            public void AsSingleton() => Singleton = true;
            public void AsSelf() => asTypes.Add(Type);

            public IEnumerable<Type> ResolutionTypes => asTypes;

            public bool Resolves(Type type) => asTypes.Contains(type);
        }

        private readonly List<Registration> registrations = new List<Registration>();
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        private IServicesProvider parentServiceProvider;

        private bool builded = false;
        
        public ScopeBuilder WithParent(IServicesProvider serviceProvider)
        {
            parentServiceProvider = serviceProvider;
            return this;
        }

        public ScopeBuilder WithType<TType>()
        {
            var registration = new Registration(null, typeof(TType));
            registrations.Add(registration);
            return this;
        }

        public ScopeBuilder As<TType>()
        {
            registrations.Last().As<TType>();
            return this;
        }

        public ScopeBuilder AsSingleton()
        {
            registrations.Last().AsSingleton();
            return this;
        }

        public ScopeBuilder AsSelf()
        {
            registrations.Last().AsSelf();
            return this;
        }

        public ScopeBuilder WithInstance(object instance)
        {
            var registration = new Registration(instance, null);
            registrations.Add(registration);
            return this;
        }

        public IServicesProvider Build()
        {
            var typesMapping = new Dictionary<Type, Type>();

            foreach (var reg in registrations)
            {
                if (reg.Singleton)
                {
                    foreach (var type in reg.ResolutionTypes)
                    {
                        GetService(type);
                    }
                }
                else if(reg.Type != null)
                {
                    foreach(var type in reg.ResolutionTypes)
                    {
                        typesMapping.Add(type, reg.Type);
                    }
                }
            }

            builded = true;
            return new ServiceContainer(typesMapping, services, parentServiceProvider);
        }

        public object GetService(Type serviceType)
        {
            if (builded) throw new InvalidOperationException();

            if (serviceType == typeof(IServicesProvider)) return this;

            if (services.TryGetValue(serviceType, out var service) && service != null) return service;

            foreach(var reg in registrations)
            {
                if(reg.Resolves(serviceType))
                {
                    if(reg.Instance != null)
                    {
                        services[serviceType] = reg.Instance;
                        return reg.Instance;
                    }

                    if (reg.Type != null)
                    {
                        services.Add(serviceType, null);
                        service = DynamicActivator.New(reg.Type, this);
                        services[serviceType] = service;
                        return service;
                    }
                }
            }

            if(parentServiceProvider != null)
            {
                return parentServiceProvider.GetService(serviceType);
            }

            throw new Exception();
        }

        public TService GetService<TService>() => (TService)GetService(typeof(TService));

        public bool TryResolveInstance(Type type, out object instance)
        {
            throw new NotSupportedException();
        }

        public bool TryResolveInstance<TType>(out TType instance)
        {
            throw new NotSupportedException();
        }
    }
}
