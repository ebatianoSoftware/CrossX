using CrossX.Abstractions.IoC;
using CrossX.Framework.XxTools;
using System.Reflection;
using Xx;
using Xx.Toolkit;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport]
    public class ImportElement
    {
        private readonly XxDefinitionObjectFactory definitionObjectFactory;
        private readonly IObjectFactory objectFactory;
        private readonly IElementTypeMapping elementTypeMapping;

        public ImportElement(XxDefinitionObjectFactory definitionObjectFactory, IObjectFactory objectFactory, IElementTypeMapping elementTypeMapping)
        {
            this.definitionObjectFactory = definitionObjectFactory;
            this.objectFactory = objectFactory;
            this.elementTypeMapping = elementTypeMapping;
        }

        public string Path
        {
            set
            {
                var parts = value.Split(';');
                if (parts.Length == 2)
                {
                    var assembly = Assembly.Load(parts[1]);
                    using (var stream = assembly.GetManifestResourceStream(parts[0]))
                    {
                        if (stream != null)
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
