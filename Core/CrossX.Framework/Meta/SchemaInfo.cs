using Xx;

namespace CrossX.Framework.Meta
{
    public class ApplicationDefinition_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.ApplicationDefinition";
        public override string SchemaOutputFile => "crossx-framework-application-definition.xsd";

        public override string RootNamespace => "CrossX.Framework.ApplicationDefinition";
    }

    public class Root_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework";
        public override string SchemaOutputFile => "crossx-framework.xsd";
        public override string RootNamespace => "CrossX.Framework";
    }

    public class Transforms_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.Transforms";
        public override string SchemaOutputFile => "crossx-framework-transforms.xsd";
        public override string RootNamespace => "CrossX.Framework.Transforms";
    }

    public class Drawables_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.Drawables";
        public override string SchemaOutputFile => "crossx-framework-drawables.xsd";
        public override string RootNamespace => "CrossX.Framework.Drawables";
    }

    public class UI_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.UI";
        public override string SchemaOutputFile => "crossx-framework-ui.xsd";
        public override string RootNamespace => "CrossX.Framework.UI";
    }

    public class UIGlobal_ShemaInfo : XxShemaInfo
    {
        public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.UI.Global";
        public override string SchemaOutputFile => "crossx-framework-ui-global.xsd";
        public override string RootNamespace => "CrossX.Framework.UI.Global";
    }

    //public class UITemplates_ShemaInfo : XxShemaInfo
    //{
    //    public override string Namespace => "https://crossx.support/Schemas/CrossX.Framework.UI.Templates";
    //    public override string SchemaOutputFile => "crossx-framework-ui-templates.xsd";
    //    public override string RootNamespace => "CrossX.Framework.UI.Templates";
    //}
}
