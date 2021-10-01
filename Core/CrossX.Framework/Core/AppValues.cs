using CrossX.Framework.ApplicationDefinition;
using System;
using System.Collections.Generic;
using Xx.Definition;

namespace CrossX.Framework.Core
{
    internal class AppValues : IAppValues
    {
        private readonly Dictionary<SelectorKey, XxElement> styles = new Dictionary<SelectorKey, XxElement>();
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly Dictionary<string, object> resources = new Dictionary<string, object>();

        public object GetResource(string name)
        {
            if (resources.TryGetValue(name, out var value)) return value;
            return null;
        }

        public IEnumerable<StyleElement> GetStyles(Type type, string classes)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string name)
        {
            if (values.TryGetValue(name, out var value)) return value;
            return null;
        }

        public void RegisterResource(string name, object obj)
        {
            if(resources.TryGetValue(name, out var value) && value is IDisposable disposable)
            {
                disposable.Dispose();
            }
            resources[name] = obj;
        }

        public void RegisterStyle(SelectorKey name, XxElement element)
        {
            styles[name] = element;
        }

        public void RegisterValue(string name, object value)
        {
            values[name] = value;
        }
    }
}
