using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xx.Toolkit
{
    public class ElementTypeMapping : IElementTypeMapping
    {
        private Dictionary<string, Dictionary<string, Type>> types = new Dictionary<string, Dictionary<string, Type>>();

        public ElementTypeMapping(Assembly assembly)
        {
            ScanAssemblies(assembly);
        }

        private void ScanAssemblies(Assembly assembly)
        {
            var assemblyList = new List<Assembly>(new[] { assembly });
            var assemblyNames = assembly.GetReferencedAssemblies();

            var name = typeof(ElementTypeMapping).Assembly.GetName();

            foreach(var assemblyName in assemblyNames)
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
            var shemaInfoTypes = assembly.DefinedTypes.Where(o => o.BaseType == typeof(XxShemaInfo)).ToArray();
            var infos = shemaInfoTypes.Select(o => (XxShemaInfo)Activator.CreateInstance(o)).OrderByDescending(o => o.RootNamespace.Length).ToArray();
            
            foreach (var info in infos)
            {
                var typesMap = new Dictionary<string, Type>();
                types.Add(info.Namespace, typesMap);
            }

            var allMatchingTypes = assembly.DefinedTypes.Where(o => !o.IsAbstract && o.GetCustomAttribute<XxSchemaExport>() != null);

            foreach (var type in allMatchingTypes)
            {
                var namespaceName = "";
                foreach (var info in infos)
                {
                    if (type.Namespace.StartsWith(info.RootNamespace))
                    {
                        namespaceName = info.Namespace;
                        break;
                    }
                }

                if(namespaceName != "")
                {
                    var name = type.Name;
                    if (name.EndsWith("Element"))
                    {
                        name = name.Substring(0, name.Length - "Element".Length);
                    }

                    types[namespaceName].Add(name, type);
                }
            }
        }

        public Type FindElement(string @namespace, string name)
        {
            if (!types.TryGetValue(@namespace, out var typesMap)) return null;
            return typesMap.TryGetValue(name, out var type) ? type : null;
        }
    }
}
