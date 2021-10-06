using CrossX.Abstractions.IoC;
using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;
using Xx.Toolkit;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(StylesElement), typeof(ResourcesElement), typeof(ThemeElement), typeof(ImportElement))]
    public sealed class ApplicationElement : IElementsContainer
    {
        public void InitChildren(IEnumerable<object> elements)
        {       
        }
    }
}
