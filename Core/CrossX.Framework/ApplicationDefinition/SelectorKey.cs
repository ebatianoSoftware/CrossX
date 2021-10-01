using System;
using System.Collections.Generic;

namespace CrossX.Framework.ApplicationDefinition
{
    public struct SelectorKey: IEquatable<SelectorKey>
    {
        public Type Type;
        public string Name;

        public bool Equals(SelectorKey other)
        {
            return Type == other.Type && Name == other.Name;
        }
    }
}
