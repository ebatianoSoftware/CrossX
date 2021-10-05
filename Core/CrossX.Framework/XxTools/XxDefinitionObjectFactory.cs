using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using CrossX.Framework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.XxTools
{
    public class XxDefinitionObjectFactory
    {
        private readonly IObjectFactory objectFactory;
        private readonly IBindingService bindingService;
        private readonly IAppValues appValues;
        private readonly IElementTypeMapping elementTypeMapping;

        public XxDefinitionObjectFactory(IObjectFactory objectFactory, IBindingService bindingService, IAppValues appValues, IElementTypeMapping elementTypeMapping)
        {
            this.objectFactory = objectFactory;
            this.bindingService = bindingService;
            this.appValues = appValues;
            this.elementTypeMapping = elementTypeMapping;
        }

        public TInstance CreateObject<TInstance>(XxElement element, bool resolveBindings = true)
        {
            if (!typeof(TInstance).IsAssignableFrom(element.Type)) throw new InvalidDataException($"Cannot create {typeof(TInstance).Name} from {element.Type.Name}!");
            var instance = objectFactory.Create(element.Type, this, elementTypeMapping);

            if(instance is IStoreElement storeElement)
            {
                storeElement.Element = element;
            }

            var properties = GetProperties(element.Type, element.Properties);

            foreach (var prop in properties)
            {
                if (prop.Value is XxBindingString binding)
                {
                    if (prop.Key.GetCustomAttribute<XxBindingStringAttribute>() != null || !resolveBindings)
                    {
                        prop.Key.SetValue(instance, $"{{{binding.Value}}}");
                    }
                    else
                    {
                        bindingService.AddBinding(instance, prop.Key, binding.Value);
                    }
                }
                else
                {
                    if (prop.Key.PropertyType.IsAssignableFrom(prop.Value.GetType()))
                    {
                        prop.Key.SetValue(instance, prop.Value);
                    }
                    else
                    {
                        var constructor = prop.Key.PropertyType.GetConstructor(new Type[] { prop.Value.GetType() });
                        if (constructor != null)
                        {
                            var obj = constructor.Invoke(new[] { prop.Value });
                            prop.Key.SetValue(instance, obj);
                        }
                    }
                }
            }

            bool childrenAsDefinitions = element.Type.GetCustomAttribute<ChildrenAsDefinitionsAttribute>() != null;

            if (element.Children != null)
            {
                if (instance is IElementsContainer container)
                {
                    var children = new List<object>();

                    foreach (var childElement in element.Children)
                    {
                        if (childrenAsDefinitions)
                        {
                            children.Add(childElement);
                        }
                        else
                        {
                            var child = CreateObject<object>(childElement, !childrenAsDefinitions && resolveBindings);
                            children.Add(child);
                        }
                    }
                    container.InitChildren(children);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot add child elements to {element.Type.Name}");
                }
            }

            return (TInstance)instance;
        }

        public Dictionary<PropertyInfo, object> GetProperties(Type type, IReadOnlyDictionary<PropertyInfo, object> properties)
        {
            var newDictionary = new Dictionary<PropertyInfo, object>(properties);

            var classesProp = properties.FirstOrDefault(o => o.Key.Name == "Classes");

            string[] classes = Array.Empty<string>();

            if (classesProp.Value is Classes cl)
            {
                classes = cl.Values;
            }

            var styles = appValues.GetStyles(type, classes);

            foreach(var style in styles)
            {
                foreach(var prop in style.Properties)
                {
                    if(!newDictionary.ContainsKey(prop.Key))
                    {
                        newDictionary.Add(prop.Key, prop.Value);
                    }
                }
            }

            return newDictionary;
        }
    }
}
