using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.IoC
{
    public class ScopeBuilder: IScopeBuilder
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

        public bool HasRegisteredInstance(Type type)
        {
            return registrations.Find(o => o.Singleton && o.ResolutionTypes.Contains(type)) != null;
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
                if (reg.Type != null)
                {
                    foreach (var type in reg.ResolutionTypes)
                    {
                        typesMapping.Add(type, reg.Type);
                    }
                }
            }
            var servicesContainer = new ServiceContainer(parentServiceProvider, typesMapping);

            foreach (var reg in registrations)
            {
                if (reg.Singleton)
                {
                    foreach (var type in reg.ResolutionTypes)
                    {
                        if (!TryRegisterInstance(servicesContainer, type)) throw new Exception();
                    }
                }
            }

            builded = true;
            return servicesContainer;
        }

       

        private bool TryRegisterInstance(ServiceContainer container, Type serviceType)
        {
            if (builded) throw new InvalidOperationException();

            if (serviceType == typeof(IServicesProvider))
            {
                return false;
            }

            if (container.TryResolveInstance(serviceType, out var service) && service != null)
            {
                return true;
            }

            foreach (var reg in registrations)
            {
                if (reg.Resolves(serviceType))
                {
                    if (reg.Instance != null)
                    {
                        container.RegisterInstance(serviceType, reg.Instance);
                        return true;
                    }

                    if (reg.Type != null)
                    {
                        service = DynamicActivator.Create(reg.Type, container);

                        foreach (var type in reg.ResolutionTypes)
                        {
                            container.RegisterInstance(type, service);
                        }
                        return true;
                    }
                }
            }
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
