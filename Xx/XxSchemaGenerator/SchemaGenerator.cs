using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace xxsgen
{
    internal class SchemaGenerator
    {
        private readonly string targetNamespace;
        private readonly SimpleType[] simpleTypes;
        private readonly ComplexType[] complexTypes;

        private Dictionary<string, string> namespaceAliases = new Dictionary<string, string>();
        
        private readonly string xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        private readonly string schemaTemplate;
        private readonly string simpleTemplate;
        private readonly string complexTemplate;

        private readonly string attributeTemplate;
        private readonly string enumTemplate;
        private readonly string emptyEnum;

        private readonly string importTemplate;
        private readonly string aliasTemplate;

        private readonly string elementTemplate;

        private readonly string childrenNode;
        private readonly string specificChildrenNode;

        private char c1 = 'a';
        private char c2 = 'a';

        private string GetAlias(string ns)
        {
            if (namespaceAliases.TryGetValue(ns, out var alias)) return alias;

            alias = $"{c1}{c2}";
            c2++;
            if (c2 > 'z')
            {
                c2 = 'a';
                c1++;
            }

            namespaceAliases.Add(ns, alias);
            return alias;
        }

        public SchemaGenerator(string targetNamespace, IEnumerable<SimpleType> simpleTypes, IEnumerable<ComplexType> complexTypes)
        {
            this.targetNamespace = targetNamespace;
            this.simpleTypes = simpleTypes.Where( o=>o.Namespace == targetNamespace).ToArray();
            this.complexTypes = complexTypes.Where(o => o.Namespace == targetNamespace).ToArray();

            namespaceAliases.Add(xmlSchemaNamespace, "xsd");
            namespaceAliases.Add(targetNamespace, "xx");

            schemaTemplate = LoadFromAssembly("Templates.Schema.xml");
            simpleTemplate = LoadFromAssembly("Templates.SimpleType.xml");
            complexTemplate = LoadFromAssembly("Templates.ComplexType.xml");
            attributeTemplate = LoadFromAssembly("Templates.Attribute.xml");
            enumTemplate = LoadFromAssembly("Templates.Enumeration.xml");
            importTemplate = LoadFromAssembly("Templates.Import.xml");
            aliasTemplate = LoadFromAssembly("Templates.Alias.xml");
            emptyEnum = LoadFromAssembly("Templates.EmptyEnumeration.xml");
            childrenNode = LoadFromAssembly("Templates.ChildrenNode.xml");
            specificChildrenNode = LoadFromAssembly("Templates.SpecificChildrenNode.xml");
            elementTemplate = LoadFromAssembly("Templates.Element.xml");
        }

        private string LoadFromAssembly(string name)
        {
            name = typeof(SchemaGenerator).Assembly.GetName().Name + '.' + name;

            using (var stream = typeof(SchemaGenerator).Assembly.GetManifestResourceStream(name))
            {
                var sr = new StreamReader(stream, System.Text.Encoding.UTF8);

                return sr.ReadToEnd();
            }
        }

        public XDocument Generate()
        {
            string simpleTypes = "";

            foreach(var st in this.simpleTypes)
            {
                var values = "";

                foreach(var val in st.Values)
                {
                    values += enumTemplate.Replace("{Value}", val);
                }

                if(values == "")
                {
                    values = emptyEnum;
                }

                var text = simpleTemplate
                    .Replace("{Name}", st.Name)
                    .Replace("{Patterns}", string.Join('|', st.Patterns))
                    .Replace("{Enumerations}", values);

                simpleTypes += text;
            }

            string complexTypes = GenerateComplexTypes();

            string elements = "";

            foreach(var ct in this.complexTypes)
            {
                if (!ct.Exportable) continue;

                var @base = GetAlias(ct.Namespace) + ':' + ct.Name;
                elements += elementTemplate
                    .Replace("{Base}", @base)
                    .Replace("{Name}", ct.Name.Split('.').Last());
            }

            string aliases = "";

            foreach (var al in namespaceAliases)
            {
                aliases += aliasTemplate.Replace("{Alias}", al.Value).Replace("{Namespace}", al.Key);
            }

            string imports = "";

            foreach (var al in namespaceAliases)
            {
                if (al.Key == targetNamespace || al.Key == xmlSchemaNamespace) continue;
                imports += importTemplate.Replace("{Namespace}", al.Key);
            }

            var xml = schemaTemplate
                .Replace("{TargetNamespace}", targetNamespace)
                .Replace("{NamespaceAliases}", aliases)
                .Replace("{NamespaceImports}", imports)
                .Replace("{SimpleTypes}", simpleTypes)
                .Replace("{ComplexTypes}", complexTypes)
                .Replace("{Elements}", elements);
            
            return XDocument.Parse(xml);
        }

        private string GenerateComplexTypes()
        {
            const string systemObject = "xx:System.Object";
            string text = "";

            foreach(var ct in complexTypes)
            {
                var @base = systemObject;

                if(ct.BaseType != null)
                {
                    @base = GetAlias(ct.BaseType.Namespace) + ':' + ct.BaseType.Name;
                }

                var children = "";
                if (ct.ChildrenMode != Xx.XxChildrenMode.Zero)
                {
                    
                    string min = "0";
                    string max = "unbounded";

                    switch(ct.ChildrenMode)
                    {
                        case Xx.XxChildrenMode.MaxOne:
                            max = "1";
                            break;

                        case Xx.XxChildrenMode.OnlyOne:
                            max = "1";
                            min = "1";
                            break;
                    }

                    if (ct.ChildrenTypes?.Length > 0)
                    {
                        children = "";
                        for(var idx =0; idx < ct.ChildrenTypes.Length; ++idx)
                        {
                            foreach (var complexType in complexTypes)
                            {
                                var child = ct.ChildrenTypes[idx];

                                if (complexType == child || complexType.IsDerrivedFrom(child))
                                {
                                    if (complexType.Exportable)
                                    {
                                        var type = GetAlias(complexType.Namespace) + ':' + complexType.Name;
                                        var name = complexType.Name.Split('.').Last();

                                        children += specificChildrenNode.Replace("{type}", type).Replace("{name}", name);
                                    }
                                }
                            }
                        }

                        children = childrenNode.Replace("{Children}", children);
                    }
                    else
                    {
                        children = childrenNode.Replace("{Children}", "<xsd:any/>");
                    }

                    children = children.Replace("{max}", max).Replace("{min}", min);
                }

                var attributes = "";

                foreach(var attr in ct.Attributes)
                {
                    var attrType = GetAlias(attr.Type.Namespace) + ':' + attr.Type.Name;
                    attributes += attributeTemplate
                        .Replace("{Name}", attr.Name)
                        .Replace("{Type}", attrType);
                }

                var complex = complexTemplate
                    .Replace("{Name}", ct.Name)
                    .Replace("{Base}", @base)
                    .Replace("{Children}", children)
                    .Replace("{Attributes}", attributes);


                text += complex;
            }

            return text;
        }
    }
}
