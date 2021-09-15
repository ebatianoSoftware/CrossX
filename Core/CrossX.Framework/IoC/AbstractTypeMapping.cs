using System;
using System.Collections.Generic;

namespace CrossX.Framework.IoC
{
    internal class AbstractTypeMapping : IAbstractTypeMapping
    {
        private readonly Dictionary<Type, Type> typesMapping = new Dictionary<Type, Type>();

        public IAbstractTypeMapping Parent { get; set; }

        public bool FindMapping(Type abstractType, out Type implementationType)
        {
            if (typesMapping.TryGetValue(abstractType, out implementationType)) return true;

            if (abstractType.IsGenericType)
            {
                var arguments = abstractType.GenericTypeArguments;
                var genType = abstractType.GetGenericTypeDefinition();

                if (typesMapping.TryGetValue(genType, out implementationType))
                {
                    implementationType = implementationType.MakeGenericType(arguments);
                    return true;
                }
            }

            if (Parent != null && Parent.FindMapping(abstractType, out implementationType)) return true;
            return false;
        }

        public void AddMapping(Type from, Type to) => typesMapping.Add(from, to);
    }
}
