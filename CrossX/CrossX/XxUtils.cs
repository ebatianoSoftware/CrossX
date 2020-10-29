using System;
using System.Collections.Generic;
using System.Text;

namespace CrossX
{
    public static class XxUtils
    {
        public static void Swap<T>(ref T val1, ref T val2)
        {
            T m = val1;
            val1 = val2;
            val2 = m;
        }
    }
}
