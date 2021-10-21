using CrossX.Abstractions.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossX.Framework.IoC
{
    public class ScopeBuilder : IScopeBuilder, IServicesProvider
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

        private readonly ServicesProvider servicesProvider = new ServicesProvider();
        private readonly ObjectFactory objectFactory = new ObjectFactory();
        private readonly AbstractTypeMapping abstractTypeMapping = new AbstractTypeMapping();

        private bool builded = false;

        public ScopeBuilder(IServicesProvider parentServicesProvider = null)
        {
            servicesProvider.ObjectFactory = objectFactory;
            servicesProvider.TypeMapping = abstractTypeMapping;
            objectFactory.ServicesProvider = servicesProvider;
            objectFactory.TypeMapping = abstractTypeMapping;

            WithType<ScopeBuilder>().As<IScopeBuilder>();

            if (parentServicesProvider != null)
            {
                servicesProvider.Parent = parentServicesProvider;
                abstractTypeMapping.Parent = parentServicesProvider.GetService<IAbstractTypeMapping>();
            }
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
            var registration = new Registration(instance, instance.GetType());
            registrations.Add(registration);
            return this;
        }

        public IServicesProvider Build()
        {
            foreach (var reg in registrations)
            {
                if (reg.Type != null)
                {
                    foreach (var type in reg.ResolutionTypes)
                    {
                        abstractTypeMapping.AddMapping(type, reg.Type);
                    }
                }
            }

            servicesProvider.RegisterInstance(typeof(IObjectFactory), objectFactory);
            servicesProvider.RegisterInstance(typeof(IAbstractTypeMapping), abstractTypeMapping);

            foreach (var reg in registrations)
            {
                if (reg.Singleton)
                {
                    foreach (var type in reg.ResolutionTypes)
                    {
                        if (!TryRegisterInstance(type)) throw new Exception();
                    }
                }
            }

            builded = true;
            return servicesProvider;
        }


        private bool TryRegisterInstance(Type serviceType)
        {
            if (builded) throw new InvalidOperationException();

            if (serviceType == typeof(IServicesProvider))
            {
                return false;
            }

            if (servicesProvider.TryResolveInstance(serviceType, out var service) && service != null)
            {
                return true;
            }

            foreach (var reg in registrations)
            {
                if (reg.Resolves(serviceType))
                {
                    if (reg.Instance != null)
                    {
                        servicesProvider.RegisterInstance(serviceType, reg.Instance);
                        return true;
                    }

                    if (reg.Type != null)
                    {
                        service = DynamicActivator.Create(reg.Type, this);

                        foreach (var type in reg.ResolutionTypes)
                        {
                            servicesProvider.RegisterInstance(type, service);
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

        public object GetService(Type serviceType)
        {
            if (TryResolveInstance(serviceType, out var instance)) return instance;
            throw new KeyNotFoundException();
        }

        public TService GetService<TService>()
        {
            if (TryResolveInstance<TService>(out var instance)) return instance;
            throw new KeyNotFoundException();
        }

        public bool TryResolveInstance(Type type, out object instance)
        {
            if (servicesProvider.TryResolveInstance(type, out instance)) return true;

            if (TryRegisterInstance(type))
            {
                instance = servicesProvider.GetService(type);
                return true;
            }

            return false;
        }

        public bool TryResolveInstance<TType>(out TType instance)
        {
            instance = default;
            if (TryResolveInstance(typeof(TType), out var instanceObj))
            {
                instance = (TType)instanceObj;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
        }
    }
}
