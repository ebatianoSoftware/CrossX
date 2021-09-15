using System;

namespace CrossX.Framework
{
    public static class CsHelpers
    {
        public static T Set<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}
