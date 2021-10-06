using CrossX.Abstractions.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CrossX.Framework.Binding
{
    internal class ConversionService : IConversionService
    {
        struct Key : IEquatable<Key>
        {
            public Type From;
            public Type To;

            public override bool Equals(object obj)
            {
                return obj is Key key && Equals(key);
            }

            public bool Equals(Key other)
            {
                return EqualityComparer<Type>.Default.Equals(From, other.From) &&
                       EqualityComparer<Type>.Default.Equals(To, other.To);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(From, To);
            }
        }

        private readonly Dictionary<Key, IImplicitValueConverter> converters = new Dictionary<Key, IImplicitValueConverter>();

        public ConversionService(Assembly assembly)
        {
            var assemblyList = new List<Assembly>(new[] { assembly });
            var assemblyNames = assembly.GetReferencedAssemblies();

            var name = typeof(IImplicitValueConverter).Assembly.GetName();

            foreach (var assemblyName in assemblyNames)
            {
                var refAssembly = Assembly.Load(assemblyName);
                var names = refAssembly.GetReferencedAssemblies();
                if (names.FirstOrDefault(o => o.FullName == name.FullName) == null) continue;
                assemblyList.Add(refAssembly);
            }

            foreach (var assembly2 in assemblyList)
            {
                ScanAssembly(assembly2);
            }
        }

        private void ScanAssembly(Assembly assembly)
        {
            var ct = typeof(IImplicitValueConverter);
            foreach(var type in assembly.DefinedTypes.Where( o=> ct.IsAssignableFrom(o)))
            {
                var attr = type.GetCustomAttribute<ImplicitValueConverterAttribute>();
                if (attr == null) continue;

                var instance = Activator.CreateInstance(type) as IImplicitValueConverter;

                converters[new Key {
                    From = attr.From,
                    To = attr.To
                }] = instance;
            }
        }

        public object Convert(object value, Type type)
        {
            if (value == null) return null;

            if (type.IsAssignableFrom(value.GetType())) return value;

            if(converters.TryGetValue(new Key
            {
                From = value.GetType(),
                To = type
            }, out var converter))
            {
                return converter.Convert(value);
            }

            throw new InvalidCastException();
        }
    }
}
