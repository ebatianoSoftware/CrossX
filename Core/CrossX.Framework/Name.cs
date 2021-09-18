using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern("[A-Za-z_]{1}[A-Za-z0-9_]*")]
    public struct Name
    {
        public string Value { get; }
        public Name(string name) => Value = name;

        public static implicit operator Name(string str) => new Name(str);
        public static implicit operator string(Name name) => name.Value;
    }
}
