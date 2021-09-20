using CrossX.Abstractions.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.XxTools
{
    public class XxDefinitionObjectFactory
    {
        private readonly IObjectFactory objectFactory;

        public XxDefinitionObjectFactory(IObjectFactory objectFactory)
        {
            this.objectFactory = objectFactory;
        }

        public TInstance CreateObject<TInstance>(XxElement element)
        {
            if (!typeof(TInstance).IsAssignableFrom(element.Type)) throw new InvalidDataException($"Cannot create {typeof(TInstance).Name} from {element.Type.Name}!");
            var instance = objectFactory.Create(element.Type);

            if (element.Children != null)
            {
                if (instance is IElementsContainer container)
                {
                    var children = new List<object>();

                    foreach (var childElement in element.Children)
                    {
                        var child = CreateObject<object>(childElement);
                        children.Add(child);
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

                }
                else
                {
                    prop.Key.SetValue(instance, prop.Value);
                }
            }

            return (TInstance)instance;
        }
    }
}
