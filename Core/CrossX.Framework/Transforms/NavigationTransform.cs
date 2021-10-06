using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.Transforms
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class NavigationTransform: IElementsContainer
    {
        public int Duration { get; set; }

        public TransformationBase Transform { get; private set; }

        public void InitChildren(IEnumerable<object> elements)
        {
            Transform = elements.FirstOrDefault(o => o is TransformationBase) as TransformationBase;
        }
    }
}
