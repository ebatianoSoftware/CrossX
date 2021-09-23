using CrossX.Abstractions.IoC;
using CrossX.Framework.Binding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.XxTools
{
    public class XxDefinitionObjectFactory
    {
        private readonly IObjectFactory objectFactory;
        private readonly IElementTypeMapping elementTypeMapping;
        private readonly IBindingService bindingService;

        public XxDefinitionObjectFactory(IObjectFactory objectFactory, IElementTypeMapping elementTypeMapping, IBindingService bindingService)
        {
            this.objectFactory = objectFactory;
            this.elementTypeMapping = elementTypeMapping;
            this.bindingService = bindingService;
        }

        public TInstance CreateObject<TInstance>(XxElement element)
        {
            if (!typeof(TInstance).IsAssignableFrom(element.Type)) throw new InvalidDataException($"Cannot create {typeof(TInstance).Name} from {element.Type.Name}!");
            var instance = objectFactory.Create(element.Type, element.Namespaces, elementTypeMapping);

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
                            var child = CreateObject<object>(childElement);
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

            foreach (var prop in element.Properties)
            {
                if(prop.Value is XxBindingString binding)
                {
                    if (prop.Key.GetCustomAttribute<XxBindingStringAttribute>() != null)
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
                        if(constructor != null)
                        {
                            var obj = constructor.Invoke(new[] { prop.Value });
                            prop.Key.SetValue(instance, obj);
                        }
                    }
                }
            }

            return (TInstance)instance;
        }
    }
}
