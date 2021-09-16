using System;

namespace XxSchema.Contracts
{
    public class XxSchemaPatternAttribute: Attribute
    {
        public XxSchemaPatternAttribute(params string[] patterns)
        {
            Patterns = patterns;
        }

        public string[] Patterns { get; }
    }
}
