using System.Diagnostics;

namespace CrossX.Skia
{
    internal static class Utils
    {
        public static void AllocBuffer<T>(ref T[] buffer, int count)
        {
            if(buffer == null)
            {
                buffer = new T[count];
            }
            Debug.Assert(buffer.Length == count);
        }
    }
}
