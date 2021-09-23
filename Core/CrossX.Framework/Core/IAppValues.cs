using CrossX.Framework.ApplicationDefinition;
using System;
using System.Collections.Generic;
using Xx.Definition;

namespace CrossX.Framework.Core
{
    public interface IAppValues
    {
        IEnumerable<Style> GetStyles(Type type, string classes);
        object GetValue(string name);
        object GetResource(string name);

        void RegisterResource(string name, object obj);
        void RegisterValue(string name, object value);

        void RegisterStyle(SelectorKey name, XxElement element);
    }
}
