using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xx;

namespace xxsgen
{
    internal class AssemblyParser
    {
        private readonly Assembly assembly;
        Dictionary<Assembly, string> namespaces = new Dictionary<Assembly, string>();

        List<SimpleType> simpleTypes = new List<SimpleType>();
        List<ComplexType> complexTypes = new List<ComplexType>();

        public string TargetNamespace { get; private set; }
        public string SchemaOutputFile { get; private set; }
        private bool allPropertiesBindable;

        public IEnumerable<SimpleType> SimpleTypes => simpleTypes;
        public IEnumerable<ComplexType> ComplexTypes => complexTypes;
        public IEnumerable<string> Namespaces => namespaces.Values;

        private SimpleType anyType;

        public AssemblyParser(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public bool Parse()
        {
            var shemaInfoType = assembly.DefinedTypes.FirstOrDefault(o => o.BaseType == typeof(XxShemaInfo));

            if (shemaInfoType == null) return false;
            
            var info = (XxShemaInfo)Activator.CreateInstance(shemaInfoType);

            TargetNamespace = info.Namespace;
            SchemaOutputFile = info.SchemaOutputFile;
            allPropertiesBindable = info.AllPropertiesBindable;

            anyType = new SimpleType
            {
                Type = null,
                Name = "any",
                Namespace = TargetNamespace,
                Patterns = new []{ ".*" },
                Values = new string[0]
            };

            simpleTypes.Add(anyType);

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsAbstract) continue;

                var ca = type.GetCustomAttribute<XxSchemaExport>();
                
                if (ca != null)
                {
                    var ct = ParseComplexType(type.AsType(), ca);
                    ct.Exportable = true;
                }
            }

            return true;
        }

        private ComplexType ParseComplexType(Type type, XxSchemaExport ca)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty | BindingFlags.Public);

            var attributes = new List<Attribute>();

            foreach(var property in properties)
            {
                if (property.GetSetMethod() == null) continue;

                var st = FindSimpleType(property.PropertyType);

                bool bindable = allPropertiesBindable;

                if (property.GetCustomAttribute<XxSchemaBindable>() != null) bindable = true;

                attributes.Add(new Attribute
                {
                    Bindable = bindable,
                    Name = property.Name,
                    Type = st
                });
            }

            var children = new List<ComplexType>();

            if(ca?.ChildrenTypes != null && !type.IsAbstract)
            {
                foreach(var cht in ca.ChildrenTypes)
                {
                    var child = FindComplexType(cht);
                    children.Add(child);
                }
            }

            GetInfo(type, out var @namespace, out var name);

            var ct = new ComplexType
            {
                Attributes = attributes.ToArray(),
                Name = name,
                Type = type,
                Children = children.ToArray(),
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

        private SimpleType FindSimpleType(Type type)
        {
            GetInfo(type, out var @namespace, out var name);

            var st = simpleTypes.FirstOrDefault( o=>o.Name == name && o.Namespace == @namespace);
            if (st != null) return st;

            var patterns = type.GetCustomAttribute<XxSchemaPatternAttribute>()?.Patterns ?? Array.Empty<string>();
            var values = ParseConstValues(type);

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
            if(type.IsEnum)
            {
                return Enum.GetNames(type);
            }

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return fields.Where(f => f.IsInitOnly).Select(o => o.Name).ToArray();
        }

        private void GetInfo(Type type, out string @namespace, out string name)
        {
            name = type.FullName;
            if (namespaces.TryGetValue(type.Assembly, out var ns))
            {
                @namespace = ns;
                return;
            }

            var shemaInfoType = type.Assembly.DefinedTypes.FirstOrDefault(o => o.BaseType == typeof(XxShemaInfo));

            if(shemaInfoType != null)
            {
                var info = (XxShemaInfo)Activator.CreateInstance(shemaInfoType);
                namespaces.Add(type.Assembly, info.Namespace);
                @namespace = info.Namespace;
                return;
            }

            @namespace = "http://www.w3.org/2001/XMLSchema";
            if ( type == typeof(int))
            {
                name = "int";
                return;
            }

            if (type == typeof(float))
            {
                name = "float";
                return;
            }

            if (type == typeof(double))
            {
                name = "double";
                return;
            }

            if (type == typeof(string))
            {
                name = "string";
                return;
            }

            if (type == typeof(bool))
            {
                name = "boolean";
                return;
            }

            name = anyType.Name;
            @namespace = TargetNamespace;
        }
    }
}
