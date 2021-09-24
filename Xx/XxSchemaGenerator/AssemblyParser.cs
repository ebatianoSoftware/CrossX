using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xx;

namespace xxsgen
{
    internal class AssemblyParser
    {
        private const string bindablePattern = @"\{.*\}";
        private readonly Assembly assembly;

        List<SimpleType> simpleTypes = new List<SimpleType>();
        List<ComplexType> complexTypes = new List<ComplexType>();

        public XxShemaInfo[] Infos { get; }

        public IEnumerable<SimpleType> SimpleTypes => simpleTypes;
        public IEnumerable<ComplexType> ComplexTypes => complexTypes;
        public HashSet<string> Namespaces = new HashSet<string>();

        private SimpleType unknownType;

        private Dictionary<Assembly, XxShemaInfo[]> shemaInfos = new Dictionary<Assembly, XxShemaInfo[]>();

        public AssemblyParser(Assembly assembly)
        {
            this.assembly = assembly;
            Infos = GetInfos(assembly);
        }

        public bool Parse()
        {
            unknownType = new SimpleType
            {
                Type = null,
                Name = "unknown",
                Namespace = Infos.Last().Namespace,
                Patterns = new[] { bindablePattern },
                Values = new string[0]
            };

            simpleTypes.Add(unknownType);

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                var ca = type.GetCustomAttribute<XxSchemaExport>();
                if (ca != null)
                {
                    var ct = FindComplexType(type.AsType());
                    ct.Exportable = true;
                }
            }

            return true;
        }

        private ComplexType ParseComplexType(Type type, XxSchemaExport ca)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty | BindingFlags.Public);

            var attributes = new List<Attribute>();

            var allPropertiesBindable = type.GetCustomAttribute<XxSchemaBindable>()?.Bindable ?? false;

            foreach (var property in properties)
            {
                if (property.GetSetMethod() == null) continue;
                if (property.GetCustomAttribute<XxSchemaIgnore>() != null) continue;

                bool bindable = allPropertiesBindable;

                var bindableAttr = property.GetCustomAttribute<XxSchemaBindable>();

                if (bindableAttr != null) bindable = bindableAttr.Bindable;
                var st = FindSimpleType(property.PropertyType, bindable);

                attributes.Add(new Attribute
                {
                    Bindable = bindable,
                    Name = property.Name,
                    Type = st
                });
            }

            XxChildrenMode childrenMode = type.IsAbstract ? XxChildrenMode.Zero : ca?.ChildrenMode ?? XxChildrenMode.Zero;

            ComplexType[] childTypes = null;

            if(childrenMode != XxChildrenMode.Zero && ca?.ChildTypes?.Length > 0)
            {
                childTypes = new ComplexType[ca.ChildTypes.Length];

                for(var idx =0; idx < childTypes.Length; ++idx)
                {
                    childTypes[idx] = FindComplexType(ca.ChildTypes[idx]);
                }
            }

            GetInfo(type, false, out var @namespace, out var name);

            var ct = new ComplexType
            {
                Attributes = attributes.ToArray(),
                Name = name,
                Type = type,
                ChildrenMode = childrenMode,
                ChildrenTypes = childTypes,
                Namespace = @namespace,
                BaseType = FindComplexType(type.BaseType)
            };

            complexTypes.Add(ct);
            return ct;
        }

        private ComplexType FindComplexType(Type type)
        {
            if (type == typeof(object)) return null;

            var ct = complexTypes.FirstOrDefault(o => o.Type == type);
            if (ct != null) return ct;

            var ca = type.GetCustomAttribute<XxSchemaExport>();
            return ParseComplexType(type, ca);
        }

        private SimpleType FindSimpleType(Type type, bool bindable)
        {
            if(!builtinTypes.ContainsKey(type) && (type.IsClass || type.IsInterface))
            {
                return unknownType;
            }

            GetInfo(type, bindable, out var @namespace, out var name);

            var st = simpleTypes.FirstOrDefault(o => o.Name == name && o.Namespace == @namespace);
            if (st != null) return st;

            var patterns = type.GetCustomAttribute<XxSchemaPatternAttribute>()?.Patterns ?? Array.Empty<string>();
            var values = ParseConstValues(type);

            if(builtinTypes.TryGetValue(type, out var tuple))
            {
                patterns = new[] { tuple.Item2 };
                values = tuple.Item3;
            }

            if (bindable)
            {
                var list = patterns.ToList();
                list.Add(bindablePattern);
                patterns = list.ToArray();
            }

            st = new SimpleType
            {
                Namespace = @namespace,
                Name = name,
                Type = type,
                Patterns = patterns,
                Values = values
            };

            simpleTypes.Add(st);
            return st;
        }

        private string[] ParseConstValues(Type type)
        {
            if (type.IsEnum)
            {
                return Enum.GetNames(type);
            }

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return fields.Where(f => f.IsInitOnly).Select(o => o.Name).ToArray();
        }

        private Dictionary<Type, Tuple<string, string, string[]>> builtinTypes = new Dictionary<Type, Tuple<string, string, string[]>>
        {
            {typeof(int), Tuple.Create("System.Int", "[0-9]+", Array.Empty<string>()) },
            {typeof(float), Tuple.Create("System.Float", "[+-]?([0-9]*[.])?[0-9]+", Array.Empty<string>())},
            {typeof(double), Tuple.Create("System.Float", "[+-]?([0-9]*[.])?[0-9]+", Array.Empty<string>())},
            {typeof(string), Tuple.Create("System.String", ".*", Array.Empty<string>())},
            {typeof(bool), Tuple.Create("System.Boolean", "", new[]{"True","False"})},
        };

        private XxShemaInfo[] GetInfos(Assembly assembly)
        {
            if (!shemaInfos.TryGetValue(assembly, out var infos))
            {
                var shemaInfoTypes = assembly.DefinedTypes.Where(o => o.BaseType == typeof(XxShemaInfo)).ToArray();
                infos = new XxShemaInfo[shemaInfoTypes.Length];
                for (var idx = 0; idx < infos.Length; ++idx)
                {
                    infos[idx] = (XxShemaInfo)Activator.CreateInstance(shemaInfoTypes[idx]);
                }
                infos = infos.OrderByDescending(o => o.RootNamespace.Length).ToArray();
                shemaInfos.Add(assembly, infos);
            }
            return infos;
        }
        
        private void GetInfo(Type type, bool bindable, out string @namespace, out string name)
        {
            var bindablePostfix = bindable ? "-Bindable" : "";

            name = type.FullName + bindablePostfix;
            if(name.EndsWith("Element"))
            {
                name = name.Substring(0, name.Length - "Element".Length);
            }

            var infos = GetInfos(type.Assembly);

            foreach (var info in infos)
            {
                if (type.Namespace.StartsWith(info.RootNamespace))
                {
                    if (!Namespaces.Contains(info.Namespace))
                    {
                        Namespaces.Add(info.Namespace);
                    }

                    @namespace = info.Namespace;
                    return;
                }
            }

            if (builtinTypes.TryGetValue(type, out var tuple))
            {
                name = tuple.Item1 + bindablePostfix;
                @namespace = Infos.Last().Namespace;
                return;
            }
            if(!bindable)
            {
                Console.WriteLine("Unknown type: {0}", type.FullName);
            }
            name = unknownType.Name;
            @namespace = Infos.Last().Namespace;
        }
    }
}
