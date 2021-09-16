using System;

namespace xxsgen
{
    internal class SimpleType
    {
        public Type Type;
        public string Namespace;
        public string Name;
        public string[] Patterns;
        public string[] Values;

        public override string ToString()
        {
            return $"Simple Type\nName: {Name}\nNamespace: {Namespace}\nPatterns:\n\t" + string.Join("\n\t", Patterns) + "\nValues: " + string.Join(';', Values) + "\n";
        }
    }
}
