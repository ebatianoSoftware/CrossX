using System;
using System.Collections.Generic;
using System.Data.Common;

namespace CrossX.IoC
{
    public class ScopeBuilder: IServiceProvider
    {
        public class Registration
        {
            private readonly ScopeBuilder scopeBuilder;
            private HashSet<Type> asTypes = new HashSet<Type>();
            public readonly object Instance;
            public readonly Type Type;
            public bool Singleton { get; private set; }

            public Registration(ScopeBuilder scopeBuilder, object instance, Type type)
            {
                this.scopeBuilder = scopeBuilder;
                this.Instance = instance;
                this.Type = type;
                Singleton = instance != null;
            }

            public Registration As<TType>()
            {
                asTypes.Add(typeof(TType));
                return this;
            }

            public Registration AsSingleton()
            {
                Singleton = true;
                return this;
            }

            public IEnumerable<Type> ResolutionTypes => asTypes;

            public bool Resolves(Type type) => asTypes.Contains(type);

            public Registration WithInstance(object instance) => scopeBuilder.WithInstance(instance);
            public Registration WithType<TType>() => scopeBuilder.WithType<TType>();

            public static implicit operator ScopeBuilder(Registration reg) => reg.scopeBuilder;
        }

        private readonly List<Registration> registrations = new List<Registration>();
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        private IServiceProvider parentServiceProvider;

        private bool builded = false;

        public ScopeBuilder WithParent(IServiceProvider serviceProvider)
        {
            parentServiceProvider = serviceProvider;
            return this;
        }

        public Registration WithType<TType>()
        {
            var registration = new Registration(this, null, typeof(TType));
            registrations.Add(registration);
            return registration;
        }
        

        public Registration WithInstance(object instance)
        {
            var registration = new Registration(this, instance, null);
            registrations.Add(registration);
            return registration;
        }

        public IServiceProvider Build()
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

            if (serviceType == typeof(IServiceProvider)) return this;

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
