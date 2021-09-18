using System;

namespace Xx.Toolkit
{
    public interface IElementTypeMapping
    {
        Type FindElement(string @namespace, string name);
    }
}
