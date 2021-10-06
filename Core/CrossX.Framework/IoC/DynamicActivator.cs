using CrossX.Abstractions.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CrossX.Framework.IoC
{
    public static class DynamicActivator
    {
        public static object Create(Type type, params object[] parameters)
        {
            return Create(type, null, parameters);
        }

        public static TObject Create<TObject>(params object[] parameters) where TObject : class
        {
            return (TObject)Create(typeof(TObject), null, parameters);
        }

        public static object Create(Type type, IServicesProvider serviceProvider, params object[] parameters)
        {
            IEnumerable<ConstructorInfo> constructors = null;
            try
            {
                constructors = type.GetTypeInfo().DeclaredConstructors;

                foreach (var constructor in constructors)
                {
                    var constructorParameters = constructor.GetParameters();

                    var arguments = new object[constructorParameters.Length];

                    var hasAllParams = true;

                    for (var idx = 0; idx < constructorParameters.Length; ++idx)
                    {
                        var param = constructorParameters[idx];

                        bool shouldResolveService = true;

                        foreach (var par in parameters)
                        {
                            if (par == null || param == null) continue;
                            if (param.ParameterType.IsAssignableFrom(par.GetType()))
                            {
                                arguments[idx] = par;
                                shouldResolveService = false;
                                break;
                            }
                        }

                        if (shouldResolveService)
                        {
                            object instance = null;
                            serviceProvider?.TryResolveInstance(param.ParameterType, out instance);

                            arguments[idx] = instance;

                            if (arguments[idx] == null && !param.IsOptional)
                            {
                                hasAllParams = false;
                                break;
                            }
                        }
                    }

                    if (hasAllParams)
                    {
                        return constructor.Invoke(arguments);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            var parametersList = string.Join(", ", parameters.Select(o => o.GetType().Name));

            var constructorsInfo = "";

            foreach (var constructor in constructors)
            {
                constructorsInfo += "\n";
                constructorsInfo += $"{type.Name}(";
                constructorsInfo += string.Join(", ", constructor.GetParameters().Select(o => o.ParameterType.Name)) + ")";
            }

            throw new Exception($"Cannot find proper constructor for {type.FullName} and given parameters.\n{parametersList}\nConstructors:{constructorsInfo}");
        }
    }
}
