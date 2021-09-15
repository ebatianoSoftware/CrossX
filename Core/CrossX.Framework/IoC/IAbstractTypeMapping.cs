using System;

namespace CrossX.Framework.IoC
{
    internal interface IAbstractTypeMapping
    {
        bool FindMapping(Type abstractType, out Type implementationType);
    }
}
