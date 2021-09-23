using CrossX.Framework.XxTools;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    [XxSchemaExport(XxChildrenMode.Multiple, typeof(Styles), typeof(Resources), typeof(ThemeElement))]
    public sealed class ApplicationElement : IElementsContainer
    {
        public void InitChildren(IEnumerable<object> elements)
        {
        }
    }
}
