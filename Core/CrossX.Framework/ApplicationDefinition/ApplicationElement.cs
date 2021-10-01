using CrossX.Abstractions.IoC;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xx;
using Xx.Toolkit;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(StylesElement), typeof(ResourcesElement), typeof(ThemeElement), typeof(ImportElement))]
    public sealed class ApplicationElement : IElementsContainer
    {
        private readonly XxDefinitionObjectFactory definitionObjectFactory;
        private readonly IObjectFactory objectFactory;
        private readonly IElementTypeMapping elementTypeMapping;

        public ApplicationElement(XxDefinitionObjectFactory definitionObjectFactory, IObjectFactory objectFactory, IElementTypeMapping elementTypeMapping)
        {
            this.definitionObjectFactory = definitionObjectFactory;
            this.objectFactory = objectFactory;
            this.elementTypeMapping = elementTypeMapping;
        }

        public void InitChildren(IEnumerable<object> elements)
        {
            foreach (var el in elements.Where(o=>o is ImportElement).Cast<ImportElement>())
            {
                var parts = el.Path.Split(';');
                if(parts.Length == 2)
                {
                    var assembly = Assembly.Load(parts[1]);
                    using(var stream = assembly.GetManifestResourceStream(parts[0]))
                    {
                        if(stream != null)
                        {
                            try
                            {
                                var parser = objectFactory.Create<XxFileParser>(elementTypeMapping);
                                var element = parser.Parse(stream);

                                definitionObjectFactory.CreateObject<ApplicationElement>(element);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}
