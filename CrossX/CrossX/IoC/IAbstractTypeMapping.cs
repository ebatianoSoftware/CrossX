using System;

namespace CrossX.IoC
{
    internal interface IAbstractTypeMapping
    {
        bool FindMapping(Type abstractType, out Type implementationType);
    }
}
