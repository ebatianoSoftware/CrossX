using CrossX.Framework.XxTools;
using System.Collections.Generic;
using System.Linq;
using Xx;

namespace CrossX.Framework.Transforms
{
    [XxSchemaExport(XxChildrenMode.Multiple)]
    public class Group : TransformationBase, IElementsContainer
    {
        private TransformationBase[] transforms;
        public void InitChildren(IEnumerable<object> elements)
        {
            transforms = elements.Where(o => o is TransformationBase).Cast<TransformationBase>().ToArray();
        }
    }
}
