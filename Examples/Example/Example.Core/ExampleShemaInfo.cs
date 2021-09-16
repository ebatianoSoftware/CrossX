using XxSchema.Contracts;

namespace CrossX.Framework.Meta
{
    public class ExampleShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Example";
        public override bool AllPropertiesBindable => true;
        public override string SchemaOutputFile => "crossx-example.xsd";
    }
}
