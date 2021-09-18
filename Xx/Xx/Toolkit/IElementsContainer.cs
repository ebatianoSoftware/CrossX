using System.Collections.Generic;

namespace Xx.Toolkit
{
    public interface IElementsContainer
    {
        void InitChildren(IEnumerable<object> elements);
    }
}
