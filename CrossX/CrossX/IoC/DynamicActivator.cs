using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CrossX.IoC
{
    public static class DynamicActivator
    {
        public static object Create(Type type, params object[] parameters)
        {
            return New(type, null, parameters);
        }

        public static TObject Create<TObject>(params object[] parameters) where TObject: class
        {
            return (TObject)New(typeof(TObject), null, parameters);
        }

        internal static object New(Type type, IServiceProvider serviceProvider, params object[] parameters)
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
                            if (param == null) continue;
                            if (param.ParameterType.IsAssignableFrom(par.GetType()))
                            {
                                arguments[idx] = par;
                                shouldResolveService = false;
                                break;
                            }
                        }

                        if (shouldResolveService)
                        {
                            try
                            {
                                arguments[idx] = serviceProvider?.GetService(param.ParameterType);
                            }
                            catch (KeyNotFoundException)
                            {
                                arguments[idx] = null;
                                Console.WriteLine($"Couldn't resolve service {param.ParameterType.Name}");
                            }

                            if (arguments[idx] == null)
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
