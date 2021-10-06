using System.Collections.Generic;

namespace CrossX.Framework.XxTools
{
    public interface IElementsContainer
    {
        void InitChildren(IEnumerable<object> elements);
    }
}
