using CrossX.Framework.ApplicationDefinition;
using System;
using System.Collections.Generic;

namespace CrossX.Framework.Core
{
    public interface IAppValues
    {
        IEnumerable<Style> GetStyles(Type type, string classes);
        object GetValue(string name);
        object FindResource(string name);
    }
}
