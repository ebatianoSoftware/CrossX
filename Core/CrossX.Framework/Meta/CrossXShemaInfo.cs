using XxSchema.Contracts;

namespace CrossX.Framework.Meta
{
    public class CrossXShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework";
        public override bool AllPropertiesBindable => true;
        public override string SchemaOutputFile => "crossx-framework.xsd";
    }
}
