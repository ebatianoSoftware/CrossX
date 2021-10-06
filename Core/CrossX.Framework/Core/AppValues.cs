using CrossX.Framework.ApplicationDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xx;
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

        public IEnumerable<XxElement> GetStyles(Type type, string[] classes)
        {
            foreach(var cl in classes)
            {
                if (styles.TryGetValue(new SelectorKey { Type = type, Name = cl }, out var classStyle))
                {
                    yield return classStyle;
                }

                var baseType = type.BaseType;
                while(baseType != typeof(object) && !baseType.IsAbstract)
                {
                    if (styles.TryGetValue(new SelectorKey { Type = baseType, Name = cl }, out var classStyle2))
                    {
                        yield return classStyle2;
                    }
                    baseType = baseType.BaseType;
                }
            }

            if (styles.TryGetValue(new SelectorKey { Type = type, Name = "" }, out var typeStyle))
            {
                yield return typeStyle;

                var baseType = type.BaseType;
                while (baseType != typeof(object) && !baseType.IsAbstract)
                {
                    if (styles.TryGetValue(new SelectorKey { Type = baseType, Name = "" }, out var classStyle2))
                    {
                        yield return classStyle2;
                    }
                    baseType = baseType.BaseType;
                }
            }
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
