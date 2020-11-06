using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.IoC
{
    public class ScopeBuilder: IServicesProvider, IScopeBuilder
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

            public void As(Type type) => asTypes.Add(type);
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

        public ScopeBuilder WithType(Type type)
        {
            var registration = new Registration(null, type);
            registrations.Add(registration);
            return this;
        }

        public ScopeBuilder As<TType>()
        {
            registrations.Last().As(typeof(TType));
            return this;
        }

        public ScopeBuilder As(Type type)
        {
            registrations.Last().As(type);
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
            if (TryResolveInstance(serviceType, out var instance)) return instance;
            throw new Exception();
        }

        public TService GetService<TService>() => (TService)GetService(typeof(TService));

        public bool TryResolveInstance(Type serviceType, out object instance)
        {
            if (builded) throw new InvalidOperationException();

            if (serviceType == typeof(IServicesProvider))
            {
                instance = this;
                return true;
            }

            if (services.TryGetValue(serviceType, out var service) && service != null)
            {
                instance = service;
                return true;
            }

            foreach (var reg in registrations)
            {
                if (reg.Resolves(serviceType))
                {
                    if (reg.Instance != null)
                    {
                        services[serviceType] = reg.Instance;
                        instance = reg.Instance;
                        return true;
                    }

                    if (reg.Type != null)
                    {
                        services.Add(serviceType, null);
                        service = DynamicActivator.New(reg.Type, this);

                        foreach (var type in reg.ResolutionTypes)
                        {
                            services[type] = service;
                        }
                        instance = service;
                        return true;
                    }
                }
            }

            if (parentServiceProvider != null)
            {
                return parentServiceProvider.TryResolveInstance(serviceType, out instance);
            }
            instance = null;
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

        IScopeBuilder IScopeBuilder.WithType<TType>() => WithType<TType>();
        IScopeBuilder IScopeBuilder.WithType(Type type) => WithType(type);
        IScopeBuilder IScopeBuilder.As<TType>() => As<TType>();
        IScopeBuilder IScopeBuilder.As(Type type) => As(type);
        IScopeBuilder IScopeBuilder.AsSingleton() => AsSingleton();
        IScopeBuilder IScopeBuilder.AsSelf() => AsSelf();
        IScopeBuilder IScopeBuilder.WithInstance(object instance) => WithInstance(instance);
    }
}
