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
}
