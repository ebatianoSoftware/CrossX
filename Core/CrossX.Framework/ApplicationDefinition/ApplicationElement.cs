using CrossX.Framework.Core;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using Xx;
using Xx.Definition;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Styles), typeof(Resources), typeof(ThemeElement))]
    public sealed class ApplicationElement : IAppValues, IElementsContainer
    {
        private readonly Dictionary<SelectorKey, XxElement> styles = new Dictionary<SelectorKey, XxElement>();
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly Dictionary<string, object> resources = new Dictionary<string, object>();

        public object FindResource(string name)
        {
            if (resources.TryGetValue(name, out var value)) return value;
            return null;
        }

        public IEnumerable<Style> GetStyles(Type type, string classes)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string name)
        {
            if (values.TryGetValue(name, out var value)) return value;
            return null;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach(var el in elements)
            {
                switch(el)
                {
                    case Styles styles:
                        InitChildren(styles.Values);
                        break;

                    case Style style:
                        if (style.Element != null)
                        {
                            styles.Add(style.Key, style.Element);
                        }
                        break;

                    case ThemeElement theme:
                        foreach(var val in theme.Values)
                        {
                            values[val.Key] = val.Value;
                        }
                        break;

                    case Resources resources:
                        InitChildren(resources.Values);
                        break;

                    case Resource res:
                        if (res.Value != null)
                        {
                            if (resources.TryGetValue(res.Key, out var resource))
                            {
                                (resource as IDisposable)?.Dispose();
                            }
                            resources[res.Key] = res.Value;
                        }
                        break;
                }
            }
        }
    }
}
