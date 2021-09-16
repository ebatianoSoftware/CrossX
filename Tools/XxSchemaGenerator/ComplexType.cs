using System;
using System.Linq;

namespace xxsgen
{
    internal class ComplexType
    {
        public Type Type;
        public string Namespace;
        public bool Exportable;
        public string Name;
        public Attribute[] Attributes;
        public ComplexType[] Children;
        public ComplexType BaseType;

        public override string ToString()
        {
            return $"Complex Type\nName: {Name}\nNamespace: {Namespace}\nBase: {BaseType?.Name}\nChildren: " + string.Join(',', Children.Select(o => o.Name).ToArray()) + "\n" +
                "Attributes:\n\t" + string.Join("\n\t", Attributes.Select(o => $"{o.Name} : {o.Type.Name}").ToArray()) + "\n";
        }
    }
}
