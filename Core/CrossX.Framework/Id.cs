using XxSchema.Contracts;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"[A-Za-z][A-Za-z0-9-_]*")]
    public struct Id
    {
        public string Value { get; }

        public Id(string value)
        {
            Value = value;
        }

        public static implicit operator string (Id id) => id.Value;
        public static implicit operator Id (string str) => new Id(str);
    }
}
