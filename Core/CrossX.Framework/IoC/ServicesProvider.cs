using CrossX.Abstractions.IoC;
using System;
using System.Collections.Generic;

namespace CrossX.Framework.IoC
{
    internal class ServicesProvider : IServicesProvider
    {
        private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

        public IServicesProvider Parent { get; set; }
        public IObjectFactory ObjectFactory { get; set; }
        public IAbstractTypeMapping TypeMapping { get; set; }

        public void RegisterInstance(Type type, object instance) => instances.Add(type, instance);

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServicesProvider)) return this;

            if (instances.TryGetValue(serviceType, out var service)) return service;

            if (Parent != null)
            {
                if (Parent.TryResolveInstance(serviceType, out service)) return service;
            }

            if (TypeMapping.FindMapping(serviceType, out var _))
            {
                return ObjectFactory.Create(serviceType);
            }
            throw new Exception();
        }

        public TService GetService<TService>() => (TService)GetService(typeof(TService));

        public bool TryResolveInstance(Type type, out object instance)
        {
            if (type == typeof(IServicesProvider))
            {
                instance = this;
                return true;
            }

            if (type == typeof(IObjectFactory))
            {
                instance = ObjectFactory;
                return true;
            }

            if (type == typeof(IAbstractTypeMapping))
            {
                instance = TypeMapping;
                return true;
            }

            if (instances.TryGetValue(type, out instance)) return true;
            if (Parent != null && Parent.TryResolveInstance(type, out instance)) return true;
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

        public void Dispose()
        {
            foreach (var instance in instances)
            {
                if (instance.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            instances.Clear();
        }
    }
}
