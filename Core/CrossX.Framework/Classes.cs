using System.Linq;
using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"[A-Za-z][A-Za-z0-9-_]*([ ]+[A-Za-z][A-Za-z0-9-_]*)*")]
    public struct Classes
    {
        public string[] Values { get; }

        public Classes(string values)
        {
            Values = values.Split(' ').Where(o => o.Length > 0).ToArray();
        }

        public static implicit operator Classes(string str) => new Classes(str);
    }

    [XxSchemaPattern("[A-Za-z][A-Za-z0-9]*")]
    public struct Id
    {
        public string Value { get; }

        public Id(string id) => Value = id;

        public static implicit operator Id(string str) => new Id(str);
        public static implicit operator string(Id id) => id.Value;
    }
}
